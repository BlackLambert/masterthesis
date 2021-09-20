using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
	public class PlatesShapingLayerFactory : ShapingLayerFactory
	{
		private const float _lenthAdditionFactor = 0.02f;
		private const ShapingLayer.Mode _mountainShapingMode = ShapingLayer.Mode.Add;
		private const ShapingLayer.Mode _canyonShapingMode = ShapingLayer.Mode.Add;
		private const float _maxRelativeSize = 0.25f;
		private List<ShapingPrimitive> _mountainPrimitives;
		private List<ShapingPrimitive> _canyonPrimitives;
		private HashSet<Vector2Int> _handledBorders;
		private PlanetData _data;

		public PlatesShapingLayerFactory() : base()
		{
			_mountainPrimitives = new List<ShapingPrimitive>();
			_canyonPrimitives = new List<ShapingPrimitive>();
			_handledBorders = new HashSet<Vector2Int>();
		}

		public ShapingLayer[] Create(Parameter parameter)
		{
			Init(parameter);
			ContinentalPlates plates = _data.ContinentalPlates;
			for (int i = 0; i < plates.PlateNeighbors.Length; i++)
				Create(parameter.ShapingParameter, plates.PlateNeighbors[i]);
			return CreateLayers(parameter);
		}

		private void Init(Parameter parameter)
		{
			_mountainPrimitives.Clear();
			_canyonPrimitives.Clear();
			_handledBorders.Clear();
			_data = parameter.Data;
		}

		private ShapingLayer[] CreateLayers(Parameter parameter)
		{
			ShapingPrimitive[][] primitives = new ShapingPrimitive[][] { _mountainPrimitives.ToArray(), _canyonPrimitives.ToArray() };
			Noise3D[] noise = new Noise3D[] { parameter.MountainNoise, parameter.CanyonsNoise };
			ShapingLayer.Mode[] modes = new ShapingLayer.Mode[] { _mountainShapingMode, _canyonShapingMode };
			return CreateLayers(primitives, noise, modes);
		}

		private void Create(TerrainStructureParameters parameter, Vector2Int neighbors)
		{
			float weight = CalculateWeight(_data.ContinentalPlates, neighbors);
			ContinentalPlates plates = _data.ContinentalPlates;
			Vector2Int[] borders = plates.PlateBorders[neighbors];
			for (int i = 0; i < borders.Length; i++)
				Create(parameter, borders[i], weight);
		}

		private void Create(TerrainStructureParameters parameter, Vector2Int border, float weight)
		{
			bool borderAlreadyHandled = _handledBorders.Contains(border);
			if (borderAlreadyHandled)
				return;
			Vector3 borderVector = CalculateBorderVector(border);
			bool borderLengthIsZero = borderVector.sqrMagnitude == 0;
			if (borderLengthIsZero)
				return;
			Vector3 pos = CalculatePrimitivePosition(border, borderVector);
			CreatePrimitive(parameter, weight, borderVector, pos);
			_handledBorders.Add(border);
		}

		private Vector3 CalculateBorderVector(Vector2Int border)
		{
			ContinentalPlates plates = _data.ContinentalPlates;
			Vector3 corner0 = plates.SegmentCorners[border[0]];
			Vector3 corner1 = plates.SegmentCorners[border[1]];
			return corner1 - corner0;
		}

		private Vector3 CalculatePrimitivePosition(Vector2Int border, Vector3 borderVector)
		{
			ContinentalPlates plates = _data.ContinentalPlates;
			Vector3 corner0 = plates.SegmentCorners[border[0]];
			return (corner0 + borderVector / 2).normalized * corner0.magnitude;
		}

		private void CreatePrimitive(TerrainStructureParameters parameter, float weight, Vector3 borderVector, Vector3 pos)
		{
			if (weight > 0)
				_mountainPrimitives.Add(CreateMountainPrimitive(parameter, borderVector, weight, pos));
			else
				_canyonPrimitives.Add(CreateCanyonsPrimitive(parameter, borderVector, -weight, pos));
		}

		private ShapingPrimitive CreateCanyonsPrimitive(TerrainStructureParameters parameter, Vector3 distanceVector, float weight, Vector3 pos)
		{
			return CreatePrimitive(distanceVector, weight, pos,
				parameter.Canyon.MinBreadth,
				parameter.Canyon.MaxBreadth, 
				parameter.Canyon.MinDepth, 
				parameter.Canyon.MaxDepth,
				parameter.Canyon.Blendvalue);
		}

		private ShapingPrimitive CreateMountainPrimitive(TerrainStructureParameters parameter, Vector3 distanceVector, float weight, Vector3 pos)
		{
			return CreatePrimitive(distanceVector, weight, pos,
				parameter.Mountain.MinBreadth,
				parameter.Mountain.MaxBreadth,
				parameter.Mountain.MinHeight,
				parameter.Mountain.MaxHeight,
				parameter.Mountain.Blendvalue);
		}

		private ShapingPrimitive CreatePrimitive(Vector3 distanceVector, 
			float weight, 
			Vector3 pos,
			float minBreadth,
			float maxBreadth, 
			float minWeight, 
			float maxWeightFactor,
			float blendValue)
		{
			float absoluteMax = _data.Dimensions.HullMaxRadius * _maxRelativeSize;
			float absoluteMaxBreadth = absoluteMax * maxBreadth;
			float lengthAddition = _data.Dimensions.HullMaxRadius * _lenthAdditionFactor;
			float length = distanceVector.magnitude + lengthAddition;
			float clampedWeight = Mathf.Max(minWeight, weight);
			float breadth = absoluteMaxBreadth * Mathf.Max(minBreadth, clampedWeight);
			//breadth = breadth - breadth * blendValue;
			float blendDistance = absoluteMax * blendValue;
			float max = Mathf.Max(length, breadth);
			float min = Mathf.Min(length, breadth);
			return new ElipsoidShapingPrimitive(pos, distanceVector, min, max, blendDistance, clampedWeight * maxWeightFactor);
		}

		private float CalculateWeight(ContinentalPlates plates, Vector2Int neighbors)
		{
			ContinentalPlate plate0 = plates.Plates[neighbors[0]];
			ContinentalPlate plate1 = plates.Plates[neighbors[1]];
			bool movingTowards0 = IsPlateMovingTowards(plate0, plate1);
			bool movingTowards1 = IsPlateMovingTowards(plate1, plate0);
			float weight = 0;
			weight += movingTowards0 ? plate0.MovementStrength / 2 : -plate0.MovementStrength / 2;
			weight += movingTowards1 ? plate1.MovementStrength / 2 : -plate1.MovementStrength / 2;
			return weight;
		}

		private bool IsPlateMovingTowards(ContinentalPlate plate0, ContinentalPlate plate1)
		{
			Vector3 movementTangent = plate0.MovementTangent;
			Vector3 distanceVector = plate1.Site - plate0.Site;
			float angle = Vector3.Angle(movementTangent, distanceVector);
			return angle < 90;
		}

		public class Parameter
		{
			public Parameter(
				PlanetData data,
				TerrainStructureParameters shapingParameter,
				Noise3D mountainNoise,
				Noise3D canyonsNoise)
			{
				Data = data;
				ShapingParameter = shapingParameter;
				MountainNoise = mountainNoise;
				CanyonsNoise = canyonsNoise;
			}

			public PlanetData Data { get; }
			public TerrainStructureParameters ShapingParameter { get; }
			public Noise3D MountainNoise { get; }
			public Noise3D CanyonsNoise { get; }
		}
	}
}