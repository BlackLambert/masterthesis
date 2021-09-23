
using Newtonsoft.Json;
using System;
using UnityEngine;
using Zenject;

namespace SBaier.Master
{
    public class PlanetGenerator : MonoBehaviour
    {
		private const float _minimalCameraDistance = 1f;
		private const float _maxRelativeBlendDistance = 0.25f;
		[Header("Planet Body")]
        [SerializeField]
        private IcosahedronGeneratorSettings _meshGeneratorSettings;

        [Header("Biome")]
        [SerializeField]
        private BiomeSettings[] _biomeSettings;

        [Header("Shaping Noise")]
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

        [Header("Layer Material")]
        [SerializeField]
        private NoiseSettings _layerMaterialGradientNoiseSettings;

        [SerializeField]
        private DelaunayMesh _delaunay;
        [SerializeField]
        private VoronoiMesh _voronoi;

        private BasicPlanetFactory _basicPlanetFactory;
        private ContinentalPlatesFactory _continentalPlatesFactory;
        private ShapingFactory _shapingFactory;
        private NoiseFactory _noiseFactory;
        private PlanetLayerMaterializer _layerMaterializer;
		private EvaluationPointDatasInitializer _evaluationPointDatasInitializer;
		private PlanetColorizer _colorizer;
		private CameraFocalDistanceController _focalDistanceController;
        private PlanetLayerMaterialSettings[] _materials;
        private PlanetLayerTogglePanel _layerTogglePanel;

        private Noise3D[] _warpingNoise;
        private Noise3D _mountainsNoise;
        private Noise3D _canyonsNoise;
        private Noise3D _oceansNoise;
        private Noise3D _continentNoise;
        private Noise3D _baseNoise;
        private Noise3D _layerMaterialGradientNoise;

        private Planet _planet;
        private Biome[] _biomes;
        private Parameter _parameter;

        [Inject]
        public void Construct(
            BasicPlanetFactory basicPlanetFactory,
            NoiseFactory noiseFactory,
            ContinentalPlatesFactory continentalPlatesFactory,
            ShapingFactory shapingFactory,
            BiomeFactory biomeFactory,
            PlanetLayerMaterializer layerMaterializer,
            EvaluationPointDatasInitializer evaluationPointDatasInitializer,
            PlanetColorizer colorizer,
            CameraFocalDistanceController focalDistanceController,
            PlanetLayerMaterialSettings[] materials,
            PlanetLayerTogglePanel layerTogglePanel)
		{
            _basicPlanetFactory = basicPlanetFactory;
            _continentalPlatesFactory = continentalPlatesFactory;
            _shapingFactory = shapingFactory;
            _noiseFactory = noiseFactory;
            _layerMaterializer = layerMaterializer;
            _evaluationPointDatasInitializer = evaluationPointDatasInitializer;
            _colorizer = colorizer;
            _focalDistanceController = focalDistanceController;
            _materials = materials;
            _layerTogglePanel = layerTogglePanel;

            _biomes = biomeFactory.Create(_biomeSettings);
        }

        public void Generate(Parameter parameter)
		{
			CleanPlanet();
			Init(parameter);
			CreatePlanet();
			_planet.Data.ContinentalPlates = CreateContinentalPlates();
			UpdateDebugView();
			_evaluationPointDatasInitializer.Compute(CreateEvaluationPointDatasInitializerParameter());
			Materialize(parameter);
			_planet.UpdateMesh();
			SetVertexColors();
            UpdateCamera();
            _layerTogglePanel.ResetView();
            _layerTogglePanel.Show();
        }

		private void Materialize(Parameter parameter)
		{
			PlanetLayerMaterializer.Parameter materializerParameter = CreateMaterializerParameter(CreateShapingLayers(parameter.Shaping));
			_layerMaterializer.CreateRockLayer(materializerParameter);
			_planet.UpdateMesh();
			_layerMaterializer.CreateGroundLayer(materializerParameter);
            _layerMaterializer.CreateGroundVegetationLayer(materializerParameter);
            _layerMaterializer.CreateLiquidLayer(materializerParameter);
            _layerMaterializer.CreateAirLayer(materializerParameter);
		}

		private void CleanPlanet()
		{
            if (_planet == null)
                return;
            _planet.Destruct();
			_planet = null;
        }

        private void Init(Parameter parameter)
		{
            _parameter = parameter;
			CreateNoise(parameter);
		}

		private void CreateNoise(Parameter parameter)
        {
            int warpLayersAmount = parameter.PlanetRegionsParameter.WarpLayers;
            _warpingNoise = new Noise3D[warpLayersAmount];
			for (int i = 0; i < warpLayersAmount; i++)
			{
                _noiseFactory.ClearCache();
                _warpingNoise[i] = _noiseFactory.Create(_continentalPlatesWarpingNoiseSettings, CreateSeed(parameter.Seed));
            }
			_mountainsNoise = _noiseFactory.Create(_mountainsNoiseSettings, parameter.Seed);
			_canyonsNoise = _noiseFactory.Create(_canyonsNoiseSettings, parameter.Seed);
			_oceansNoise = _noiseFactory.Create(_oceansNoiseSettings, parameter.Seed);
			_continentNoise = _noiseFactory.Create(_continentNoiseSettings, parameter.Seed);
			_baseNoise = _noiseFactory.Create(_baseNoiseSettings, parameter.Seed);
            _layerMaterialGradientNoise = _noiseFactory.Create(_layerMaterialGradientNoiseSettings, parameter.Seed);
        }

		private Seed CreateSeed(Seed seed)
		{
            return new Seed(seed.Random.Next());
		}

		private void CreatePlanet()
        {
            _planet = _basicPlanetFactory.Create(CreateBasicPlanetFactoryParameter());
        }

        private BasicPlanetFactory.Parameter CreateBasicPlanetFactoryParameter()
		{
            PlanetDimensions dimensions = _parameter.PlanetDimensions;
            TemperatureSpectrum temperature = _parameter.TemperatureSpectrum;
            PlanetAxisData axis = _parameter.AxisData;
            return new BasicPlanetFactory.Parameter(
                dimensions, 
                temperature, 
                axis, 
                _parameter.Subdivisions, 
                _parameter.Seed,
                _materials,
                _layerMaterialGradientNoise);
		}

		private ContinentalPlates CreateContinentalPlates()
		{
			ContinentalPlatesFactory.Parameters continentalPlatesFactoryParameter = CreatePlatesFactoryParameters();
			ContinentalPlates plates = _continentalPlatesFactory.Create(continentalPlatesFactoryParameter);
			return plates;
		}

        private EvaluationPointDatasInitializer.Parameter CreateEvaluationPointDatasInitializerParameter()
        {
            return new EvaluationPointDatasInitializer.Parameter(
                _planet, 
                _warpingNoise, 
                _parameter.PlanetRegionsParameter.WarpFactor,
                _biomes,
                _parameter.PlanetRegionsParameter.BlendFactor * _maxRelativeBlendDistance * _planet.Data.Dimensions.HullMaxRadius);
        }

        private void UpdateDebugView()
        {
            ContinentalPlates plates = _planet.Data.ContinentalPlates;
            _delaunay.UpdateView(plates.SegmentSites, plates.SegmentsDelaunayTriangles);
            _voronoi.UpdateView(plates.SegmentsVoronoi);
        }

        private ShapingLayer[] CreateShapingLayers(ShapingParameter shaping)
		{
            PlanetData data = _planet.Data;
			ShapingFactory.Parameter parameter = new ShapingFactory.Parameter(
                data,
                shaping,
                _mountainsNoise, 
                _canyonsNoise, 
                _oceansNoise, 
                _continentNoise, 
                _baseNoise);
			ShapingLayer[] shapingLayers = _shapingFactory.Create(parameter);
			return shapingLayers;
		}

		private ContinentalPlatesFactory.Parameters CreatePlatesFactoryParameters()
		{
			return new ContinentalPlatesFactory.Parameters(
                _planet, 
                _parameter.Seed, 
                _parameter.PlanetRegionsParameter, 
                _biomeSettings);
		}

        private PlanetLayerMaterializer.Parameter CreateMaterializerParameter(ShapingLayer[] shapingLayers)
        {
            return new PlanetLayerMaterializer.Parameter(
                _planet, 
                _biomes, 
                shapingLayers);
        }

        private void SetVertexColors()
        {
            _colorizer.Compute(new PlanetColorizer.Parameter(
                _planet
            ));
        }

        private void UpdateCamera()
        {
            _focalDistanceController.SetMinDistance(_parameter.PlanetDimensions.HullMaxRadius + _minimalCameraDistance);

		}


        [Serializable]
        public class Parameter
		{
            public Parameter(Seed seed,
                float subdivisions,
                PlanetDimensions planetDimensions,
                PlanetAxisData axisData,
                PlanetRegionsParameter continentalPlatesParameter,
                TemperatureSpectrum temperatureSpectrum,
                ShapingParameter shaping)
			{
				Seed = seed;
				Subdivisions = subdivisions;
				PlanetDimensions = planetDimensions;
                AxisData = axisData;
				PlanetRegionsParameter = continentalPlatesParameter;
				TemperatureSpectrum = temperatureSpectrum;
				Shaping = shaping;
			}

            [JsonIgnore]
			public Seed Seed { get; }
			public float Subdivisions { get; }
			public PlanetDimensions PlanetDimensions { get; }
			public PlanetAxisData AxisData { get; }
			public PlanetRegionsParameter PlanetRegionsParameter { get; }
			public TemperatureSpectrum TemperatureSpectrum { get; }
			public ShapingParameter Shaping { get; }
		}
    }
}