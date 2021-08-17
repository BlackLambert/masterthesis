using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public class PlanetLayerMaterializer
    {
        private Vector3BinaryKDTreeFactory _treeFactory;

        private Planet _planet;
        private Biome[] _biomes;
        private ShapingLayer[] _shapingLayers;
        private KDTree<Vector3> _segmentsKDTree;

        public PlanetLayerMaterializer(Vector3BinaryKDTreeFactory treeFactory)
		{
            _treeFactory = treeFactory;
        }

        public void UpdateElevation(Parameter parameter)
        {
            Init(parameter);
            for (int i = 0; i < _planet.Faces.Count; i++)
                UpdateElevation(_planet.Faces[i]);
        }

        private void Init(Parameter parameter)
        {
            _planet = parameter.Planet;
            _biomes = parameter.Biomes;
            _shapingLayers = parameter.ShapingLayers;
            ContinentalPlates plates = _planet.Data.ContinentalPlates;
            _segmentsKDTree = _treeFactory.Create(plates.SegmentSites);
        }

        private void UpdateElevation(PlanetFace face)
		{
            PlanetShaper shaper = new PlanetShaper(_shapingLayers, _planet.Data.Dimensions.RelativeSeaLevel);
            float[] shapeValues = shaper.Shape(face.Data.EvaluationPoints);

            for (int j = 0; j < shapeValues.Length; j++)
				UpdateElevation(face, shapeValues, j);
		}

		private void UpdateElevation(PlanetFace face, float[] shapeValues, int index)
        {
            ContinentalPlates plates = _planet.Data.ContinentalPlates;
            Vector3 vertex = face.Data.EvaluationPoints[index].WarpedPoint;
			int segmentIndex = _segmentsKDTree.GetNearestTo(vertex);
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
                return _planet.Data.Dimensions.RelativeSeaLevel * _planet.Data.Dimensions.MaxHullThickness;
            return CreateLandDelta(shapeValue);
        }

        private float CreateLandDelta(float shapeValue)
        {
            float transformedShape = shapeValue - 0.5f;
            float seaLevel = _planet.Data.Dimensions.RelativeSeaLevel;
            float transformdedSeaLevel = (seaLevel - 0.5f) * 2;
            float posFactor = 1 - transformdedSeaLevel;
            float negFactor = 1 + transformdedSeaLevel;
            float factor;
            if (transformedShape > 0)
                factor = posFactor * transformedShape;
            else if (transformedShape < 0)
                factor = negFactor * transformedShape;
            else
                factor = 0;
            factor += seaLevel;
            return factor * _planet.Data.Dimensions.MaxHullThickness;
        }

        private void InitContinentPlate(EvaluationPointData data, float value)
        {
            //float continentalPlateHeight = value * parameter.Dimensions.MaxHullThickness;
            data.Layers[0].Height = data.Layers[0].Height - value;
            data.Layers.Insert(0, new PlanetLayerData(1, value));
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