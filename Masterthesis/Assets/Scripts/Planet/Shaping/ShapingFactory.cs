using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SBaier.Master
{
    public class ShapingFactory
    {
		private const float _mountainsBreadthFactor = 0.10f;
		private const float _mountainsBlendDistanceFactor = 0.15f;
        private const float _canyonsBreadthFactor = 0.05f;
        private const float _canyonsBlendDistanceFactor = 0.08f;
        private const float _oceansBlendDistanceFactor = 0.05f;
        private const float _continentsBlendDistanceFactor = 0.03f;
        private const float _lenthAdditionFactor = 0.05f;
		private const float _oceanWeight = 0.8f;
		private const float _continentWeight = 0.3f;
		private Vector3BinaryKDTreeFactory _kdTreeFactory;

        public ShapingFactory(Vector3BinaryKDTreeFactory kdTreeFactory)
        {
            _kdTreeFactory = kdTreeFactory;
        }


        public ShapingLayer[] Create(Parameter parameter)
        {
            List<ShapingLayer> result = new List<ShapingLayer>();
            ShapingLayer baseLayer = CreateSeaLevelLayer(parameter);
            ShapingLayer[] segmentShapings = CreateSegmentShaping(parameter);
            ShapingLayer oceansShaping = segmentShapings[0];
            ShapingLayer continentsShaping = segmentShapings[1];
            ShapingLayer[] continentsBorderShaping = CreateBorderShaping(parameter);
            ShapingLayer mountainsShaping = continentsBorderShaping[0];
            ShapingLayer canyonsShaping = continentsBorderShaping[1];
            result.Add(baseLayer);
            result.Add(continentsShaping);
            result.Add(oceansShaping);
            result.Add(mountainsShaping);
            result.Add(canyonsShaping);
            return result.ToArray();
        }

		private ShapingLayer CreateSeaLevelLayer(Parameter parameter)
		{
            ShapingPrimitive[] primitive = new ShapingPrimitive[] { new AllOverShapingPrimitive(Vector3.zero, 0, 1) };
            return CreateShapingLayer(primitive, parameter.SeaLevelNoise, ShapingLayer.Mode.Add);
		}

		private ShapingLayer[] CreateSegmentShaping(Parameter parameter)
        {
            List<ShapingPrimitive> oceanPrimitives = CreateOceanPrimitives(parameter);
            List<ShapingPrimitive> continentPrimitives = CreateContinentPrimitives(parameter);
            ShapingPrimitive[][] primitives = new ShapingPrimitive[][] { oceanPrimitives.ToArray(), continentPrimitives.ToArray() };
            Noise3D[] noise = new Noise3D[] { parameter.OceanNoise, parameter.ContinentNoise };
            ShapingLayer.Mode[] modes = new ShapingLayer.Mode[] { ShapingLayer.Mode.Blend, ShapingLayer.Mode.Blend };
            return CreateLayers(primitives, noise, modes);
        }

		private List<ShapingPrimitive> CreateContinentPrimitives(Parameter parameter)
		{
            ContinentalPlates plates = parameter.Data.ContinentalPlates;
            List<ShapingPrimitive> primitives = new List<ShapingPrimitive>();
            for (int i = 0; i < plates.Regions.Length; i++)
            {
                ContinentalRegion region = plates.Regions[i];
                if (region.RegionType != ContinentalRegion.Type.ContinentalPlate)
                    continue;
                for (int j = 0; j < region.Segements.Length; j++)
                {
                    int segmentIndex = region.Segements[j];
                    primitives.Add(CreateContinentPrimitives(parameter, segmentIndex));
                }
            }
            return primitives;
        }

		private ShapingPrimitive CreateContinentPrimitives(Parameter parameter, int segmentIndex)
		{
            ContinentalPlates plates = parameter.Data.ContinentalPlates;
            ContinentalPlateSegment segment = plates.Segments[segmentIndex];
            PolygonBody body = plates.SegmentsVoronoi;
            Vector3 pos = segment.Site;
            float blendDistance = _continentsBlendDistanceFactor * parameter.Data.Dimensions.AtmosphereRadius;
            return new ConvexPolygonShapingPrimitive(body, segmentIndex, pos, blendDistance, _continentWeight);
        }

		private List<ShapingPrimitive> CreateOceanPrimitives(Parameter parameter)
		{
            ContinentalPlates plates = parameter.Data.ContinentalPlates;
            List<ShapingPrimitive> primitives = new List<ShapingPrimitive>();
            KDTree<Vector3> segmentsTree = _kdTreeFactory.Create(plates.SegmentSites);
            for (int i = 0; i < plates.Regions.Length; i++)
			{
                ContinentalRegion region = plates.Regions[i];
                if (region.RegionType != ContinentalRegion.Type.Oceanic)
                    continue;
				for (int j = 0; j < region.Segements.Length; j++)
				{
                    int segmentIndex = region.Segements[j];
                    primitives.Add(CreateOceanPrimitive(parameter, segmentIndex, segmentsTree));
                }
            }
            return primitives;
		}

		private ShapingPrimitive CreateOceanPrimitive(Parameter parameter, int segmentIndex, KDTree<Vector3> segmentsTree)
        {
            ContinentalPlates plates = parameter.Data.ContinentalPlates;
            ContinentalPlateSegment segment = plates.Segments[segmentIndex];
            PolygonBody body = plates.SegmentsVoronoi;
            Vector3 pos = segment.Site;
            float blendDistance = _oceansBlendDistanceFactor * parameter.Data.Dimensions.AtmosphereRadius;
            return new ConvexPolygonShapingPrimitive(body, segmentIndex, pos, blendDistance, _oceanWeight);
        }

		private ShapingLayer[] CreateBorderShaping(Parameter parameter)
		{
			ContinentalPlates plates = parameter.Data.ContinentalPlates;
			List<ShapingPrimitive> mountainPrimitives = new List<ShapingPrimitive>();
			List<ShapingPrimitive> canyonPrimitives = new List<ShapingPrimitive>();
			HashSet<Vector2Int> handledBorders = new HashSet<Vector2Int>();
			for (int i = 0; i < plates.PlateNeighbors.Length; i++)
			{
				Vector2Int neighbors = plates.PlateNeighbors[i];
				float weight = CalculateMountainWeight(plates, neighbors);

				Vector2Int[] borders = plates.PlateBorders[neighbors];
				for (int j = 0; j < borders.Length; j++)
				{
					Vector2Int border = borders[j];
					if (handledBorders.Contains(border))
						continue;
					Vector3 corner0 = plates.SegmentCorners[border[0]];
					Vector3 corner1 = plates.SegmentCorners[border[1]];
					Vector3 distanceVector = corner1 - corner0;
					float distance = distanceVector.magnitude;
					if (distance == 0)
						continue;
					Vector3 pos = (corner0 + distanceVector / 2).normalized * corner0.magnitude;

					if (weight > 0)
					{
						ShapingPrimitive mountainPrimitive = CreateMountainPrimitive(parameter.Data, distanceVector, weight, pos);
						mountainPrimitives.Add(mountainPrimitive);
					}
					else
					{
						ShapingPrimitive canyonPrimitive = CreateCanyonsPrimitive(parameter.Data, distanceVector, -weight, pos);
						canyonPrimitives.Add(canyonPrimitive);
					}
					handledBorders.Add(border);
				}
			}

            ShapingPrimitive[][] primitives = new ShapingPrimitive[][] { mountainPrimitives.ToArray(), canyonPrimitives.ToArray() };
            Noise3D[] noise = new Noise3D[] { parameter.MountainNoise, parameter.CanyonsNoise };
            ShapingLayer.Mode[] modes = new ShapingLayer.Mode[] { ShapingLayer.Mode.Blend, ShapingLayer.Mode.Blend };
            return CreateLayers(primitives, noise, modes);
		}

		private ShapingLayer[] CreateLayers(ShapingPrimitive[][] primitives, Noise3D[] noise, ShapingLayer.Mode[] modes)
		{
			List<ShapingLayer> layers = new List<ShapingLayer>();
			for (int i = 0; i < primitives.Length; i++)
			{
                ShapingPrimitive[] layerPrimitives = primitives[i];
                layers.Add(CreateShapingLayer(layerPrimitives, noise[i], modes[i]));
            }
			return layers.ToArray();
		}

		private ShapingLayer CreateShapingLayer(ShapingPrimitive[] primitives, Noise3D noise, ShapingLayer.Mode mode)
        {
            Vector3[] nodes = primitives.Select(p => p.Position).ToArray();
            KDTree<Vector3> tree = nodes.Length > 0 ? _kdTreeFactory.Create(nodes) : null;
            return new ShapingLayer(primitives, tree, noise, mode);
        }

        private ShapingPrimitive CreateCanyonsPrimitive(PlanetData data, Vector3 distanceVector, float weight, Vector3 pos)
        {
            float maxBreadth = data.Dimensions.AtmosphereRadius * _canyonsBreadthFactor;
            float lengthAddition = data.Dimensions.AtmosphereRadius * _lenthAdditionFactor;
            float bledDistance = data.Dimensions.AtmosphereRadius * _canyonsBlendDistanceFactor;
            float length = distanceVector.magnitude + lengthAddition;
            float breadth = maxBreadth * weight;
            float blendValue = bledDistance * weight;
            float max = Mathf.Max(length, breadth);
            float min = Mathf.Min(length, breadth);
            return new ElipsoidShapingPrimitive(pos, distanceVector, min, max, blendValue, weight);
        }

		private ShapingPrimitive CreateMountainPrimitive(PlanetData data, Vector3 distanceVector, float weight, Vector3 pos)
        {
            float maxBreadth = data.Dimensions.AtmosphereRadius * _mountainsBreadthFactor;
            float lengthAddition = data.Dimensions.AtmosphereRadius * _lenthAdditionFactor;
            float bledDistance = data.Dimensions.AtmosphereRadius * _mountainsBlendDistanceFactor;
            float mountainLength = distanceVector.magnitude + lengthAddition;
            float mountainBreadth = maxBreadth * weight;
            float blendValue = bledDistance * weight;
            float max = Mathf.Max(mountainLength, mountainBreadth);
            float min = Mathf.Min(mountainLength, mountainBreadth);
            return new ElipsoidShapingPrimitive(pos, distanceVector, min, max, blendValue, weight);
        }

        private float CalculateMountainWeight(ContinentalPlates plates, Vector2Int neighbors)
        {
            ContinentalPlate plate0 = plates.Plates[neighbors[0]];
            ContinentalPlate plate1 = plates.Plates[neighbors[1]];
            Vector3 movementTangent0 = plate0.MovementTangent;
            Vector3 movementTangent1 = plate1.MovementTangent;
            Vector3 distanceVector0 = plate1.Site - plate0.Site;
            Vector3 distanceVector1 = plate0.Site - plate1.Site;
            float angle0 = Vector3.Angle(movementTangent0, distanceVector0);
            float angle1 = Vector3.Angle(movementTangent1, distanceVector1);
            bool movingTowards0 = angle0 < 90;
            bool movingToWards1 = angle1 < 90;
            if (movingTowards0 && movingToWards1)
                return (plate0.MovementStrength + plate1.MovementStrength) / 2;
            if (!movingTowards0 && !movingToWards1)
                return -(plate0.MovementStrength + plate1.MovementStrength) / 2;
            if(movingTowards0 && !movingToWards1)
                return plate0.MovementStrength / 2;
            return plate1.MovementStrength / 2;
        }

        public class Parameter
		{
            public Parameter(
                PlanetData data, 
                Noise3D mountainNoise,
                Noise3D canyonsNoise,
                Noise3D oceanNoise,
                Noise3D continentNoise,
                Noise3D seaLevelNoise)
			{
				Data = data;
				MountainNoise = mountainNoise;
				CanyonsNoise = canyonsNoise;
				OceanNoise = oceanNoise;
                ContinentNoise = continentNoise;
				SeaLevelNoise = seaLevelNoise;
			}

			public PlanetData Data { get; }
			public Noise3D MountainNoise { get; }
			public Noise3D CanyonsNoise { get; }
			public Noise3D OceanNoise { get; }
            public Noise3D ContinentNoise { get; }
			public Noise3D SeaLevelNoise { get; }
		}
    }
}