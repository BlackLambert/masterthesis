using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SBaier.Master
{
    public class ContinentalPlatesFactory
    {
		private RandomPointsOnSphereGenerator _randomPointsGenerator;
		private SampleElimination3D _sampleElimination;
		private QuickSelector<Vector3> _quickSelector;

		public ContinentalPlatesFactory(
			RandomPointsOnSphereGenerator randomPointsGenerator,
			SampleElimination3D sampleElimination,
			QuickSelector<Vector3> quickSelector)
		{
			_randomPointsGenerator = randomPointsGenerator;
			_sampleElimination = sampleElimination;
			_quickSelector = quickSelector;
		}

		public ContinentalPlates Create(Parameters parameters)
		{
			float atmosphereRadius = parameters.Planet.AtmosphereRadius;
			int segmentsAmount = parameters.PlateSegments;
			int segmentSamples = (int)(segmentsAmount * parameters.SampleEliminationFactor);
			Vector3[] points = _randomPointsGenerator.Generate(segmentSamples, atmosphereRadius);
			float sphereSurface = GetSphereSurface(parameters);
			Vector3[] plateSegmentSites = _sampleElimination.Eliminate(points, segmentsAmount, sphereSurface).ToArray();
			Triangle[] triangles = CreatePlateSegmentTriangles(plateSegmentSites);
			VoronoiDiagram voronoi = CreatePlateSegmentVoronoiDiagram(plateSegmentSites, triangles);
			int[] biomePerSegment = CreateBiomePerSegment(parameters.Biomes, parameters);
			bool[] oceanicPerSegment = CreateOceanicPerSegment(parameters, plateSegmentSites);
			ContinentalPlateSegment[] segments = CreateContinentalPlateSegments(plateSegmentSites, voronoi, biomePerSegment, oceanicPerSegment);
			ContinentalPlate[] plates = CreatePlates(parameters, segments);
			Dictionary<Vector2Int, Vector2Int[]> plateBorders = FindPlateBorders(plates);
			ContinentalPlates result = new ContinentalPlates(segments, plates, plateSegmentSites, plateBorders, voronoi, triangles);
			return result;
		}

		private bool[] CreateOceanicPerSegment(Parameters parameters, Vector3[] plateSegmentSites)
		{
			int sitesAmount = parameters.Oceans + parameters.Continents;
			bool[] result = new bool[plateSegmentSites.Length];
			float atmosphereRadius = parameters.Planet.AtmosphereRadius;
			int siteSamples = (int)(sitesAmount * parameters.SampleEliminationFactor);
			Vector3[] points = _randomPointsGenerator.Generate(siteSamples, atmosphereRadius);
			float sphereSurface = GetSphereSurface(parameters);
			Vector3[] sites = _sampleElimination.Eliminate(points, sitesAmount, sphereSurface).ToArray();
			KDTree<Vector3> kdTree = new Vector3BinaryKDTree(sites, _quickSelector);
			for (int i = 0; i < result.Length; i++)
			{
				int siteIndex = kdTree.GetNearestTo(plateSegmentSites[i]);
				result[i] = siteIndex <= parameters.Oceans;
			}
			return result;
		}

		private int[] CreateBiomePerSegment(BiomeSettings[] biomes, Parameters parameters)
		{
			int frequencySum = biomes.Sum(b => b.Frequency) + 1;
			int[] result = new int[parameters.PlateSegments];
			for (int i = 0; i < result.Length; i++)
			{
				int randomNum = parameters.Seed.Random.Next(0, frequencySum);
				int frequencyArea = 0;
				for (int j = 0; j < biomes.Length; j++)
				{
					BiomeSettings biome = biomes[j];
					frequencyArea += biome.Frequency;
					if (frequencyArea > randomNum)
					{
						result[i] = j;
						break;
					}
				}
			}
			return result;
		}

		private Dictionary<Vector2Int, Vector2Int[]> FindPlateBorders(ContinentalPlate[] plates)
		{
			Dictionary<Vector2Int, Vector2Int[]> result = new Dictionary<Vector2Int, Vector2Int[]>();
			for (int i = 0; i < plates.Length; i++)
			{
				for (int j = i + 1; j < plates.Length; j++)
				{
					Vector2Int plateBorderIJ = new Vector2Int(i, j);
					Vector2Int plateBorderJI = new Vector2Int(j, i);
					Polygon[] polygones = new Polygon[] { plates[i], plates[j] };
					EdgesFinder edgesFinder = new SharedEdgesFinder(polygones);
					edgesFinder.Find();
					Vector2Int[] edges = edgesFinder.Edges.ToArray();
					result.Add(plateBorderJI, edges);
					result.Add(plateBorderIJ, edges);
				}
			}
			return result;
		}

		private VoronoiDiagram CreatePlateSegmentVoronoiDiagram(Vector3[] plateSegmentSites, Triangle[] triangles)
		{
			VoronoiDiagram diagram = new SphericalVoronoiCalculator(triangles, plateSegmentSites).CalculateVoronoiDiagram();
			return diagram;
		}

		private Triangle[] CreatePlateSegmentTriangles(Vector3[] plateSegmentSites)
		{
			SphericalDelaunayTriangulation triangulation = new SphericalDelaunayTriangulation(plateSegmentSites);
			triangulation.Create();
			Triangle[] triangles = triangulation.Result.ToArray();
			return triangles;
		}

		private ContinentalPlate[] CreatePlates(Parameters parameters, ContinentalPlateSegment[] segments)
		{
			float sphereSurface = GetSphereSurface(parameters);
			int plateSamples = (int)(parameters.Plates * parameters.SampleEliminationFactor);
			Vector3[] continentalPoints = _randomPointsGenerator.Generate(plateSamples, parameters.Planet.Data.Dimensions.AtmosphereThickness);
			Vector3[] plateSites = _sampleElimination.Eliminate(continentalPoints, parameters.Plates, sphereSurface).ToArray();
			KDTree<Vector3> platesKDTree = new Vector3BinaryKDTree(plateSites, _quickSelector);
			List<ContinentalPlateSegment>[] plateToSegments = new List<ContinentalPlateSegment>[parameters.Plates];
			for (int i = 0; i < plateToSegments.Length; i++)
				plateToSegments[i] = new List<ContinentalPlateSegment>();

			for (int i = 0; i < segments.Length; i++)
			{
				int index = platesKDTree.GetNearestTo(segments[i].Site);
				plateToSegments[index].Add(segments[i]);
			}

			ContinentalPlate[] result = new ContinentalPlate[parameters.Plates];
			Seed seed = new Seed(parameters.Seed.Random.Next());
			for (int i = 0; i < plateToSegments.Length; i++)
			{
				float movementAngle = (float)seed.Random.NextDouble() * 360;
				float movementStrength = (float)seed.Random.NextDouble();
				List<Polygon> polygons = new List<Polygon>(plateToSegments[i]);
				EdgesFinder finder = new UnsharedEdgesFinder(polygons);
				finder.Find();
				Vector2Int[] borders = finder.Edges.ToArray();
				result[i] = new ContinentalPlate(plateSites[i], plateToSegments[i].ToArray(), movementAngle, movementStrength, borders);
			}
			return result;
		}

		private float GetSphereSurface(Parameters parameters)
		{
			return 4 * Mathf.PI * parameters.Planet.AtmosphereRadius;
		}

		private ContinentalPlateSegment[] CreateContinentalPlateSegments(
			Vector3[] plateSegmentSites, 
			VoronoiDiagram voronoi, 
			int[] biomePerSegment,
			bool[] oceanic)
		{
			ContinentalPlateSegment[] result = new ContinentalPlateSegment[plateSegmentSites.Length];
			for (int i = 0; i < plateSegmentSites.Length; i++)
				result[i] = new ContinentalPlateSegment(voronoi.Regions[i], oceanic[i], biomePerSegment[i]);
			return result;
		}

		

		public class Parameters
		{

			public Parameters(Planet planet, 
				Seed seed, 
				int plateSegments, 
				int plates, 
				BiomeSettings[] biomes,
				float sampleEliminationFactor,
				int continents,
				int oceans)
			{
				Planet = planet;
				Seed = seed;
				PlateSegments = plateSegments;
				Plates = plates;
				Biomes = biomes;
				SampleEliminationFactor = sampleEliminationFactor;
				Continents = continents;
				Oceans = oceans;
			}

			public Planet Planet { get; }
			public Seed Seed { get; }
			public int PlateSegments { get; }
			public int Plates { get; }
			public BiomeSettings[] Biomes { get; }
			public float SampleEliminationFactor { get; }
			public int Continents { get; }
			public int Oceans { get; }
		}
	}
}