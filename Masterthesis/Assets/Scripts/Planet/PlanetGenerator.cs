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
        private DelaunayMesh _delaunay;
        [SerializeField]
        private VoronoiMesh _voronoi;

        private BasicPlanetFactory _basicPlanetFactory;
        private Vector3BinaryKDTreeFactory _treeFactory;
        private ContinentalPlatesFactory _continentalPlatesFactory;
        private ShapingFactory _shapingFactory;
        private NoiseFactory _noiseFactory;
        private PlanetLayerMaterializer _layerMaterializer;


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
            Vector3BinaryKDTreeFactory treeFactory,
            BiomeFactory biomeFactory,
            PlanetLayerMaterializer layerMaterializer)
		{
            _basicPlanetFactory = basicPlanetFactory;
            _continentalPlatesFactory = continentalPlatesFactory;
            _shapingFactory = shapingFactory;
            _treeFactory = treeFactory;
            _noiseFactory = noiseFactory;
            _layerMaterializer = layerMaterializer;

            _biomes = biomeFactory.Create(_biomeSettings);
        }

        public void Generate(Parameter parameter)
		{
			CleanPlanet();
			Init(parameter);
			_planet = _basicPlanetFactory.Create(CreateBasicPlanetFactoryParameter(parameter));
			ContinentalPlates plates = CreateContinentalPlates(_planet, parameter);
			_planet.Data.ContinentalPlates = plates;
			ShapingLayer[] shapingLayers = CreateShapingLayers(_planet.Data);

            _delaunay.UpdateView(plates.SegmentSites, plates.SegmentsDelaunayTriangles);
            _voronoi.UpdateView(plates.SegmentsVoronoi);

			UpdateEvaluationPointData(_planet, parameter);
            _layerMaterializer.UpdateElevation(new PlanetLayerMaterializer.Parameter(_planet, _biomes, shapingLayers));
			_planet.UpdateMesh();
			SetVertexColors(_planet, parameter.ContinentalPlatesParameter, _biomes);
			//InitContinentLayer(planet);
		}

		private void CleanPlanet()
		{
            if (_planet == null)
                return;
			Destroy(_planet.gameObject);
			_planet = null;
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
			return new ContinentalPlatesFactory.Parameters(planet, parameter.Seed, parameter.ContinentalPlatesParameter, _biomeSettings);
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
            PlanetColorizer colorizer = new PlanetColorizer(planet, continentalPlatesParameter, biomes);
            colorizer.Compute();
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