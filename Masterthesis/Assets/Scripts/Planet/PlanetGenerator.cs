
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

        /*[SerializeField]
        private DelaunayMesh _delaunay;
        [SerializeField]
        private VoronoiMesh _voronoi;*/

        private BasicPlanetFactory _basicPlanetFactory;
        private ContinentalPlatesFactory _continentalPlatesFactory;
        private ShapingFactory _shapingFactory;
        private NoiseFactory _noiseFactory;
        private PlanetLayerMaterializer _layerMaterializer;
		private EvaluationPointDatasInitializer _evaluationPointDatasInitializer;
		private PlanetColorizer _colorizer;
		private Noise3D _continentalPlatesWarpingNoise;
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
            PlanetColorizer colorizer)
		{
            _basicPlanetFactory = basicPlanetFactory;
            _continentalPlatesFactory = continentalPlatesFactory;
            _shapingFactory = shapingFactory;
            _noiseFactory = noiseFactory;
            _layerMaterializer = layerMaterializer;
            _evaluationPointDatasInitializer = evaluationPointDatasInitializer;
            _colorizer = colorizer;

            _biomes = biomeFactory.Create(_biomeSettings);
        }

        public void Generate(Parameter parameter)
		{
			CleanPlanet();
			Init(parameter);
			CreatePlanet();
			_planet.Data.ContinentalPlates = CreateContinentalPlates();
			//UpdateDebugView();
			_evaluationPointDatasInitializer.Compute(CreateEvaluationPointDatasInitializerParameter());
			Materialize(parameter);
			_planet.UpdateMesh();
			SetVertexColors();
		}

		private void Materialize(Parameter parameter)
		{
			PlanetLayerMaterializer.Parameter materializerParameter = CreateMaterializerParameter(CreateShapingLayers(parameter.Shaping));
			_layerMaterializer.UpdateRockElevation(materializerParameter);
			_planet.UpdateMesh();
			_layerMaterializer.UpdateTopSolidElevation(materializerParameter);
			_layerMaterializer.UpdateLiquidElevation(materializerParameter);
			_layerMaterializer.UpdateAirElevation(materializerParameter);
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
			_noiseFactory.ClearCache();
			_continentalPlatesWarpingNoise = _noiseFactory.Create(_continentalPlatesWarpingNoiseSettings, parameter.Seed);
			_mountainsNoise = _noiseFactory.Create(_mountainsNoiseSettings, parameter.Seed);
			_canyonsNoise = _noiseFactory.Create(_canyonsNoiseSettings, parameter.Seed);
			_oceansNoise = _noiseFactory.Create(_oceansNoiseSettings, parameter.Seed);
			_continentNoise = _noiseFactory.Create(_continentNoiseSettings, parameter.Seed);
			_baseNoise = _noiseFactory.Create(_baseNoiseSettings, parameter.Seed);
            _layerMaterialGradientNoise = _noiseFactory.Create(_layerMaterialGradientNoiseSettings, parameter.Seed);
        }

        private void CreatePlanet()
        {
            _planet = _basicPlanetFactory.Create(CreateBasicPlanetFactoryParameter());
        }

        private BasicPlanetFactory.Parameter CreateBasicPlanetFactoryParameter()
		{
            PlanetDimensions dimensions = _parameter.Dimensions;
            TemperatureSpectrum temperature = _parameter.TemperatureSpectrum;
            PlanetAxisData axis = _parameter.AxisData;
            return new BasicPlanetFactory.Parameter(
                dimensions, 
                temperature, 
                axis, 
                _parameter.Subdivisions, 
                _parameter.Seed);
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
                _continentalPlatesWarpingNoise, 
                _parameter.ContinentalPlatesParameter.WarpFactor);
        }

         /*private void UpdateDebugView()
        {
            ContinentalPlates plates = _planet.Data.ContinentalPlates;
            _delaunay.UpdateView(plates.SegmentSites, plates.SegmentsDelaunayTriangles);
            _voronoi.UpdateView(plates.SegmentsVoronoi);
        }*/

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
                _parameter.ContinentalPlatesParameter, 
                _biomeSettings);
		}

        private PlanetLayerMaterializer.Parameter CreateMaterializerParameter(ShapingLayer[] shapingLayers)
        {
            return new PlanetLayerMaterializer.Parameter(
                _planet, 
                _biomes, 
                shapingLayers,
                _parameter.ContinentalPlatesParameter.BlendFactor);
        }

        private void SetVertexColors()
        {
            _colorizer.Compute(new PlanetColorizer.Parameter(
                _planet,
                _layerMaterialGradientNoise
            ));
        }


        public class Parameter
		{
            public Parameter(Seed seed,
                int subdivisions,
                PlanetDimensions dimensions,
                PlanetAxisData axisData,
                ContinentalPlatesParameter continentalPlatesParameter,
                TemperatureSpectrum temperatureSpectrum,
                ShapingParameter shaping)
			{
				Seed = seed;
				Subdivisions = subdivisions;
				Dimensions = dimensions;
				AxisData = axisData;
				ContinentalPlatesParameter = continentalPlatesParameter;
				TemperatureSpectrum = temperatureSpectrum;
				Shaping = shaping;
			}

			public Seed Seed { get; }
			public int Subdivisions { get; }
			public PlanetDimensions Dimensions { get; }
			public PlanetAxisData AxisData { get; }
			public ContinentalPlatesParameter ContinentalPlatesParameter { get; }
			public TemperatureSpectrum TemperatureSpectrum { get; }
			public ShapingParameter Shaping { get; }
		}
    }
}