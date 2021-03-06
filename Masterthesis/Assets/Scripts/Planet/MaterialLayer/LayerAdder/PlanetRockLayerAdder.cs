using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SBaier.Master
{
    public class PlanetRockLayerAdder : PlanetLayerAdder
    {
		private const float _bottomLayerHeight = 0.01f;
		private Planet _planet;
		private ShapingLayer[] _shapingLayers;
		private float _relativeSeaLevel;
        private SeaLevelValueTransformer _transformer;
        private PlanetShaper _shaper;

		protected override PlanetMaterialState LayerState => PlanetMaterialState.Solid;
        protected override PlanetMaterialType MaterialType => PlanetMaterialType.Rock;

        public PlanetRockLayerAdder(PlanetLayerMaterialSerializer serializer,
            BiomeOccurrenceSerializer biomeOccurrenceSerializer) : 
            base(serializer, biomeOccurrenceSerializer)
		{
        }

        protected override void InitConcrete(PlanetLayerAdder.Parameter parameter)
        {
            Parameter p = parameter as Parameter;
            _planet = parameter.Planet;
            _shapingLayers = p.ShapingLayers;
            _shaper = new PlanetShaper(_shapingLayers);
            _relativeSeaLevel = _planet.Data.Dimensions.RelativeSeaLevel;
            _transformer = new SeaLevelValueTransformer(_relativeSeaLevel);
        }


        protected override void AddLayer(PlanetFace face)
        {
            float[] shapeValues = _shaper.Shape(face);

            for (int i = 0; i < shapeValues.Length; i++)
            {
                AddLayer(face, i, _bottomLayerHeight);
                AddLayer(face, i, Mathf.Clamp01(CalculateHeight(shapeValues[i])-_bottomLayerHeight));
            }
        }

        private float CalculateHeight(float shapeValue)
        {
            return _transformer.Transform(shapeValue);
        }

        protected override PlanetLayerMaterialSettings GetMaterial(Biome biome)
        {
            return biome.RockMaterial;
        }

        public new class Parameter : PlanetLayerAdder.Parameter
		{
			public Parameter(Biome[] biomes, 
                Planet planet, 
                ShapingLayer[] shapingLayers) : 
                base(biomes, planet)
			{
				ShapingLayers = shapingLayers;
			}

			public ShapingLayer[] ShapingLayers { get; }
		}
	}
}