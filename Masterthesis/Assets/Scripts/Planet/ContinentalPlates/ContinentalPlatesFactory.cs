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
			int segmentsAmount = parameters.PlateSegments;
			Vector3[] plateSegmentSites = CreateSites(parameters, segmentsAmount);
			Triangle[] segmentsTriangles = CreatePlateSegmentTriangles(plateSegmentSites);
			VoronoiDiagram segmentsVoronoi = CreatePlateSegmentVoronoiDiagram(plateSegmentSites, segmentsTriangles);
			int[] biomePerSegment = CreateBiomePerSegment(parameters);
			ContinentalPlateSegment[] segments = CreateContinentalPlateSegments(plateSegmentSites, segmentsVoronoi, biomePerSegment);
			ContinentalRegion[] regions = CreateRegions(parameters, plateSegmentSites, segments);
			ContinentalPlate[] plates = CreatePlates(parameters, regions);
			Dictionary<Vector2Int, Vector2Int[]> plateBorders = FindConnectingBorders(plates);
			Vector2Int[] neighbors = FindNeighbors(plateBorders);
			ContinentalPlates result = new ContinentalPlates(segments, regions, plates, plateBorders, segmentsVoronoi, segmentsTriangles, neighbors);
			return result;
		}

		private Vector2Int[] FindNeighbors(Dictionary<Vector2Int, Vector2Int[]> plateBorders)
		{
			List<Vector2Int> result = new List<Vector2Int>();
			foreach (KeyValuePair<Vector2Int, Vector2Int[]> pair in plateBorders)
			{
				if (pair.Value.Length == 0)
					continue;
				result.Add(pair.Key);
			}
			return result.ToArray();
		}

		private ContinentalRegion[] CreateRegions(Parameters parameters, Vector3[] plateSegmentSites, ContinentalPlateSegment[] segments)
		{
			int regionSitesAmount = parameters.Oceans + parameters.Continents;
			ContinentalRegion[] result = new ContinentalRegion[regionSitesAmount];
			Vector3[] regionSites = CreateSites(parameters, regionSitesAmount);
			KDTree<Vector3> regionSitesTree = new Vector3BinaryKDTree(regionSites, _quickSelector);
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
			return new ContinentalRegion(type, regionToSegments[index], regionSites[index], borders);
		}

		private int[] CreateBiomePerSegment(Parameters parameters)
		{
			int frequencySum = parameters.Biomes.Sum(b => b.Frequency) + 1;
			int[] result = new int[parameters.PlateSegments];
			for (int i = 0; i < result.Length; i++)
				result[i] = SelectBiome(parameters, frequencySum);
			return result;
		}

		private int SelectBiome(Parameters parameters, int frequencySum)
		{
			int randomNum = parameters.Seed.Random.Next(0, frequencySum);
			int frequencyArea = 0;
			int count = parameters.Biomes.Length;
			for (int j = 0; j < count; j++)
			{
				BiomeSettings biome = parameters.Biomes[j];
				frequencyArea += biome.Frequency;
				if (frequencyArea >= randomNum)
					return j;
			}
			throw new InvalidOperationException();
		}

		private Dictionary<Vector2Int, Vector2Int[]> FindConnectingBorders(ContinentalPlate[] plates)
		{
			List<Polygon> polygons = new List<Polygon>(plates);
			return new ConnectingBordersFinder(polygons).Find();
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
			KDTree<Vector3> platesKDTree = new Vector3BinaryKDTree(plateSites, _quickSelector);
			List<int>[] plateToRegion = FindNearest(regionSites, platesKDTree);
			ContinentalPlate[] result = new ContinentalPlate[parameters.Plates];
			Seed seed = new Seed(parameters.Seed.Random.Next());
			for (int i = 0; i < plateToRegion.Length; i++)
				result[i] = CreatePlate(regions, plateSites, plateToRegion, seed, i);
			return result;
		}

		private ContinentalPlate CreatePlate(ContinentalRegion[] regions, Vector3[] plateSites, List<int>[] plateToRegion, Seed seed, int index)
		{
			float movementAngle = (float)seed.Random.NextDouble() * 360;
			float movementStrength = (float)seed.Random.NextDouble();

			List<Polygon> polygons = ExtractPolygons(plateToRegion[index], regions);
			Vector2Int[] borders = GetBorders(polygons);
			return new ContinentalPlate(plateSites[index], plateToRegion[index].ToArray(), movementAngle, movementStrength, borders);
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
			VoronoiDiagram voronoi, 
			int[] biomePerSegment)
		{
			ContinentalPlateSegment[] result = new ContinentalPlateSegment[plateSegmentSites.Length];
			for (int i = 0; i < plateSegmentSites.Length; i++)
				result[i] = new ContinentalPlateSegment(voronoi.Regions[i], biomePerSegment[i]);
			return result;
		}

		private Vector3[] CreateSites(Parameters parameters, int sitesAmount)
		{
			float atmosphereRadius = parameters.Planet.AtmosphereRadius;
			int siteSamples = (int)(sitesAmount * parameters.SampleEliminationFactor);
			Vector3[] points = _randomPointsGenerator.Generate(siteSamples, atmosphereRadius);
			float sphereSurface = GetSphereSurface(parameters);
			return _sampleElimination.Eliminate(points, sitesAmount, sphereSurface).ToArray();
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