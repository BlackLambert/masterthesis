using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using Zenject;

namespace SBaier.Master
{
    public class PlanetGenerator : MonoBehaviour
    {
        [Header("Temperature Spectrum")]
        [SerializeField]
        private float _teperatureMin = -20f;
        [SerializeField]
        private float _teperatureMax = 50f;

        [Header("Planet Body")]
        [SerializeField]
        private IcosahedronGeneratorSettings _meshGeneratorSettings;

        [Header("Biome")]
        [SerializeField]
        private BiomeSettings[] _biomes;

        [Header("Noise")]
        [SerializeField]
        private NoiseSettings _continentalPlatesNoiseSettings;
        [SerializeField]
        private NoiseSettings _continentsNoiseSettings;
        [SerializeField]
        private NoiseSettings _continentalPlatesWarpingNoiseSettings;
        [SerializeField]
        private NoiseSettings _mountainsNoiseSettings;
        [SerializeField]
        private NoiseSettings _canyonsNoiseSettings;

        [SerializeField]
        private MeshFilter _delaunay;
        [SerializeField]
        private MeshFilter _voronoi;

        BasicPlanetFactory _basicPlanetFactory;
        private Vector3BinaryKDTreeFactory _treeFactory;
        private ContinentalPlatesFactory _continentalPlatesFactory;
        private ShapingFactory _shapingFactory;
        private NoiseFactory _noiseFactory;


        private Noise3D _continentalPlatesWarpingNoise;
        private Noise3D _mountainsNoise;
        private Noise3D _canyonsNoise;

        private Planet _planet;

        [Inject]
        public void Construct(
            BasicPlanetFactory basicPlanetFactory,
            NoiseFactory noiseFactory,
            ContinentalPlatesFactory continentalPlatesFactory,
            ShapingFactory shapingFactory,
            Vector3BinaryKDTreeFactory treeFactory)
		{
            _basicPlanetFactory = basicPlanetFactory;
            _continentalPlatesFactory = continentalPlatesFactory;
            _shapingFactory = shapingFactory;
            _treeFactory = treeFactory;
            _noiseFactory = noiseFactory;
        }

        public void Generate(Parameter parameter)
		{
			if (_planet != null)
			{
				Destroy(_planet.gameObject);
				_planet = null;
			}

			Init(parameter);

			Planet planet = _basicPlanetFactory.Create(CreateBasicPlanetFactoryParameter(parameter));
            ContinentalPlates plates = CreateContinentalPlates(planet, parameter);
            planet.Data.ContinentalPlates = plates;
			ShapingLayer[] shapingLayers = CreateShapingLayers(planet.Data);

            InitSegmentsDelaunayMesh(plates);
			InitSegmentsVoronoiView(plates);
            Biome[] biomes = CreateBiomes();
			UpdateEvaluationPointData(planet, parameter);
            SetVertexColors(planet, parameter.ContinentalPlatesParameter, biomes);
            UpdateElevation(planet, biomes, shapingLayers, parameter);
            //InitContinentLayer(planet);
            planet.UpdateMesh();
			_planet = planet;
        }

		private void Init(Parameter parameter)
		{
            _noiseFactory.ClearCache();
            _continentalPlatesWarpingNoise = _noiseFactory.Create(_continentalPlatesWarpingNoiseSettings, parameter.Seed);
			_mountainsNoise = _noiseFactory.Create(_mountainsNoiseSettings, parameter.Seed);
			_canyonsNoise = _noiseFactory.Create(_canyonsNoiseSettings, parameter.Seed);
		}

		private BasicPlanetFactory.Parameter CreateBasicPlanetFactoryParameter(Parameter parameter)
		{
            PlanetDimensions dimensions = parameter.Dimensions;
            TemperatureSpectrum temperature = new TemperatureSpectrum(_teperatureMin, _teperatureMax);
            PlanetAxisData axis = parameter.AxisData;
            return new BasicPlanetFactory.Parameter(dimensions, temperature, axis, parameter.Subdivisions, parameter.Seed);
		}

		private ContinentalPlates CreateContinentalPlates(Planet planet, Parameter parameter)
		{
			ContinentalPlatesFactory.Parameters continentalPlatesFactoryParameter = CreatePlatesFactoryParameters(planet, parameter);
			ContinentalPlates plates = _continentalPlatesFactory.Create(continentalPlatesFactoryParameter);
			return plates;
		}

		private ShapingLayer[] CreateShapingLayers(PlanetData data)
		{
			ShapingFactory.Parameter shapingFactoryParameter = new ShapingFactory.Parameter(data, _mountainsNoise, _canyonsNoise);
			ShapingLayer[] shapingLayers = _shapingFactory.Create(shapingFactoryParameter);
			return shapingLayers;
		}

		private ContinentalPlatesFactory.Parameters CreatePlatesFactoryParameters(Planet planet, Parameter parameter)
		{
			return new ContinentalPlatesFactory.Parameters(
				planet,
                parameter.Seed,
                parameter.ContinentalPlatesParameter,
				_biomes);
		}

		private Biome[] CreateBiomes()
		{
            Biome[] result = new Biome[_biomes.Length];
			for (int i = 0; i < _biomes.Length; i++)
			{
                BiomeSettings settings = _biomes[i];
                result[i] = new Biome(settings.BaseColor, settings.RegionType);
			}
            return result;
		}

		private void InitSegmentsVoronoiView(ContinentalPlates plates)
		{
            _voronoi.sharedMesh = new Mesh();
            _voronoi.sharedMesh.vertices = plates.SegmentsVoronoi.Vertices;
            Draw(plates.SegmentsVoronoi);
        }

        private void Draw(VoronoiDiagram diagram)
        {
            foreach (VoronoiRegion region in diagram.Regions)
            {
                for (int i = 0; i < region.VertexIndices.Count; i++)
                {
                    Vector3 c0 = diagram.Vertices[region.VertexIndices[i]];
                    Vector3 c1 = diagram.Vertices[region.VertexIndices[(i + 1) % region.VertexIndices.Count]];
                    Debug.DrawLine(c0, c1, Color.green, 60);
                }
            }
        }

        private void InitSegmentsDelaunayMesh(ContinentalPlates plates)
        {
            _delaunay.sharedMesh = new Mesh();
            _delaunay.sharedMesh.vertices = plates.SegmentSites;
            _delaunay.sharedMesh.triangles = CreateTriangles(plates.SegmentsDelaunayTriangles);
            _delaunay.sharedMesh.RecalculateNormals();
        }

        private int[] CreateTriangles(Triangle[] triangles)
        {
            int[] result = new int[triangles.Length * 3];
            for (int i = 0; i < triangles.Length; i++)
            {
                result[i * 3] = triangles[i].VertexIndices[0];
                result[i * 3 + 1] = triangles[i].VertexIndices[1];
                result[i * 3 + 2] = triangles[i].VertexIndices[2];
            }

            return result;
        }

        private void UpdateEvaluationPointData(Planet planet, Parameter parameter)
		{
            ContinentalPlates plates = planet.Data.ContinentalPlates;
            KDTree<Vector3> segmentsKDTree = _treeFactory.Create(plates.SegmentSites);
            for (int i = 0; i < planet.Faces.Count; i++)
			{
				PlanetFace face = planet.Faces[i];
				PlanetFaceData faceData = face.Data;
                PlanetPointsWarper warper = new PlanetPointsWarper(_continentalPlatesWarpingNoise);
				Vector3[] vertices = warper.Warp(face.Vertices, parameter.ContinentalPlatesParameter.WarpFactor, parameter.Dimensions.AtmosphereRadius);

				for (int j = 0; j < vertices.Length; j++)
				{
					Vector3 vertex = vertices[j];
					EvaluationPointData pointData = faceData.EvaluationPoints[j];
					int segmentIndex = segmentsKDTree.GetNearestTo(vertex);
					ContinentalPlateSegment segment = plates.Segments[segmentIndex];
					pointData.ContinentalPlateSegmentIndex = segmentIndex;
					pointData.BiomeID = segment.BiomeID;
                    pointData.WarpedPoint = vertex;
                }
			}
		}

        private void SetVertexColors(Planet planet, ContinentalPlatesParameter continentalPlatesParameter, Biome[] biomes)
		{
            ContinentalPlates plates = planet.Data.ContinentalPlates;
            for (int i = 0; i < planet.Faces.Count; i++)
            {
                PlanetFace face = planet.Faces[i];
                PlanetFaceData faceData = face.Data;
                Color[] vertexColors = new Color[face.MeshFilter.sharedMesh.vertexCount];
                float length = faceData.EvaluationPoints.Length;

                for (int j = 0; j < length; j++)
                {
                    EvaluationPointData pointData = faceData.EvaluationPoints[j];
                    Vector3 vertex = pointData.WarpedPoint;
                    Color vertexColor = GetVertexColor(planet, continentalPlatesParameter, vertex, plates, biomes, pointData.ContinentalPlateSegmentIndex);
                    vertexColors[j] = vertexColor;
                }

                face.MeshFilter.sharedMesh.colors = vertexColors;
            }
        }

        private Color GetVertexColor(Planet planet, ContinentalPlatesParameter continentalPlatesParameter, Vector3 vertex, ContinentalPlates plates, Biome[] biomes, int segmentIndex)
        {
            ContinentalPlateSegment segment = plates.Segments[segmentIndex];
            Biome biome = biomes[segment.BiomeID];
            Color color = biome.BaseColor;
            float blendDistance = continentalPlatesParameter.BlendFactor * planet.AtmosphereRadius;
            Vector3 pointOnBorder = plates.SegmentsVoronoi.GetNearestBorderPointOf(vertex, segmentIndex);
            float distanceToInnerBorder = GetDistanceOnPlanet(planet, vertex, pointOnBorder);
            if (distanceToInnerBorder >= blendDistance)
                return color;
            for (int i = 0; i < segment.Neighbors.Length; i++)
            {
                int neighborSegmentIndex = segment.Neighbors[i];
                ContinentalPlateSegment neighborSegment = plates.Segments[neighborSegmentIndex];
                Biome neighborBiome = biomes[neighborSegment.BiomeID];
                if (neighborBiome.RegionType != biome.RegionType)
                    continue;
                Vector3 pointNeighborOnBorder = plates.SegmentsVoronoi.GetNearestBorderPointOf(vertex, neighborSegmentIndex);
                float distanceToSegment = GetDistanceOnPlanet(planet, vertex, pointNeighborOnBorder);
                if (distanceToSegment > blendDistance)
                    continue;
                float portion = 0.5f - (distanceToSegment / blendDistance) / 2;
                color = Color.Lerp(color, neighborBiome.BaseColor, portion);
            }
            return color;
        }


		private void UpdateElevation(Planet planet, Biome[] biomes, ShapingLayer[] shapingLayers, Parameter parameter)
        {
            ContinentalPlates plates = planet.Data.ContinentalPlates;
            KDTree<Vector3> segmentsKDTree = _treeFactory.Create(plates.SegmentSites);
            for (int i = 0; i < planet.Faces.Count; i++)
            {
                PlanetFace face = planet.Faces[i];
                PlanetShaper shaper = new PlanetShaper(shapingLayers, parameter.Dimensions.RelativeSeaLevel);
                float[] shapeValues = shaper.Shape(face.Data.EvaluationPoints);

                for (int j = 0; j < shapeValues.Length; j++)
				{
                    Vector3 vertex = face.Data.EvaluationPoints[j].WarpedPoint;
                    int segmentIndex = segmentsKDTree.GetNearestTo(vertex);
                    ContinentalPlateSegment segment = plates.Segments[segmentIndex];
                    Biome biome = biomes[segment.BiomeID];
					float delta = CalculateDelta(planet, shapeValues[j], biome);
					InitContinentPlate(face.Data.EvaluationPoints[j], delta, parameter);
				}
            }
        }

        private float GetDistanceOnPlanet(Planet planet, Vector3 vertex, Vector3 point)
		{
            point = point.normalized * planet.AtmosphereRadius;
            return vertex.FastSubstract(point).magnitude;
        }

		private float CalculateDelta(Planet planet, float shapeValue, Biome biome)
		{
            bool isOceanic = biome.RegionType == ContinentalRegion.Type.Oceanic;
            if (isOceanic)
				return CreateOceanDelta(planet);
			else
				return CreateLandDelta(planet, shapeValue);
		}

		private float CreateOceanDelta(Planet planet)
		{
			return planet.Data.Dimensions.RelativeSeaLevel * planet.Data.Dimensions.MaxHullThickness;
		}

		private float CreateLandDelta(Planet planet, float shapeValue)
		{
            float transformedShape = shapeValue - 0.5f;
            float seaLevel = planet.Data.Dimensions.RelativeSeaLevel;
            float transformdedSeaLevel = (seaLevel - 0.5f) * 2;
            float posFactor = 1 - transformdedSeaLevel;
            float negFactor = 1 + transformdedSeaLevel;
            float factor;
            if (transformedShape > 0)
                factor = posFactor * transformedShape;
            else if (transformedShape < 0)
                factor = negFactor * transformedShape;
            else
                factor = 0;
            factor += seaLevel;
            return factor * planet.Data.Dimensions.MaxHullThickness;
		}

        private void InitContinentPlate(EvaluationPointData data, float value, Parameter parameter)
        {
            float continentalPlateHeight = value * parameter.Dimensions.MaxHullThickness;
            data.Layers[0].Height = data.Layers[0].Height - continentalPlateHeight;
            data.Layers.Insert(0, new PlanetLayerData(1, continentalPlateHeight));
        }
        

        public class Parameter
		{
            public Parameter(Seed seed,
                int subdivisions,
                PlanetDimensions dimensions,
                PlanetAxisData axisData,
                ContinentalPlatesParameter continentalPlatesParameter)
			{
				Seed = seed;
				Subdivisions = subdivisions;
				Dimensions = dimensions;
				AxisData = axisData;
				ContinentalPlatesParameter = continentalPlatesParameter;
			}

			public Seed Seed { get; }
			public int Subdivisions { get; }
			public PlanetDimensions Dimensions { get; }
			public PlanetAxisData AxisData { get; }
			public ContinentalPlatesParameter ContinentalPlatesParameter { get; }
		}
    }
}