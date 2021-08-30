

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

        private Biome[] _biomes;
        private PlanetLayerMaterialSerializer _planetLayerMaterialSerializer;
        private SeaLevelValueTransformer _transformer;
        private HashSet<PlanetLayerMaterialSettings> _handledGround;

        public PlanetGroundVegetationLayerAdder(PlanetLayerMaterialSerializer planetLayerMaterialSerializer,
            BiomeOccurrenceSerializer biomeOccurrenceSerializer) : 
            base(planetLayerMaterialSerializer, biomeOccurrenceSerializer)
		{
            _planetLayerMaterialSerializer = planetLayerMaterialSerializer;
            _handledGround = new HashSet<PlanetLayerMaterialSettings>();
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
            BiomeOccurrence[] biomeOccurrences = GetBiomeOccurrences(data.Biomes);
            float heightPortion = 0;
            List<PlanetMaterialLayerData> layers = data.Layers;
            float heightSum = layers.Sum(l => l.Height);
            float heightEvalInput = _transformer.Revert(heightSum) * 2 - 1;
            _handledGround.Clear();
            foreach (BiomeOccurrence biomeOccurrence in biomeOccurrences)
            {
                Biome biome = _biomes[biomeOccurrence.ID];
                VegetationPlanetLayerMaterialSettings material = biome.Vegetation;
                if (material == null || _handledGround.Contains(material.GroundRequirements))
                    continue;
                _handledGround.Add(material.GroundRequirements);
                heightPortion += GetHeightPortion(biomeOccurrence, data, heightEvalInput);
            }
            return heightPortion * _maxVegetationLayerThickness;
        }

		private float GetHeightPortion(BiomeOccurrence biomeOccurrence, EvaluationPointData data, float heightEvaluationInput)
		{
            List<PlanetMaterialLayerData> layers = data.Layers;
            Biome biome = _biomes[biomeOccurrence.ID];
            VegetationPlanetLayerMaterialSettings material = biome.Vegetation;
            float heightGrowth = material.HeightRequirements.Evaluate(heightEvaluationInput);
            if (heightGrowth <= 0)
                return 0;

            PlanetMaterialLayerData topLayer = layers[layers.Count - 1];
            float groundRequirementPortion = 0;
            float portionSum = 0;
            foreach (short s in topLayer.Materials)
            {
                PlanetLayerMaterial m = _planetLayerMaterialSerializer.Deserialize(s);
                portionSum += m.Portion;
                if (m.MaterialID == material.GroundRequirements.ID)
                    groundRequirementPortion += m.Portion;
            }
            groundRequirementPortion /= portionSum;
            return heightGrowth * groundRequirementPortion;
        }

		protected override PlanetLayerMaterialSettings GetMaterial(Biome biome)
		{
            return biome.Vegetation;
		}
	}
}