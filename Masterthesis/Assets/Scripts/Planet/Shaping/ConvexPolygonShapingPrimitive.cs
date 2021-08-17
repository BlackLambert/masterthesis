using System;
using UnityEngine;

namespace SBaier.Master
{
	public class ConvexPolygonShapingPrimitive : ShapingPrimitive
	{
		private float _epsilon = 1f;
		private readonly float _angleSumMax = 360f;

		public override float MaxAreaOfEffect { get; }
		private PolygonBody _body;
		private int _polygonIndex;
		private Vector3 _normal;
		private Vector3 _corner0;
		private Vector3 _center;
		private KDTree<Vector3> _polygonKDTree;

		private Vector3 _projection;
		private float _blendValue;
		private bool _inside;
		private Vector3[] _corners;
		private int _nearest;

		public ConvexPolygonShapingPrimitive(PolygonBody body, KDTree<Vector3> polygonKDTree, int polygonIndex, Vector3 position, float blendArea, float weight) : 
			base(position, blendArea, weight)
		{
			_body = body;
			_polygonIndex = polygonIndex;
			Vector3[] corners = _body.GetVertices(_polygonIndex);
			_center = CalculateCenter(corners);
			MaxAreaOfEffect = CalculateMaxAreaOfEffect(corners, blendArea);
			_corner0 = _body.GetVertex(_body.GetPolygon( _polygonIndex).VertexIndices[0]); 
			_normal = _body.GetNormal(_polygonIndex);
			_polygonKDTree = polygonKDTree;
			_corners = _body.GetVertices(_polygonIndex);
		}

		protected override void InitEvaluation(Vector3 point)
		{
			_projection = GetProjectionOnPolygonPlane(point);
			_nearest = _polygonKDTree.GetNearestTo(_projection);
			_blendValue = CalculateBlendValue(_projection);
			_inside = CalculateInside(_projection);
		}

		protected override bool IsOutsideAreaOfEffect(Vector3 point)
		{
			return _nearest != _polygonIndex && GetBlendedValue(point) > 1;
			//return !_inside && _blendValue > 1;
		}

		protected override float GetBlendedValue(Vector3 point)
		{
			return 1 - _blendValue;
		}

		protected override bool IsInsideKernel(Vector3 point)
		{
			return _nearest == _polygonIndex;
			//return _inside;
		}

		private float CalculateBlendValue(Vector3 point)
		{
			float distanceToBorder = _body.GetDistanceTo(point, _polygonIndex);
			return distanceToBorder / BlendArea;
		}

		private Vector3 GetProjectionOnPolygonPlane(Vector3 point)
		{
			Vector3 distanceToOrigin = point - _corner0;
			float dot = Vector3.Dot(distanceToOrigin, _normal);
			return point - dot * _normal;
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
			for (int i = 0; i < _corners.Length; i++)
			{
				Vector3 c0 = _corners[i].FastSubstract(projection);
				Vector3 c1 = _corners[(i + 1) % _corners.Length].FastSubstract(projection);
				float angle = Vector3.Angle(c0, c1);
				angleSum += angle;
			}

			return angleSum >= _angleSumMax - _epsilon;
		}
	}
}