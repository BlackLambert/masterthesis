using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SBaier.Master
{
    public class PlanetLiquidLayerAdder : PlanetLayerAdder
    {
        private Planet _planet;

        protected override PlanetMaterialState LayerState => PlanetMaterialState.Liquid;

        public PlanetLiquidLayerAdder(PlanetLayerMaterialSerializer serializer,
            BiomeOccurrenceSerializer biomeOccurrenceSerializer) : 
            base(serializer, biomeOccurrenceSerializer)
		{
        }

        protected override void InitConcrete(Parameter parameter)
        {
            _planet = parameter.Planet;
        }

        protected override void AddLayer(PlanetFace face)
        {
            int count = face.Data.EvaluationPoints.Length;
            for (int i = 0; i < count; i++)
                AddLayer(face, i, CalculateHeight(face.Data.EvaluationPoints[i]));
        }

        private float CalculateHeight(EvaluationPointData data)
        {
            float heightSum = data.Layers.Sum(l => l.Height);
            return Mathf.Max(_planet.Data.Dimensions.RelativeSeaLevel - heightSum, 0);
        }

        protected override PlanetLayerMaterialSettings GetMaterial(Biome biome)
        {
            return biome.LiquidMaterial;
        }
    }
}