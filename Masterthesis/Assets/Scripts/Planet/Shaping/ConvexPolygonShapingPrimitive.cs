using System;
using UnityEngine;

namespace SBaier.Master
{
	public class ConvexPolygonShapingPrimitive : ShapingPrimitive
	{
		private float _epsilon = 10f;
		private readonly float _angleSumMax = 360f;
		private float _maxInsideAngle;

		public override float MaxAreaOfEffect { get; }

		public override float MaxAreaOfEffectSqr { get; }

		private PolygonBody _body;
		private int _polygonIndex;
		private Vector3 _normal;
		private Vector3 _corner0;
		private Vector3 _center;

		private Vector3 _projection;
		private float _blendValue;
		private bool _inside;
		private Vector3[] _corners;
		private Vector3[] _projectedCorners;
		private Vector3[] _edges;

		public ConvexPolygonShapingPrimitive(PolygonBody body, int polygonIndex, Vector3 position, float blendArea, float weight) : 
			base(position, blendArea, weight)
		{
			_body = body;
			_polygonIndex = polygonIndex;
			_corners = _body.GetVertices(_polygonIndex);
			_center = CalculateCenter(_corners);
			MaxAreaOfEffect = CalculateMaxAreaOfEffect(_corners, blendArea);
			MaxAreaOfEffectSqr = MaxAreaOfEffect * MaxAreaOfEffect;
			_corner0 = _body.GetVertex(_body.GetPolygon( _polygonIndex).VertexIndices[0]); 
			_normal = _body.GetNormal(_polygonIndex);
			_projectedCorners = GetProjectedCorners(_corners);
			_edges = GetEdges(_projectedCorners);
			_maxInsideAngle = _angleSumMax - _epsilon;
		}

		private Vector3[] GetProjectedCorners(Vector3[] corners)
		{
			Vector3[] result = new Vector3[corners.Length];
			for (int i = 0; i < corners.Length; i++)
				result[i] = GetProjectionOnPolygonPlane(corners[i]);
			return result;
		}

		private Vector3[] GetEdges(Vector3[] projectedCorners)
		{
			Vector3[] edges = new Vector3[projectedCorners.Length];
			for (int i = 0; i < projectedCorners.Length; i++)
			{
				Vector3 c0 = _projectedCorners[i];
				Vector3 c1 = _projectedCorners[(i + 1) % _projectedCorners.Length];
				Vector3 edge = c1.FastSubstract(c0);
				edges[i] = edge;
			}
			return edges;
		}

		protected override void InitEvaluation(Vector3 point)
		{
			_projection = GetProjectionOnPolygonPlane(point);
			_inside = CalculateInside(_projection);
		}

		protected override bool IsOutsideAreaOfEffect(Vector3 point)
		{
			_blendValue = CalculateBlendValue(_projection);
			return !_inside && _blendValue > 1;
		}

		protected override float GetBlendedValue(Vector3 point)
		{
			return 1 - _blendValue;
		}

		protected override bool IsInsideKernel(Vector3 point)
		{
			return _inside;
		}

		private float CalculateBlendValue(Vector3 point)
		{
			float distanceToBorder = _body.GetDistanceTo(point, _polygonIndex);
			return distanceToBorder / BlendArea;
		}

		private Vector3 GetProjectionOnPolygonPlane(Vector3 point)
		{
			Vector3 distanceToOrigin = point.FastSubstract(_corner0);
			float dot = Vector3.Dot(distanceToOrigin, _normal);
			return point.FastSubstract(_normal.FastMultiply(dot));
		}

		private Vector3 CalculateCenter(Vector3[] corners)
		{
			Vector3 sum = Vector3.zero;
			for (int i = 0; i < corners.Length; i++)
				sum += corners[i];
			return sum / corners.Length;
		}

		private float CalculateMaxAreaOfEffect(Vector3[] corners, float blendArea)
		{
			float maxCornerDistance = 0;
			for (int i = 0; i < corners.Length; i++)
			{
				Vector3 corner = corners[i];
				float centerDistance = (corner - _center).magnitude;
				if (centerDistance > maxCornerDistance)
					maxCornerDistance = centerDistance;
			}
			return (maxCornerDistance + blendArea);
		}

		private bool CalculateInside(Vector3 projection)
		{
			float angleSum = 0;
			for (int i = 0; i < _projectedCorners.Length; i++)
			{
				Vector3 c0 = _projectedCorners[i].FastSubstract(projection);
				Vector3 c1 = _projectedCorners[(i + 1) % _projectedCorners.Length].FastSubstract(projection);
				float angle = Vector3.Angle(c0, c1);
				angleSum += angle;
			}

			return angleSum >= _maxInsideAngle;
		}
	}
}