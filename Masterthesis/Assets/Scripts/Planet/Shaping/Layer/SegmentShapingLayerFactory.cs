using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
	public class SegmentShapingLayerFactory : ShapingLayerFactory
    {
        private const float _oceansBlendDistanceFactor = 0.05f;
        private const float _continentsBlendDistanceFactor = 0.03f;
        private const float _continentWeight = 0.3f;
        private const float _oceanWeight = 0.8f;

        private Parameter _parameter;
        List<ShapingPrimitive> _oceanPrimitives;
        List<ShapingPrimitive> _continentPrimitives;

        public SegmentShapingLayerFactory() : base()
		{
            _oceanPrimitives = new List<ShapingPrimitive>();
            _continentPrimitives = new List<ShapingPrimitive>();
        }

		public ShapingLayer[] Create(Parameter parameter)
		{
			Init(parameter);
			CreateOceanPrimitives();
			CreateContinentPrimitives();
			return CreateLayers();
		}

		private void Init(Parameter parameter)
		{
            _parameter = parameter;
            _oceanPrimitives.Clear();
            _continentPrimitives.Clear();
        }

        private ShapingLayer[] CreateLayers()
        {
            ShapingPrimitive[][] primitives = new ShapingPrimitive[][] { _oceanPrimitives.ToArray(), _continentPrimitives.ToArray() };
            Noise3D[] noise = new Noise3D[] { _parameter.OceanNoise, _parameter.ContinentNoise };
            ShapingLayer.Mode[] modes = new ShapingLayer.Mode[] { ShapingLayer.Mode.Blend, ShapingLayer.Mode.Blend };
            return CreateLayers(primitives, noise, modes);
        }

        private void CreateContinentPrimitives()
        {
            ContinentalPlates plates = _parameter.Data.ContinentalPlates;
            for (int i = 0; i < plates.Regions.Length; i++)
                CreateContinentPrimitives(plates.Regions[i]);
        }

        private void CreateContinentPrimitives(ContinentalRegion region)
		{
            if (region.RegionType != ContinentalRegion.Type.ContinentalPlate)
                return;
            for (int j = 0; j < region.Segements.Length; j++)
                _continentPrimitives.Add(CreateContinentPrimitive(region.Segements[j]));
        }

        private ShapingPrimitive CreateContinentPrimitive(int segmentIndex)
        {
            ContinentalPlates plates = _parameter.Data.ContinentalPlates;
            ContinentalPlateSegment segment = plates.Segments[segmentIndex];
            PolygonBody body = plates.SegmentsVoronoi;
            Vector3 pos = segment.Site;
            float blendDistance = _continentsBlendDistanceFactor * _parameter.Data.Dimensions.AtmosphereRadius;
            return new ConvexPolygonShapingPrimitive(body, segmentIndex, pos, blendDistance, _continentWeight);
        }

        private void CreateOceanPrimitives()
        {
            ContinentalPlates plates = _parameter.Data.ContinentalPlates;
            for (int i = 0; i < plates.Regions.Length; i++)
                CreateOceanPrimitives(plates.Regions[i]);
        }

        private void CreateOceanPrimitives(ContinentalRegion region)
		{
            if (region.RegionType != ContinentalRegion.Type.Oceanic)
                return;
            for (int j = 0; j < region.Segements.Length; j++)
                _oceanPrimitives.Add(CreateOceanPrimitive(region.Segements[j]));
        }

        private ShapingPrimitive CreateOceanPrimitive(int segmentIndex)
        {
            ContinentalPlates plates = _parameter.Data.ContinentalPlates;
            ContinentalPlateSegment segment = plates.Segments[segmentIndex];
            PolygonBody body = plates.SegmentsVoronoi;
            Vector3 pos = segment.Site;
            float blendDistance = _oceansBlendDistanceFactor * _parameter.Data.Dimensions.AtmosphereRadius;
            return new ConvexPolygonShapingPrimitive(body, segmentIndex, pos, blendDistance, _oceanWeight);
        }

        public class Parameter
        {
            public Parameter(
                PlanetData data,
                Noise3D oceanNoise,
                Noise3D continentNoise)
            {
                Data = data;
                OceanNoise = oceanNoise;
                ContinentNoise = continentNoise;
            }

            public PlanetData Data { get; }
            public Noise3D OceanNoise { get; }
            public Noise3D ContinentNoise { get; }
        }
    }
}