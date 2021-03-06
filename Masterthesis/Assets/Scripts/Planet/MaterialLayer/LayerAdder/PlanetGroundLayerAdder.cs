using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SBaier.Master
{
    public class PlanetGroundLayerAdder : PlanetLayerAdder
    {
        private const float _maxSolidTopLayerThickness = 0.02f;
        private const float _maxSlopeAngle = 90;

        private Biome[] _biomes;
        private float _heightFactor;

		protected override PlanetMaterialState LayerState => PlanetMaterialState.Solid;
        protected override PlanetMaterialType MaterialType => PlanetMaterialType.Ground;

        public PlanetGroundLayerAdder(PlanetLayerMaterialSerializer serializer,
            BiomeOccurrenceSerializer biomeOccurrenceSerializer) : 
            base(serializer, biomeOccurrenceSerializer)
		{
        }

        protected override void InitConcrete(Parameter parameter)
        {
            _biomes = parameter.Biomes;
            _heightFactor = _maxSolidTopLayerThickness / parameter.Planet.Data.Dimensions.MaxHullThickness;
        }

        protected override void AddLayer(PlanetFace face)
        {
            Vector3[] normals = face.Normals;
            Vector3[] vertices = face.WarpedVertices;
            for (int i = 0; i < normals.Length; i++)
            {
                EvaluationPointData data = face.Data.EvaluationPoints[i];
                AddLayer(face, i, CalculateHeight(vertices[i], normals[i], GetBiomeOccurrences(data.Biomes)));
            }
        }

		private float CalculateHeight(Vector3 warpedPoint, Vector3 normal, BiomeOccurrence[] biomeOccurrences)
		{
            float sum = 0;
            float portion = 0;
			foreach(BiomeOccurrence biomeOccurrence in biomeOccurrences)
			{
                portion += GetHeight(warpedPoint, normal, biomeOccurrence);
                sum += biomeOccurrence.Portion;
            }
            return (portion / sum) * _heightFactor;
		}

		private float GetHeight(Vector3 warpedPoint, Vector3 normal, BiomeOccurrence biomeOccurrence)
		{
            Biome biome = _biomes[biomeOccurrence.ID];
            SolidPlanetLayerMaterialSettings ground = biome.GroundMaterial;
            float angle = Vector3.Angle(warpedPoint, normal);
            float portion = angle / _maxSlopeAngle;
            float density = ground.Density;
            if (portion > density)
                return 0;
            float heightPortion = 1 - portion / density;
            return heightPortion * biomeOccurrence.Portion;
        }

        protected override PlanetLayerMaterialSettings GetMaterial(Biome biome)
        {
            return biome.GroundMaterial;
        }
    }
}