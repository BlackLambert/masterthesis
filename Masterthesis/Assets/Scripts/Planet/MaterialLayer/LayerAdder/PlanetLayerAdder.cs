using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public abstract class PlanetLayerAdder
    {
		private Planet _planet;
		private Biome[] _biomes;
		private PlanetLayerMaterialSerializer _planetLayerMaterialSerializer;
        private BiomeOccurrenceSerializer _biomeOccurrenceSerializer;
        protected abstract PlanetMaterialState LayerState { get; }
        protected abstract PlanetMaterialType MaterialType { get; }

        public PlanetLayerAdder(PlanetLayerMaterialSerializer serializer,
            BiomeOccurrenceSerializer biomeOccurrenceSerializer)
		{
            _planetLayerMaterialSerializer = serializer;
            _biomeOccurrenceSerializer = biomeOccurrenceSerializer;
        }

        public void AddLayer(Parameter parameter)
        {
            Init(parameter);
            for (int i = 0; i < _planet.Faces.Length; i++)
                AddLayer(_planet.Faces[i]);
        }

        protected void Init(Parameter parameter)
        {
            _planet = parameter.Planet;
            _biomes = parameter.Biomes;
            InitConcrete(parameter);
        }

		protected abstract void InitConcrete(Parameter parameter);
		protected abstract void AddLayer(PlanetFace face);
        protected abstract PlanetLayerMaterialSettings GetMaterial(Biome biome);

        protected void AddLayer(PlanetFace face, int index, float height)
        {
            EvaluationPointData data = face.Data.EvaluationPoints[index];
            List<short> materials = CreateMaterials(data);
            AddLayer(data, height, materials);
        }

        protected void AddLayer(EvaluationPointData data, float height, List<short> materials)
        {
            if (height <= 0 || materials.Count == 0)
                return;
            List<PlanetMaterialLayerData> layers = data.Layers;
            PlanetMaterialLayerData layer = new PlanetMaterialLayerData(materials, LayerState, MaterialType, height);
            layers.Add(layer);
        }

        protected List<short> CreateMaterials(EvaluationPointData data)
        {
            List<short> result = new List<short>(data.Biomes.Length);
            BiomeOccurrence[] biomeOccurrences = GetBiomeOccurrences(data.Biomes);
            foreach (BiomeOccurrence biomeOccurrence in biomeOccurrences)
                AddMaterial(biomeOccurrence, result);
            return result;
        }

		private void AddMaterial(BiomeOccurrence biomeOccurrence, List<short> result)
		{
            Biome biome = _biomes[biomeOccurrence.ID];
            float portion = biomeOccurrence.Portion;
            PlanetLayerMaterialSettings material = GetMaterial(biome);
            if (material == null)
                return;
            short value = _planetLayerMaterialSerializer.Serialize(new PlanetLayerMaterial(material.ID, portion));
            result.Add(value);
        }

		protected BiomeOccurrence[] GetBiomeOccurrences(short[] biomes)
		{
            BiomeOccurrence[] result = new BiomeOccurrence[biomes.Length];
            for (int i = 0; i < biomes.Length; i++)
                result[i] = _biomeOccurrenceSerializer.Deserialize(biomes[i]);
            return result;
        }

        public class Parameter
		{
            public Parameter(Biome[] biomes,
                Planet planet)
			{
				Biomes = biomes;
				Planet = planet;
			}

			public Biome[] Biomes { get; }
			public Planet Planet { get; }
		}
    }
}