using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SBaier.Master
{
    public class PlanetLayerMaterializer
    {
        private const float _maxSolidTopLayerThickness = 0.01f;
        private const float _maxSlopeAngle = 90;

		private PlanetLayerMaterialSerializer _serializer;

		private Planet _planet;
		private ContinentalPlates _plates;
		private Biome[] _biomes;
        private ShapingLayer[] _shapingLayers;
        private float _maxHullThickness;
        private float _relativeSeaLevel;
        private float _transformedSeaLevel;
        private float _blendDistance;

        public PlanetLayerMaterializer(PlanetLayerMaterialSerializer serializer)
		{
			_serializer = serializer;
		}

        public void UpdateRockElevation(Parameter parameter)
        {
            UpdateElevation(parameter, UpdateRockElevation);
        }

        public void UpdateTopSolidElevation(Parameter parameter)
        {
            UpdateElevation(parameter, UpdateGroundLayer);
        }

        public void UpdateLiquidElevation(Parameter parameter)
        {
            UpdateElevation(parameter, UpdateLiquidElevation);
        }

        public void UpdateAirElevation(Parameter parameter)
        {
            UpdateElevation(parameter, UpdateAirElevation);
        }

        private void UpdateElevation(Parameter parameter, Action<PlanetFace> updateMethod)
        {
            Init(parameter);
            for (int i = 0; i < _planet.Faces.Length; i++)
                updateMethod(_planet.Faces[i]);
        }

        private void Init(Parameter parameter)
		{
			_planet = parameter.Planet;
            _plates = parameter.Planet.Data.ContinentalPlates;
			_biomes = parameter.Biomes;
			_shapingLayers = parameter.ShapingLayers;
			_maxHullThickness = _planet.Data.Dimensions.MaxHullThickness;
			_relativeSeaLevel = _planet.Data.Dimensions.RelativeSeaLevel;
			_transformedSeaLevel = GetTransformedSeaLevel();
            _blendDistance = parameter.BlendFactor * _planet.AtmosphereRadius;
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
                UpdateRockLayer(face, shapeValues, i);
        }

        private void UpdateGroundLayer(PlanetFace face)
        {
            Vector3[] normals = face.Normals;
            for (int i = 0; i < normals.Length; i++)
                UpdateGroundLayer(face, i, normals[i]);
        }

        private void UpdateLiquidElevation(PlanetFace face)
        {
            int count = face.Data.EvaluationPoints.Length;
            for (int i = 0; i < count; i++)
                UpdateLiquidLayer(face, i);
        }

        private void UpdateAirElevation(PlanetFace face)
        {
            int count = face.Data.EvaluationPoints.Length;
            for (int i = 0; i < count; i++)
                UpdateAirLayer(face, i);
        }

        private void UpdateRockLayer(PlanetFace face, float[] shapeValues, int index)
		{
            EvaluationPointData data = face.Data.EvaluationPoints[index];
            float height = CalculateSolidHeight(shapeValues[index]);
            List<short> materials = GetMaterials(data, 0);
            AddLayer(data, PlanetMaterialState.Solid, height, materials);
        }

        private void UpdateGroundLayer(PlanetFace face, int index, Vector3 normal)
        {
            EvaluationPointData data = face.Data.EvaluationPoints[index];
            Biome biome = _biomes[data.BiomeID];
            float height = CalculateTopSolidHeight(data.WarpedPoint, normal, biome.GroundMaterial);
            List<short> materials = GetMaterials(data, 1);
            AddLayer(data, PlanetMaterialState.Solid, height, materials);
        }

        private void UpdateLiquidLayer(PlanetFace face, int index)
        {
            EvaluationPointData data = face.Data.EvaluationPoints[index];
            float height = CalculateLiquidHeight(data);
            List<short> materials = GetMaterials(data, 2);
            AddLayer(data, PlanetMaterialState.Liquid, height, materials);
        }

        private void UpdateAirLayer(PlanetFace face, int index)
        {
            EvaluationPointData data = face.Data.EvaluationPoints[index];
            float height = CalculateAirHeight(data);
            List<short> materials = GetMaterials(data, 3);
            AddLayer(data, PlanetMaterialState.Gas, height, materials);
        }

        private float CalculateAirHeight(EvaluationPointData data)
		{
            float heightSum = data.Layers.Sum(l => l.Height);
            return Mathf.Clamp01(1 - heightSum);
        }

		private float CalculateLiquidHeight(EvaluationPointData data)
		{
            float heightSum = data.Layers.Sum(l => l.Height);
            return Mathf.Max(_planet.Data.Dimensions.RelativeSeaLevel - heightSum, 0);
        }

        private float CalculateTopSolidHeight(Vector3 warpedPosition, Vector3 normal, SolidPlanetLayerMaterialSettings topSolidMaterial)
        {
            float angle = Vector3.Angle(warpedPosition, normal);
            float portion = angle / _maxSlopeAngle;
            float density = topSolidMaterial.Density;
            if (portion > density)
                return 0;
            float halfDensity = density / 2;
            if (portion < halfDensity)
                return _maxSolidTopLayerThickness;
            float heightPortion = 1 - (portion - halfDensity) / (topSolidMaterial.Density - halfDensity);
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
            return factor;
        }

        private void AddLayer(EvaluationPointData data, PlanetMaterialState state, float height, List<short> materials)
        {
            if (height <= 0)
                return;
            List<PlanetMaterialLayerData> layers = data.Layers;
            PlanetMaterialLayerData layer = new PlanetMaterialLayerData(materials, state, height);
            layers.Add(layer);
        }

        private List<short> GetMaterials(EvaluationPointData data, int materialIndex)
        {
            List<short> result = new List<short>();
            Biome biome = _biomes[data.BiomeID];
            result.Add(_serializer.Serialize(new PlanetLayerMaterial(biome.GetMeterial(materialIndex).ID, 1f)));
            float distanceToInnerBorder = GetDistance(data, data.ContinentalPlateSegmentIndex);
            if (distanceToInnerBorder < _blendDistance)
                result.AddRange(GetNeighborMaterials(data,  materialIndex));
            return result;
        }

        private List<short> GetNeighborMaterials(EvaluationPointData data, int materialIndex)
        {
            ContinentalPlateSegment segment = _plates.Segments[data.ContinentalPlateSegmentIndex];
            List<short> result = new List<short>();
            int[] neighbors = segment.Neighbors;
            for (int i = 0; i < neighbors.Length; i++)
                AddNeighborMaterial(data, neighbors[i], materialIndex, result);
            return result;
        }

        private void AddNeighborMaterial(EvaluationPointData data, int neighborIndex, int materialIndex, List<short> result)
        {
            ContinentalPlateSegment neighborSegment = _plates.Segments[neighborIndex];
            Biome neighborBiome = _biomes[neighborSegment.BiomeID];
            float distanceToSegment = GetDistance(data, neighborIndex);
            if (distanceToSegment > _blendDistance)
                return;
            short value = _serializer.Serialize(new PlanetLayerMaterial(neighborBiome.GetMeterial(materialIndex).ID, 1 - (distanceToSegment / _blendDistance)));
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
            public Parameter(Planet planet,
                Biome[] biomes,
                ShapingLayer[] shapingLayers,
                float blendFactor)
			{
				Planet = planet;
				Biomes = biomes;
				ShapingLayers = shapingLayers;
				BlendFactor = blendFactor;
			}

			public Planet Planet { get; }
			public Biome[] Biomes { get; }
			public ShapingLayer[] ShapingLayers { get; }
			public float BlendFactor { get; }
		}
    }
}