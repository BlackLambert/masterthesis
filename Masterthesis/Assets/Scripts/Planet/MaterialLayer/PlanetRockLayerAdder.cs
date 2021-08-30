using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SBaier.Master
{
    public class PlanetRockLayerAdder : PlanetLayerAdder
    {
		private const int _rockMaterialIndex = 0;

		private Planet _planet;
		private ShapingLayer[] _shapingLayers;
		private float _maxHullThickness;
		private float _relativeSeaLevel;
        private float _transformedSeaLevel;
        private SeaLevelValueTransformer _transformer;

		protected override PlanetMaterialState LayerState => PlanetMaterialState.Solid;
		protected override int MaterialIndex => _rockMaterialIndex;

		public PlanetRockLayerAdder(PlanetLayerMaterialSerializer serializer) : base(serializer)
		{
        }

        protected override void InitConcrete(PlanetLayerAdder.Parameter parameter)
        {
            Parameter p = parameter as Parameter;
            _planet = parameter.Planet;
            _shapingLayers = p.ShapingLayers;
            _maxHullThickness = _planet.Data.Dimensions.MaxHullThickness;
            _relativeSeaLevel = _planet.Data.Dimensions.RelativeSeaLevel;
            _transformedSeaLevel = GetTransformedSeaLevel();
            _transformer = new SeaLevelValueTransformer(_relativeSeaLevel);
        }


        protected override void AddLayer(PlanetFace face)
        {
            PlanetShaper shaper = new PlanetShaper(_shapingLayers, _maxHullThickness);
            float[] shapeValues = shaper.Shape(face.Data.EvaluationPoints);

            for (int i = 0; i < shapeValues.Length; i++)
                AddLayer(face, i, CalculateHeight(shapeValues[i]));
        }

        private float GetTransformedSeaLevel()
        {
            return (_relativeSeaLevel - 0.5f) * 2;
        }

        private float CalculateHeight(float shapeValue)
        {
            return _transformer.Transform(shapeValue);
        }

		public new class Parameter : PlanetLayerAdder.Parameter
		{
			public Parameter(Biome[] biomes, 
                Planet planet, 
                float blendDistance,
                ShapingLayer[] shapingLayers) : 
                base(biomes, planet, blendDistance)
			{
				ShapingLayers = shapingLayers;
			}

			public ShapingLayer[] ShapingLayers { get; }
		}
	}
}