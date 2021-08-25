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
		private ContinentalPlates _plates;
		private float _blendDistance;
		private PlanetLayerMaterialSerializer _serializer;
        protected abstract PlanetMaterialState LayerState { get; }
        protected abstract int MaterialIndex { get; }

        public PlanetLayerAdder(PlanetLayerMaterialSerializer serializer)
		{
            _serializer = serializer;

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
            _plates = parameter.Planet.Data.ContinentalPlates;
            _blendDistance = parameter.BlendFactor * _planet.AtmosphereRadius;
            InitConcrete(parameter);
        }

		protected abstract void InitConcrete(Parameter parameter);
		protected abstract void AddLayer(PlanetFace face);

        protected void AddLayer(PlanetFace face, int index, float height)
        {
            EvaluationPointData data = face.Data.EvaluationPoints[index];
            Biome biome = _biomes[data.BiomeID];
            PlanetLayerMaterialSettings material = biome.GetMeterial(MaterialIndex);
            if (material == null)
                return;
            List<short> materials = GetMaterials(data);
            AddLayer(data, LayerState, height, materials);
        }

        protected void AddLayer(EvaluationPointData data, PlanetMaterialState state, float height, List<short> materials)
        {
            if (height <= 0)
                return;
            List<PlanetMaterialLayerData> layers = data.Layers;
            PlanetMaterialLayerData layer = new PlanetMaterialLayerData(materials, state, height);
            layers.Add(layer);
        }

        protected List<short> GetMaterials(EvaluationPointData data)
        {
            List<short> result = new List<short>();
            Biome biome = _biomes[data.BiomeID];
            PlanetLayerMaterialSettings material = biome.GetMeterial(MaterialIndex);
            result.Add(_serializer.Serialize(new PlanetLayerMaterial(material.ID, 1f)));
            float distanceToInnerBorder = GetDistance(data, data.ContinentalPlateSegmentIndex);
            if (distanceToInnerBorder < _blendDistance)
                result.AddRange(GetNeighborMaterials(data));
            return result;
        }

        private List<short> GetNeighborMaterials(EvaluationPointData data)
        {
            ContinentalPlateSegment segment = _plates.Segments[data.ContinentalPlateSegmentIndex];
            List<short> result = new List<short>();
            int[] neighbors = segment.Neighbors;
            for (int i = 0; i < neighbors.Length; i++)
                AddNeighborMaterial(data, neighbors[i], result);
            return result;
        }

        private void AddNeighborMaterial(EvaluationPointData data, int neighborIndex, List<short> result)
        {
            ContinentalPlateSegment neighborSegment = _plates.Segments[neighborIndex];
            Biome neighborBiome = _biomes[neighborSegment.BiomeID];
            float distanceToSegment = GetDistance(data, neighborIndex);
            if (distanceToSegment > _blendDistance)
                return;
            PlanetLayerMaterialSettings neighborMaterial = neighborBiome.GetMeterial(MaterialIndex);
            if (neighborMaterial == null)
                return;
            float weight = 1 - (distanceToSegment / _blendDistance);
            short value = _serializer.Serialize(new PlanetLayerMaterial(neighborMaterial.ID, weight));
            result.Add(value);
        }

        private float GetDistance(EvaluationPointData data, int segmentIndex)
        {
            Vector3 vertex = data.WarpedPoint;
            Vector3 pointNeighborOnBorder = _plates.SegmentsVoronoi.GetNearestBorderPointOf(vertex, segmentIndex);
            float distanceToSegment = _planet.GetDistanceOnSurface(vertex, pointNeighborOnBorder);
            return distanceToSegment;
        }

        public class Parameter
		{
            public Parameter(Biome[] biomes,
                Planet planet,
                float blendFactor)
			{
				Biomes = biomes;
				Planet = planet;
				BlendFactor = blendFactor;
			}

			public Biome[] Biomes { get; }
			public Planet Planet { get; }
			public float BlendFactor { get; }
		}
    }
}