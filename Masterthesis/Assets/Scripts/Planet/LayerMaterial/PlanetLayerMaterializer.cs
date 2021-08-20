using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public class PlanetLayerMaterializer
    {
        private Planet _planet;
        private Biome[] _biomes;
        private ShapingLayer[] _shapingLayers;
        private float _maxHullThickness;
        private float _relativeSeaLevel;
        private float _transformedSeaLevel;

        public PlanetLayerMaterializer()
		{
        }

        public void UpdateElevation(Parameter parameter)
        {
            Init(parameter);
            for (int i = 0; i < _planet.Faces.Length; i++)
                UpdateElevation(_planet.Faces[i]);
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

		private void UpdateElevation(PlanetFace face)
		{
            PlanetShaper shaper = new PlanetShaper(_shapingLayers, _maxHullThickness);
            float[] shapeValues = shaper.Shape(face.Data.EvaluationPoints);

            for (int j = 0; j < shapeValues.Length; j++)
				UpdateElevation(face, shapeValues, j);
		}

		private void UpdateElevation(PlanetFace face, float[] shapeValues, int index)
        {
            ContinentalPlates plates = _planet.Data.ContinentalPlates;
            EvaluationPointData data = face.Data.EvaluationPoints[index];
            int segmentIndex = data.ContinentalPlateSegmentIndex;
            ContinentalPlateSegment segment = plates.Segments[segmentIndex];
			Biome biome = _biomes[segment.BiomeID];
			float delta = CalculateDelta(shapeValues[index], biome);
			InitContinentPlate(face.Data.EvaluationPoints[index], delta);
		}

		private float CalculateDelta(float shapeValue, Biome biome)
        {
            //bool isOceanic = biome.RegionType == ContinentalRegion.Type.Oceanic;
            //if (isOceanic)
            //return CreateOceanDelta(planet, shapeValue);
            //else
            return CreateLandDelta(shapeValue);
        }

        private float CreateOceanDelta(float shapeValue)
        {
            if (shapeValue <= 0.5f)
                return _relativeSeaLevel * _maxHullThickness;
            return CreateLandDelta(shapeValue);
        }

        private float CreateLandDelta(float shapeValue)
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

        private void InitContinentPlate(EvaluationPointData data, float value)
        {
            //float continentalPlateHeight = value * parameter.Dimensions.MaxHullThickness;
            List<PlanetLayerData> layers = data.Layers;
            PlanetLayerData airLayer = layers[0];
            airLayer.Height = airLayer.Height - value;
            layers.Insert(0, new PlanetLayerData(1, value));
        }

        public class Parameter
		{
            public Parameter(Planet planet, Biome[] biomes, ShapingLayer[] shapingLayers)
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