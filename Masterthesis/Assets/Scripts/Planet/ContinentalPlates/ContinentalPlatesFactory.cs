using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SBaier.Master
{
    public class ContinentalPlatesFactory
    {
		private const float _minPlatesForce = 0.5f;
		private const float _minStartSamplesFactor = 1.0f;
		private const float _startSamplesFactorVariance = 2.0f;
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
			int segmentsAmount = parameters.PlanetRegionsParameter.SegmentsAmount;
			Vector3[] plateSegmentSites = CreateSites(parameters, segmentsAmount);
			Triangle[] segmentsTriangles = CreatePlateSegmentTriangles(plateSegmentSites);
			VoronoiDiagram segmentsVoronoi = CreatePlateSegmentVoronoiDiagram(plateSegmentSites, segmentsTriangles, parameters.Planet.AtmosphereRadius);
			ContinentalPlateSegment[] segments = CreateContinentalPlateSegments(plateSegmentSites, segmentsVoronoi);
			ContinentalRegion[] regions = CreateRegions(parameters, plateSegmentSites, segments, segmentsVoronoi);
			ContinentalPlate[] plates = CreatePlates(parameters, regions);
			ConnectingBordersFinder connectingBordersFinder = new ConnectingBordersFinder(plates);
			connectingBordersFinder.Calculate();
			Vector2Int[] neighbors = connectingBordersFinder.Neighbors;
			Dictionary<Vector2Int, Vector2Int[]> plateBorders = connectingBordersFinder.NeighborsToBorders;
			ContinentalPlates result = new ContinentalPlates(segments, regions, plates, plateBorders, segmentsVoronoi, segmentsTriangles, neighbors);
			return result;
		}

		private ContinentalRegion[] CreateRegions(Parameters parameters, Vector3[] plateSegmentSites, ContinentalPlateSegment[] segments, VoronoiDiagram segmentsVoronoi)
		{
			int regionSitesAmount = parameters.PlanetRegionsParameter.OceansAmount + parameters.PlanetRegionsParameter.ContinentsAmount;
			ContinentalRegion[] result = new ContinentalRegion[regionSitesAmount];
			Vector3[] regionSites = CreateSites(parameters, regionSitesAmount);
			//KDTree<Vector3> regionSitesTree = _kdTreeFactory.Create(regionSites);
			KDTree<Vector3> segmentSitesTree = _kdTreeFactory.Create(plateSegmentSites);
			List<int>[] regionToSegments = ChooseSegments(parameters, regionSites, segments, segmentSitesTree, segmentsVoronoi);
			for (int i = 0; i < result.Length; i++)
				result[i] = CreateRegion(parameters, regionToSegments, segments, regionSites, i);
			return result;
		}

		private List<int>[] ChooseSegments(Parameters parameters, Vector3[] regionSites, ContinentalPlateSegment[] segments, KDTree<Vector3> segmentSitesTree, VoronoiDiagram segmentsVoronoi)
		{
			List<int>[] result = new List<int>[regionSites.Length];
			for (int i = 0; i < result.Length; i++)
				result[i] = new List<int>();

			int[][] possibleNext = new int[regionSites.Length][];
			HashSet<int> chosenSites = new HashSet<int>();
			int[] startSegments = segmentSitesTree.GetNearestTo(regionSites);
			List<int> openRegions = new List<int>();
			for (int i = 0; i < startSegments.Length; i++)
			{
				possibleNext[i] = new int[] { startSegments[i] };
				openRegions.Add(i);
			}
				

			while (openRegions.Count > 0)
			{
				for (int i = 0; i < openRegions.Count; i++)
				{
					int region = openRegions[i];
					int[] neighbors = possibleNext[region];
					int[] indices = CreateRandomIndices(neighbors.Length, parameters.Seed);
					bool foundNext = false;

					for (int j = 0; j < indices.Length; j++)
					{
						int neighbor = neighbors[indices[j]];
						if (chosenSites.Contains(neighbor))
							continue;
						result[region].Add(neighbor);
						chosenSites.Add(neighbor);
						possibleNext[region] = GetNeighbors(result[region], segmentsVoronoi);
						foundNext = true;
						break;
					}

					if(!foundNext)
						openRegions.Remove(region);
				}
			}
			return result;
		}

		private int[] GetNeighbors(List<int> segementIndices, VoronoiDiagram diagram)
		{
			HashSet<int> neighbors = new HashSet<int>();
			for (int i = 0; i < segementIndices.Count; i++)
			{
				VoronoiRegion region = diagram.Regions[segementIndices[i]];
				for (int j = 0; j < region.NeighborIndices.Length; j++)
					neighbors.Add(region.NeighborIndices[j]);
			}
			return neighbors.ToArray();
		}

		private int[] CreateRandomIndices(int count, Seed seed)
		{
			int[] result = new int[count];
			for (int i = 0; i < count; i++)
				result[i] = i;
			return result.OrderBy(x => seed.Random.Next()).ToArray();
		}

		private ContinentalRegion CreateRegion(Parameters parameters, List<int>[] regionToSegments,
			ContinentalPlateSegment[] segments, Vector3[] regionSites, int index)
		{

			List<Polygon> polygons = ExtractPolygons(regionToSegments[index], segments);
			Vector2Int[] borders = GetBorders(polygons.ToArray());
			ContinentalRegion.Type type = index < parameters.PlanetRegionsParameter.OceansAmount ? ContinentalRegion.Type.Oceanic : ContinentalRegion.Type.ContinentalPlate;
			List<int> segmentIndices = regionToSegments[index];

			for (int i = 0; i < segmentIndices.Count; i++)
				SetBiome(parameters, segments, type, segmentIndices, i);

			return new ContinentalRegion(type, regionToSegments[index].ToArray(), regionSites[index], borders);
		}

		private void SetBiome(Parameters parameters, ContinentalPlateSegment[] segments, ContinentalRegion.Type type, List<int> segmentIndices, int i)
		{
			ContinentalPlateSegment segment = segments[segmentIndices[i]];
			int biomeIndex = ChooseBiome(parameters, segment, type);
			segment.BiomeID = biomeIndex;
		}

		private int ChooseBiome(Parameters parameters, ContinentalPlateSegment segment, ContinentalRegion.Type type)
		{
			float temperature = CalculateTemperature(parameters, segment);

			List<int> possibleBiomes = new List<int>();
			int sum = 0;
			for (int i = 0; i < parameters.Biomes.Length; i++)
			{
				if (!IsBiomePossible(parameters.Biomes[i], type, temperature))
					continue;
				possibleBiomes.Add(i);
				sum += parameters.Biomes[i].Frequency;
			}

			int randomNum = parameters.Seed.Random.Next(0, sum + 1);
			int frequencyArea = 0;
			for (int j = 0; j < possibleBiomes.Count; j++)
			{
				int biomeID = possibleBiomes[j];
				BiomeSettings biome = parameters.Biomes[biomeID];
				frequencyArea += biome.Frequency;
				if (frequencyArea >= randomNum)
					return biomeID;
			}
			throw new NoFittingBiomeFoundException();
		}

		private bool IsBiomePossible(BiomeSettings biome, ContinentalRegion.Type type, float temperature)
		{
			bool isRightRegion = biome.RegionType == type;
			float minTemp = biome.TemperatureSpecturm.x;
			float maxTemp = biome.TemperatureSpecturm.y;
			bool isInsideTempSpectrum = temperature >= minTemp && temperature <= maxTemp;
			bool hasRightTemperature = !biome.UseTemeratureSpectrum || isInsideTempSpectrum;
			return isRightRegion && hasRightTemperature;
		}

		private float CalculateTemperature(Parameters parameters, ContinentalPlateSegment segment)
		{
			Vector3 site = segment.Site;
			TemperatureSpectrum spectrum = parameters.Planet.Data.TemperatureSpectrum;
			float angle = Vector3.Angle(site, Vector3.up);
			float t = 1 - Mathf.Abs((angle - 90) / 90);
			return Mathf.Lerp(spectrum.Minimal, spectrum.Maximal, t);
		}

		private VoronoiDiagram CreatePlateSegmentVoronoiDiagram(Vector3[] plateSegmentSites, Triangle[] triangles, float radius)
		{
			VoronoiDiagram diagram = new SphericalVoronoiCalculator(triangles, plateSegmentSites, radius).CalculateVoronoiDiagram();
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
			Vector3[] plateSites = CreateSites(parameters, parameters.PlanetRegionsParameter.PlatesAmount);
			KDTree<Vector3> platesKDTree = _kdTreeFactory.Create(plateSites);
			List<int>[] plateToRegion = FindNearest(regionSites, platesKDTree);
			ContinentalPlate[] result = new ContinentalPlate[parameters.PlanetRegionsParameter.PlatesAmount];
			Seed seed = new Seed(parameters.Seed.Random.Next());
			for (int i = 0; i < plateToRegion.Length; i++)
				result[i] = CreatePlate(parameters, regions, plateSites, plateToRegion, seed, i);
			return result;
		}

		private ContinentalPlate CreatePlate(Parameters parameters, ContinentalRegion[] regions, Vector3[] plateSites, List<int>[] plateToRegion, Seed seed, int index)
		{
			Vector3 movementTangent = CalculateMovementTangent(plateSites[index], seed);
			float movementStrength = CalculateMovementStrength(seed, parameters.PlanetRegionsParameter.PlatesMinForce);
			List<Polygon> polygons = ExtractPolygons(plateToRegion[index], regions);
			Vector2Int[] borders = GetBorders(polygons.ToArray());
			return new ContinentalPlate(plateSites[index], plateToRegion[index].ToArray(), movementTangent, movementStrength, borders);
		}

		private float CalculateMovementStrength(Seed seed, float minPlatesForce)
		{
			float value = (float)seed.Random.NextDouble();
			float delta = value * (1 - minPlatesForce);
			return minPlatesForce + delta;
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

		private static Vector2Int[] GetBorders(Polygon[] polygons)
		{
			EdgesFinder finder = new UnsharedEdgesFinder(polygons);
			Vector2Int[] borders = finder.Find();
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
			float startSamplesFactor = _minStartSamplesFactor + parameters.PlanetRegionsParameter.SampleEliminationFactor * _startSamplesFactorVariance;
			int siteSamples = (int)(sitesAmount * startSamplesFactor);
			Vector3[] points = _randomPointsGenerator.Generate(siteSamples, atmosphereRadius, parameters.Seed);
			float sphereSurface = GetSphereSurface(parameters);
			return _sampleElimination.Eliminate(points, sitesAmount, sphereSurface);
		}


		private List<int>[] FindNearest(Vector3[] samples, KDTree<Vector3> tree)
		{
			List<int>[] result = new List<int>[tree.Count];
			for (int i = 0; i < result.Length; i++)
				result[i] = new List<int>();
			int[] nearestResult = tree.GetNearestTo(samples);
			for (int i = 0; i < nearestResult.Length; i++)
				result[nearestResult[i]].Add(i);
			return result;
		}

		public class Parameters
		{
			public Parameters(
				Planet planet, 
				Seed seed, 
				PlanetRegionsParameter planetRegionsParameter,
				BiomeSettings[] biomes)
			{
				Planet = planet;
				Seed = seed;
				PlanetRegionsParameter = planetRegionsParameter;
				Biomes = biomes;
			}

			public Planet Planet { get; }
			public Seed Seed { get; }
			public PlanetRegionsParameter PlanetRegionsParameter { get; }
			public BiomeSettings[] Biomes { get; }
		}

		public class NoFittingBiomeFoundException : ArgumentException { }
	}
}