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
        [Header("Planet Body")]
        [SerializeField]
        private IcosahedronGeneratorSettings _meshGeneratorSettings;

        [Header("Biome")]
        [SerializeField]
        private BiomeSettings[] _biomeSettings;

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
        private NoiseSettings _oceansNoiseSettings;
        [SerializeField]
        private NoiseSettings _continentNoiseSettings;
        [SerializeField]
        private NoiseSettings _baseNoiseSettings;

        [SerializeField]
        private MeshFilter _delaunay;
        [SerializeField]
        private MeshFilter _voronoi;
        [SerializeField]
        private Transform _atmosphere;

        private BasicPlanetFactory _basicPlanetFactory;
        private Vector3BinaryKDTreeFactory _treeFactory;
        private ContinentalPlatesFactory _continentalPlatesFactory;
        private ShapingFactory _shapingFactory;
        private NoiseFactory _noiseFactory;


        private Noise3D _continentalPlatesWarpingNoise;
        private Noise3D _mountainsNoise;
        private Noise3D _canyonsNoise;
        private Noise3D _oceansNoise;
        private Noise3D _continentNoise;
        private Noise3D _baseNoise;

        private Planet _planet;
        private Biome[] _biomes;

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

            _biomes = CreateBiomes();
        }

        public void Generate(Parameter parameter)
		{
			CleanPlanet();
			Init(parameter);
			_planet = _basicPlanetFactory.Create(CreateBasicPlanetFactoryParameter(parameter));
			ContinentalPlates plates = CreateContinentalPlates(_planet, parameter);
			_planet.Data.ContinentalPlates = plates;
			ShapingLayer[] shapingLayers = CreateShapingLayers(_planet.Data);

			InitSegmentsDelaunayMesh(plates);
			InitSegmentsVoronoiView(plates);
            _atmosphere.localScale = Vector3.one * _planet.Data.Dimensions.AtmosphereRadius * 2;

            UpdateEvaluationPointData(_planet, parameter);
			UpdateElevation(_planet, _biomes, shapingLayers, parameter);
			_planet.UpdateMesh();
			SetVertexColors(_planet, parameter.ContinentalPlatesParameter, _biomes);
			//InitContinentLayer(planet);
		}

		private void CleanPlanet()
		{
			if (_planet != null)
			{
				Destroy(_planet.gameObject);
				_planet = null;
			}
		}

		private void SetVertexColors(Planet planet, ContinentalPlatesParameter continentalPlatesParameter, Biome[] biomes)
		{
            PlanetColorizer colorizer = new PlanetColorizer(planet, continentalPlatesParameter, biomes);
            colorizer.Compute();
        }

		private void Init(Parameter parameter)
		{
            _noiseFactory.ClearCache();
            _continentalPlatesWarpingNoise = _noiseFactory.Create(_continentalPlatesWarpingNoiseSettings, parameter.Seed);
			_mountainsNoise = _noiseFactory.Create(_mountainsNoiseSettings, parameter.Seed);
			_canyonsNoise = _noiseFactory.Create(_canyonsNoiseSettings, parameter.Seed);
            _oceansNoise = _noiseFactory.Create(_oceansNoiseSettings, parameter.Seed);
            _continentNoise = _noiseFactory.Create(_continentNoiseSettings, parameter.Seed);
            _baseNoise = _noiseFactory.Create(_baseNoiseSettings, parameter.Seed);
        }

		private BasicPlanetFactory.Parameter CreateBasicPlanetFactoryParameter(Parameter parameter)
		{
            PlanetDimensions dimensions = parameter.Dimensions;
            TemperatureSpectrum temperature = parameter.TemperatureSpectrum;
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
			ShapingFactory.Parameter shapingFactoryParameter = new ShapingFactory.Parameter(data, _mountainsNoise, _canyonsNoise, _oceansNoise, _continentNoise, _baseNoise);
			ShapingLayer[] shapingLayers = _shapingFactory.Create(shapingFactoryParameter);
			return shapingLayers;
		}

		private ContinentalPlatesFactory.Parameters CreatePlatesFactoryParameters(Planet planet, Parameter parameter)
		{
			return new ContinentalPlatesFactory.Parameters(
				planet,
                parameter.Seed,
                parameter.ContinentalPlatesParameter,
				_biomeSettings);
		}

		private Biome[] CreateBiomes()
		{
            Biome[] result = new Biome[_biomeSettings.Length];
			for (int i = 0; i < _biomeSettings.Length; i++)
                result[i] = CreateBiome(_biomeSettings[i]);
			return result;
		}

		private Biome CreateBiome(BiomeSettings settings)
		{
            if (settings is ContinentBiomeSettings)
            {
                ContinentBiomeSettings continental = settings as ContinentBiomeSettings;
                return new ContinentalBiome(continental.BaseColor, continental.RegionType, continental.MountainSlopeColor, continental.SlopeThreshold);
            }
			return new Biome(settings.BaseColor, settings.RegionType);
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
                for (int i = 0; i < region.VertexIndices.Length; i++)
                {
                    Vector3 c0 = diagram.Vertices[region.VertexIndices[i]];
                    Vector3 c1 = diagram.Vertices[region.VertexIndices[(i + 1) % region.VertexIndices.Length]];
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

		private float CalculateDelta(Planet planet, float shapeValue, Biome biome)
		{
            //bool isOceanic = biome.RegionType == ContinentalRegion.Type.Oceanic;
            //if (isOceanic)
				//return CreateOceanDelta(planet, shapeValue);
			//else
				return CreateLandDelta(planet, shapeValue);
		}

		private float CreateOceanDelta(Planet planet, float shapeValue)
		{
            if(shapeValue <= 0.5f)
                return planet.Data.Dimensions.RelativeSeaLevel * planet.Data.Dimensions.MaxHullThickness;
            return CreateLandDelta(planet, shapeValue);
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
            //float continentalPlateHeight = value * parameter.Dimensions.MaxHullThickness;
            data.Layers[0].Height = data.Layers[0].Height - value;
            data.Layers.Insert(0, new PlanetLayerData(1, value));
        }
        

        public class Parameter
		{
            public Parameter(Seed seed,
                int subdivisions,
                PlanetDimensions dimensions,
                PlanetAxisData axisData,
                ContinentalPlatesParameter continentalPlatesParameter,
                TemperatureSpectrum temperatureSpectrum)
			{
				Seed = seed;
				Subdivisions = subdivisions;
				Dimensions = dimensions;
				AxisData = axisData;
				ContinentalPlatesParameter = continentalPlatesParameter;
				TemperatureSpectrum = temperatureSpectrum;
			}

			public Seed Seed { get; }
			public int Subdivisions { get; }
			public PlanetDimensions Dimensions { get; }
			public PlanetAxisData AxisData { get; }
			public ContinentalPlatesParameter ContinentalPlatesParameter { get; }
			public TemperatureSpectrum TemperatureSpectrum { get; }
		}
    }
}