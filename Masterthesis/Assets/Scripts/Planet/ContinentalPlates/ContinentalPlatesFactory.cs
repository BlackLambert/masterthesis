using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SBaier.Master
{
    public class ContinentalPlatesFactory
    {
		private const float _minPlatesForce = 0.33f;

		private RandomPointsOnSphereGenerator _randomPointsGenerator;
		private SampleElimination3D _sampleElimination;
		private Vector3BinaryKDTreeFactory _kdTreeFactory;

		public ContinentalPlatesFactory(
			RandomPointsOnSphereGenerator randomPointsGenerator,
			SampleElimination3D sampleElimination,
			Vector3BinaryKDTreeFactory kdTreeFactory)
		{
			_randomPointsGenerator = randomPointsGenerator;
			_sampleElimination = sampleElimination;
			_kdTreeFactory = kdTreeFactory;
		}

		public ContinentalPlates Create(Parameters parameters)
		{
			int segmentsAmount = parameters.PlateSegments;
			Vector3[] plateSegmentSites = CreateSites(parameters, segmentsAmount);
			Triangle[] segmentsTriangles = CreatePlateSegmentTriangles(plateSegmentSites);
			VoronoiDiagram segmentsVoronoi = CreatePlateSegmentVoronoiDiagram(plateSegmentSites, segmentsTriangles);
			ContinentalPlateSegment[] segments = CreateContinentalPlateSegments(plateSegmentSites, segmentsVoronoi);
			ContinentalRegion[] regions = CreateRegions(parameters, plateSegmentSites, segments);
			ContinentalPlate[] plates = CreatePlates(parameters, regions);
			ConnectingBordersFinder connectingBordersFinder = new ConnectingBordersFinder(plates);
			connectingBordersFinder.Calcualte();
			Vector2Int[] neighbors = connectingBordersFinder.Neighbors;
			Dictionary<Vector2Int, Vector2Int[]> plateBorders = connectingBordersFinder.NeighborsToBorders;
			ContinentalPlates result = new ContinentalPlates(segments, regions, plates, plateBorders, segmentsVoronoi, segmentsTriangles, neighbors);
			return result;
		}

		private ContinentalRegion[] CreateRegions(Parameters parameters, Vector3[] plateSegmentSites, ContinentalPlateSegment[] segments)
		{
			int regionSitesAmount = parameters.Oceans + parameters.Continents;
			ContinentalRegion[] result = new ContinentalRegion[regionSitesAmount];
			Vector3[] regionSites = CreateSites(parameters, regionSitesAmount);
			KDTree<Vector3> regionSitesTree = _kdTreeFactory.Create(regionSites);
			List<int>[] regionToSegments = FindNearest(plateSegmentSites, regionSitesTree);
			for (int i = 0; i < result.Length; i++)
				result[i] = CreateRegion(parameters, regionToSegments, segments, regionSites, i);
			return result;
		}

		private ContinentalRegion CreateRegion(Parameters parameters, List<int>[] regionToSegments,
			ContinentalPlateSegment[] segments, Vector3[] regionSites, int index)
		{

			List<Polygon> polygons = ExtractPolygons(regionToSegments[index], segments);
			Vector2Int[] borders = GetBorders(polygons);
			ContinentalRegion.Type type = index <= parameters.Oceans ? ContinentalRegion.Type.Oceanic : ContinentalRegion.Type.ContinentalPlate;
			List<int> segmentIndices = regionToSegments[index];
			int[] segmentBiomes = ChooseBiomes(parameters, type, segmentIndices.Count);
			for (int i = 0; i < segmentIndices.Count; i++)
				segments[segmentIndices[i]].BiomeID = segmentBiomes[i];
			return new ContinentalRegion(type, regionToSegments[index], regionSites[index], borders);
		}

		private int[] ChooseBiomes(Parameters parameters, ContinentalRegion.Type type, int count)
		{
			List<int> possibleBiomes = new List<int>();
			int[] result = new int[count];
			int sum = 0;
			for (int i = 0; i < parameters.Biomes.Length; i++)
			{
				if (parameters.Biomes[i].RegionType != type)
					continue;
				possibleBiomes.Add(i);
				sum += parameters.Biomes[i].Frequency;
			}

			for (int i = 0; i < count; i++)
			{
				int randomNum = parameters.Seed.Random.Next(0, sum + 1);
				int frequencyArea = 0;
				for (int j = 0; j < possibleBiomes.Count; j++)
				{
					int biomeID = possibleBiomes[j];
					BiomeSettings biome = parameters.Biomes[biomeID];
					frequencyArea += biome.Frequency;
					if (frequencyArea >= randomNum)
					{
						result[i] = biomeID;
						break;
					}
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

		private ContinentalPlate[] CreatePlates(Parameters parameters, ContinentalRegion[] regions)
		{
			Vector3[] regionSites = regions.Select(r => r.Site).ToArray();
			Vector3[] plateSites = CreateSites(parameters, parameters.Plates);
			KDTree<Vector3> platesKDTree = _kdTreeFactory.Create(plateSites);
			List<int>[] plateToRegion = FindNearest(regionSites, platesKDTree);
			ContinentalPlate[] result = new ContinentalPlate[parameters.Plates];
			Seed seed = new Seed(parameters.Seed.Random.Next());
			for (int i = 0; i < plateToRegion.Length; i++)
				result[i] = CreatePlate(regions, plateSites, plateToRegion, seed, i);
			return result;
		}

		private ContinentalPlate CreatePlate(ContinentalRegion[] regions, Vector3[] plateSites, List<int>[] plateToRegion, Seed seed, int index)
		{
			Vector3 movementTangent = CalculateMovementTangent(plateSites[index], seed);
			float movementStrength = CalculateMovementStrength(seed);
			List<Polygon> polygons = ExtractPolygons(plateToRegion[index], regions);
			Vector2Int[] borders = GetBorders(polygons);
			return new ContinentalPlate(plateSites[index], plateToRegion[index].ToArray(), movementTangent, movementStrength, borders);
		}

		private static float CalculateMovementStrength(Seed seed)
		{
			float value = (float)seed.Random.NextDouble();
			float delta = value * (1 - _minPlatesForce);
			return _minPlatesForce + delta;
		}

		private Vector3 CalculateMovementTangent(Vector3 site, Seed seed)
		{
			float movementAngle = (float)seed.Random.NextDouble();
			Vector3 crossVector = site.normalized == Vector3.forward ? Vector3.right : Vector3.forward;
			Vector3 tangential = Vector3.Cross(site, crossVector).normalized;
			float sinWarpValueHalf = Mathf.Sin(Mathf.PI * movementAngle);
			float cosWarpValueHalf = Mathf.Cos(Mathf.PI * movementAngle);
			return new Quaternion(sinWarpValueHalf * site.x,
				sinWarpValueHalf * site.y,
				sinWarpValueHalf * site.z,
				cosWarpValueHalf).normalized * tangential;
		}

		private static Vector2Int[] GetBorders(IList<Polygon> polygons)
		{
			EdgesFinder finder = new UnsharedEdgesFinder(polygons);
			finder.Find();
			Vector2Int[] borders = finder.Edges.ToArray();
			return borders;
		}

		private List<Polygon> ExtractPolygons<T>(List<int> polygonIndices, IList<T> polygons) where T : Polygon
		{
			List<Polygon> result = new List<Polygon>();
			for (int i = 0; i < polygonIndices.Count; i++)
				result.Add(polygons[polygonIndices[i]]);
			return result;
		}

		private float GetSphereSurface(Parameters parameters)
		{
			return 4 * Mathf.PI * parameters.Planet.AtmosphereRadius;
		}

		private ContinentalPlateSegment[] CreateContinentalPlateSegments(
			Vector3[] plateSegmentSites, 
			VoronoiDiagram voronoi)
		{
			ContinentalPlateSegment[] result = new ContinentalPlateSegment[plateSegmentSites.Length];
			for (int i = 0; i < plateSegmentSites.Length; i++)
				result[i] = new ContinentalPlateSegment(voronoi.Regions[i]);
			return result;
		}

		private Vector3[] CreateSites(Parameters parameters, int sitesAmount)
		{
			float atmosphereRadius = parameters.Planet.AtmosphereRadius;
			int siteSamples = (int)(sitesAmount * parameters.SampleEliminationFactor);
			Vector3[] points = _randomPointsGenerator.Generate(siteSamples, atmosphereRadius);
			float sphereSurface = GetSphereSurface(parameters);
			return _sampleElimination.Eliminate(points, sitesAmount, sphereSurface);
		}


		private static List<int>[] FindNearest(Vector3[] samples, KDTree<Vector3> regionSitesTree)
		{
			List<int>[] result = new List<int>[regionSitesTree.Count];
			for (int i = 0; i < result.Length; i++)
				result[i] = new List<int>();
			for (int i = 0; i < samples.Length; i++)
				AddNearstTo(result, samples, regionSitesTree, i);
			return result;
		}

		private static void AddNearstTo(List<int>[] result, Vector3[] samples, KDTree<Vector3> regionSitesTree, int index)
		{
			int siteIndex = regionSitesTree.GetNearestTo(samples[index]);
			result[siteIndex].Add(index);
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