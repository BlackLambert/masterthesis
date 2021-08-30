

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SBaier.Master
{
    public class PlanetGroundVegetationLayerAdder : PlanetLayerAdder
    {
        private const float _maxVegetationLayerThickness = 0.02f;

		protected override PlanetMaterialState LayerState => PlanetMaterialState.Solid;
		protected override int MaterialIndex => 4;

        private Biome[] _biomes;
        private PlanetLayerMaterialSerializer _serializer;
        private SeaLevelValueTransformer _transformer;

        public PlanetGroundVegetationLayerAdder(PlanetLayerMaterialSerializer serializer) : base(serializer)
		{
            _serializer = serializer;
        }

        protected override void InitConcrete(Parameter parameter)
        {
            _biomes = parameter.Biomes;
            _transformer = new SeaLevelValueTransformer(parameter.Planet.Data.Dimensions.RelativeSeaLevel);
        }

        protected override void AddLayer(PlanetFace face)
        {
            int count = face.Data.EvaluationPoints.Length;
            for (int i = 0; i < count; i++)
                AddLayer(face, i, GetHeight(face.Data.EvaluationPoints[i]));
        }

		private float GetHeight(EvaluationPointData data)
		{
            List<PlanetMaterialLayerData> layers = data.Layers;
            float heightSum = layers.Sum(l => l.Height);
            Biome biome = _biomes[data.BiomeID];
            VegetationPlanetLayerMaterialSettings material = biome.GetMeterial(MaterialIndex) as VegetationPlanetLayerMaterialSettings;
            if (material == null)
                return 0;
            float heightGrowth = material.HeightRequirements.Evaluate(_transformer.Revert(heightSum) * 2 - 1);

            PlanetMaterialLayerData topLayer = layers[layers.Count - 1];
            float portion = 0;
            float portionSum = 0;
			foreach(short s in topLayer.Materials)
			{
                PlanetLayerMaterial m = _serializer.Deserialize(s);
                portionSum += m.Portion;
                if (m.MaterialID == material.GroundRequirements.ID)
                    portion += m.Portion;
            }
            portion /= portionSum;

            return Mathf.Min(heightGrowth, portion) * _maxVegetationLayerThickness;
        }
	}
}