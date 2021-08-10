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
        private const float _lenthAdditionFactor = 0.05f;

        private Vector3BinaryKDTreeFactory _kdTreeFactory;

        public ShapingFactory(Vector3BinaryKDTreeFactory kdTreeFactory)
        {
            _kdTreeFactory = kdTreeFactory;
        }


        public ShapingLayer[] Create(Parameter parameter)
        {
            List<ShapingLayer> result = new List<ShapingLayer>();
            ShapingLayer[] continentsBorderShaping = CreateBorderShaping(parameter);
            result.AddRange(continentsBorderShaping);
            return result.ToArray();
        }

		private ShapingLayer CreateShapingLayer(ShapingPrimitive[] primitives, Noise3D noise)
        {
            Vector3[] nodes = primitives.Select(p => p.Position).ToArray();
            KDTree<Vector3> tree = _kdTreeFactory.Create(nodes);
            return new ShapingLayer(primitives, tree, noise);
        }

        private ShapingLayer[] CreateBorderShaping(Parameter parameter)
        {
            ContinentalPlates plates = parameter.Plates;
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

            List<ShapingLayer> layers = new List<ShapingLayer>();
            if(mountainPrimitives.Count > 0)
                layers.Add(CreateShapingLayer(mountainPrimitives.ToArray(), parameter.MountainNoise));
            if (canyonPrimitives.Count > 0)
                layers.Add(CreateShapingLayer(canyonPrimitives.ToArray(), parameter.CanyonsNoise));
            return layers.ToArray();
        }

		private ShapingPrimitive CreateCanyonsPrimitive(PlanetData data, Vector3 distanceVector, float weight, Vector3 pos)
        {
            float maxBreadth = data.Dimensions.ThicknessRadius * _canyonsBreadthFactor;
            float lengthAddition = data.Dimensions.ThicknessRadius * _lenthAdditionFactor;
            float bledDistance = data.Dimensions.ThicknessRadius * _canyonsBlendDistanceFactor;
            float length = distanceVector.magnitude + lengthAddition;
            float breadth = maxBreadth * weight;
            float blendValue = bledDistance * weight;
            float max = Mathf.Max(length, breadth);
            float min = Mathf.Min(length, breadth);
            return new ElipsoidShapePrimitive(pos, distanceVector, min, max, blendValue, weight);
        }

		private ShapingPrimitive CreateMountainPrimitive(PlanetData data, Vector3 distanceVector, float weight, Vector3 pos)
        {
            float maxBreadth = data.Dimensions.ThicknessRadius * _mountainsBreadthFactor;
            float lengthAddition = data.Dimensions.ThicknessRadius * _lenthAdditionFactor;
            float bledDistance = data.Dimensions.ThicknessRadius * _mountainsBlendDistanceFactor;
            float mountainLength = distanceVector.magnitude + lengthAddition;
            float mountainBreadth = maxBreadth * weight;
            float blendValue = bledDistance * weight;
            float max = Mathf.Max(mountainLength, mountainBreadth);
            float min = Mathf.Min(mountainLength, mountainBreadth);
            return new ElipsoidShapePrimitive(pos, distanceVector, min, max, blendValue, weight);
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
            public Parameter(ContinentalPlates plates, 
                PlanetData data, 
                Noise3D mountainNoise,
                Noise3D canyonsNoise)
			{
				Plates = plates;
				Data = data;
				MountainNoise = mountainNoise;
				CanyonsNoise = canyonsNoise;
			}

			public ContinentalPlates Plates { get; }
			public PlanetData Data { get; }
			public Noise3D MountainNoise { get; }
			public Noise3D CanyonsNoise { get; }
		}
    }
}