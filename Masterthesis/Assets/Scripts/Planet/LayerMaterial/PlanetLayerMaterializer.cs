using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SBaier.Master
{
    public class PlanetLayerMaterializer
    {
        private const float _maxSolidTopLayerThickness = 0.02f;
        private const float _maxSlopeAngle = 90;

        private Planet _planet;
        private Biome[] _biomes;
        private ShapingLayer[] _shapingLayers;
        private float _maxHullThickness;
        private float _relativeSeaLevel;
        private float _transformedSeaLevel;

        public PlanetLayerMaterializer()
		{
        }

        public void UpdateRockElevation(Parameter parameter)
        {
            Init(parameter);
            for (int i = 0; i < _planet.Faces.Length; i++)
                UpdateRockElevation(_planet.Faces[i]);
        }

        public void UpdateTopSolidElevation(Parameter parameter)
        {
            Init(parameter);
            for (int i = 0; i < _planet.Faces.Length; i++)
                UpdateTopSolidElevation(_planet.Faces[i]);
        }

        public void UpdateLiquidElevation(Parameter parameter)
        {
            Init(parameter);
            for (int i = 0; i < _planet.Faces.Length; i++)
                UpdateLiquidElevation(_planet.Faces[i]);
        }

        public void UpdateAirElevation(Parameter parameter)
        {
            Init(parameter);
            for (int i = 0; i < _planet.Faces.Length; i++)
                UpdateAirElevation(_planet.Faces[i]);
        }

		private void Init(Parameter parameter)
		{
			_planet = parameter.Planet;
			_biomes = parameter.Biomes;
			_shapingLayers = parameter.ShapingLayers;
			_maxHullThickness = _planet.Data.Dimensions.MaxHullThickness;
			_relativeSeaLevel = _planet.Data.Dimensions.RelativeSeaLevel;
			_transformedSeaLevel = GetTransformedSeaLevel();
        }

		private float GetTransformedSeaLevel()
		{
			return (_relativeSeaLevel - 0.5f) * 2;
		}

		private void UpdateRockElevation(PlanetFace face)
		{
            PlanetShaper shaper = new PlanetShaper(_shapingLayers, _maxHullThickness);
            float[] shapeValues = shaper.Shape(face.Data.EvaluationPoints);

            for (int i = 0; i < shapeValues.Length; i++)
                UpdateRockElevation(face, shapeValues, i);
        }

        private void UpdateTopSolidElevation(PlanetFace face)
        {
            Vector3[] normals = face.Normals;
            for (int i = 0; i < normals.Length; i++)
                UpdateTopSolidElevation(face, i, normals[i]);
        }

        private void UpdateLiquidElevation(PlanetFace face)
        {
            int count = face.Data.EvaluationPoints.Length;
            for (int i = 0; i < count; i++)
                UpdateLiquidElevation(face, i);
        }

        private void UpdateAirElevation(PlanetFace face)
        {
            int count = face.Data.EvaluationPoints.Length;
            for (int i = 0; i < count; i++)
                UpdateAirElevation(face, i);
        }

        private void UpdateRockElevation(PlanetFace face, float[] shapeValues, int index)
		{
            EvaluationPointData data = face.Data.EvaluationPoints[index];
            Biome biome = _biomes[data.BiomeID];
            float height = CalculateSolidHeight(shapeValues[index]);
            AddLayer(data, height, biome.RockMaterial);
        }

        private void UpdateTopSolidElevation(PlanetFace face, int index, Vector3 normal)
        {
            EvaluationPointData data = face.Data.EvaluationPoints[index];
            Biome biome = _biomes[data.BiomeID];
            float height = CalculateTopSolidHeight(data.WarpedPoint, normal, biome.TopSolidMaterial);
            AddLayer(data, height, biome.TopSolidMaterial);
        }

        private void UpdateLiquidElevation(PlanetFace face, int index)
        {
            EvaluationPointData data = face.Data.EvaluationPoints[index];
            Biome biome = _biomes[data.BiomeID];
            float height = CalculateLiqidHeight(data);
            AddLayer(data, height, biome.LiquidMaterial);
        }

        private void UpdateAirElevation(PlanetFace face, int index)
        {
            EvaluationPointData data = face.Data.EvaluationPoints[index];
            Biome biome = _biomes[data.BiomeID];
            float height = CalculateAirHeight(data);
            AddLayer(data, height, biome.GasMaterial);
        }

        private float CalculateAirHeight(EvaluationPointData data)
		{
            float heightSum = data.Layers.Sum(l => l.Height);
            return Mathf.Clamp01(1 - heightSum);
        }

		private float CalculateLiqidHeight(EvaluationPointData data)
		{
            float heightSum = data.Layers.Sum(l => l.Height);
            return Mathf.Max(_planet.Data.Dimensions.RelativeSeaLevel - heightSum, 0);
        }

        private float CalculateTopSolidHeight(Vector3 warpedPosition, Vector3 normal, SolidPlanetLayerMaterialSettings topSolidMaterial)
        {
            float angle = Vector3.Angle(warpedPosition, normal);
            float portion = angle / _maxSlopeAngle;
            if (angle / _maxSlopeAngle > topSolidMaterial.Density)
                return 0;
            float heightPortion = (topSolidMaterial.Density - portion) * (1 - topSolidMaterial.Density);
            return heightPortion * _maxSolidTopLayerThickness;
        }

        private float CalculateSolidHeight(float shapeValue)
        {
            float transformedShape = shapeValue - 0.5f;
            float factor;
            if (transformedShape > 0)
                factor = (1 - _transformedSeaLevel) * transformedShape;
            else if (transformedShape < 0)
                factor = (1 + _transformedSeaLevel) * transformedShape;
            else
                factor = 0;
            factor += _relativeSeaLevel;
            return factor * _maxHullThickness;
        }

        private void AddLayer(EvaluationPointData data, float height, PlanetLayerMaterialSettings material)
        {
            if (height <= 0)
                return;
            List<PlanetLayerData> layers = data.Layers;
            PlanetLayerData layer = new PlanetLayerData(material.ID, height);
            layers.Add(layer);
        }

        public class Parameter
		{
            public Parameter(Planet planet,
                Biome[] biomes,
                ShapingLayer[] shapingLayers)
			{
				Planet = planet;
				Biomes = biomes;
				ShapingLayers = shapingLayers;
			}

			public Planet Planet { get; }
			public Biome[] Biomes { get; }
			public ShapingLayer[] ShapingLayers { get; }
		}
    }
}