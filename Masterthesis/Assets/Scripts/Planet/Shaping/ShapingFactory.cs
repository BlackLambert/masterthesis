using System.Collections.Generic;

namespace SBaier.Master
{
    public class ShapingFactory
    {
		private const ShapingLayer.Mode _baseShapingMode = ShapingLayer.Mode.Max;
		private BaseShapingLayerFactory _baseLayerFactory;
        private SegmentShapingLayerFactory _segmentLayerFactory;
        private PlatesShapingLayerFactory _plateLayerFactory;

        public ShapingFactory(
            BaseShapingLayerFactory baseLayerFactory,
            SegmentShapingLayerFactory segmentLayerFactory,
            PlatesShapingLayerFactory plateLayerFactory)
        {
            _baseLayerFactory = baseLayerFactory;
            _segmentLayerFactory = segmentLayerFactory;
            _plateLayerFactory = plateLayerFactory;
        }


        public ShapingLayer[] Create(Parameter parameter)
        {
            List<ShapingLayer> result = new List<ShapingLayer>();
            ShapingLayer baseLayer = _baseLayerFactory.Create(parameter.SeaLevelNoise, _baseShapingMode);
            ShapingLayer[] segmentShapings = _segmentLayerFactory.Create(
                new SegmentShapingLayerFactory.Parameter(parameter.Data, parameter.OceanNoise, parameter.ContinentNoise));
            ShapingLayer oceansShaping = segmentShapings[0];
            ShapingLayer continentsShaping = segmentShapings[1];
            ShapingLayer[] continentsBorderShaping = _plateLayerFactory.Create(
                new PlatesShapingLayerFactory.Parameter(parameter.Data, parameter.Shaping.Plates, parameter.MountainNoise, parameter.CanyonsNoise));
            ShapingLayer mountainsShaping = continentsBorderShaping[0];
            ShapingLayer canyonsShaping = continentsBorderShaping[1];
            result.Add(baseLayer);
            result.Add(continentsShaping);
            result.Add(oceansShaping);
            result.Add(mountainsShaping);
            result.Add(canyonsShaping);
            return result.ToArray();
        }

        public class Parameter
		{
            public Parameter(
                PlanetData data,
                ShapingParameter shaping,
                Noise3D mountainNoise,
                Noise3D canyonsNoise,
                Noise3D oceanNoise,
                Noise3D continentNoise,
                Noise3D seaLevelNoise)
			{
				Data = data;
                Shaping = shaping;
                MountainNoise = mountainNoise;
				CanyonsNoise = canyonsNoise;
				OceanNoise = oceanNoise;
                ContinentNoise = continentNoise;
				SeaLevelNoise = seaLevelNoise;
			}

			public PlanetData Data { get; }
            public ShapingParameter Shaping { get; }
			public Noise3D MountainNoise { get; }
			public Noise3D CanyonsNoise { get; }
			public Noise3D OceanNoise { get; }
            public Noise3D ContinentNoise { get; }
			public Noise3D SeaLevelNoise { get; }
		}
    }
}