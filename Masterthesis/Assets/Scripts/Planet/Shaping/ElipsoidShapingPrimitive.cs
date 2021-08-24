using System;
using Unity.Collections;
using UnityEngine;

namespace SBaier.Master
{
	public class ElipsoidShapingPrimitive : ShapingPrimitive
	{
		private const int _blendAreaFactor = 3;
		private Vector3 _kernelFocus0;
		private Vector3 _kernelFocus1;
		private float _kernelFocusDistance;
		private float _areaOfEffectFocusDistance;
		private float _distanceDelta;
		private Vector3 _normal;

		private float _focusDistance;

		public override float MaxAreaOfEffect { get; }

		public ElipsoidShapingPrimitive(Vector3 position, Vector3 stretchDirection, float min, float max, float blendArea, float weight) : 
			base(position, blendArea, weight)
		{
			ValidateMinMax(min, max);
			ValidateStretchDirection(stretchDirection);
			Init(position, stretchDirection, min, max, blendArea);
			MaxAreaOfEffect = max / 2;
			
		}

		private void ValidateStretchDirection(Vector3 stretchDirection)
		{
			if (stretchDirection.sqrMagnitude == 0)
				throw new ArgumentException();
		}

		private void ValidateMinMax(float min, float max)
		{
			if (min > max || min < 0)
				throw new ArgumentException();
		}

		private void Init(Vector3 position, Vector3 stretchDirection, float min, float max, float blendArea)
		{
			Vector3[] foci = CalculateFocusPoints(position, stretchDirection, min, max);
			_kernelFocus0 = foci[0];
			_kernelFocus1 = foci[1];
			_kernelFocusDistance = CalculateFocusDistance(_kernelFocus0, position, min);
			_areaOfEffectFocusDistance = CalculateFocusDistance(_kernelFocus0, position, min + blendArea * _blendAreaFactor);
			_distanceDelta = _areaOfEffectFocusDistance - _kernelFocusDistance;
			_normal = CreateNormal();
		}

		private Vector3 CreateNormal()
		{
			return (_kernelFocus0.normalized + _kernelFocus1.normalized) / 2;
		}

		private float CalculateFocusDistance(Vector3 focus0, Vector3 position, float min)
		{
			float distanceToCenter = (focus0 - position).magnitude;
			float result = Mathf.Sqrt(distanceToCenter * distanceToCenter + min * min) * 2;
			return result;
		}

		private Vector3[] CalculateFocusPoints(Vector3 position, Vector3 stretchDirection, float min, float max)
		{
			Vector3 normalizedStretchDirection = stretchDirection.normalized;
			float maxSemiAxis = max / 2;
			float minSemiAxis = min / 2;
			float focusDistanceToCenter = Mathf.Sqrt(maxSemiAxis * maxSemiAxis - minSemiAxis * minSemiAxis);
			Vector3 focus0 = position + normalizedStretchDirection * focusDistanceToCenter;
			Vector3 focus1 = position - normalizedStretchDirection * focusDistanceToCenter;
			return new Vector3[] { focus0 , focus1};
		}

		protected override void InitEvaluation(Vector3 point)
		{
			_focusDistance = GetFocusDistance(point);
		}

		protected override float GetBlendedValue(Vector3 point)
		{
			float kernelDelta = _focusDistance - _kernelFocusDistance;
			float result = 1 - kernelDelta / _distanceDelta;
			result *= result;
			return result;
		}

		protected override bool IsInsideKernel(Vector3 point)
		{
			return _focusDistance <= _kernelFocusDistance;
		}

		protected override bool IsOutsideAreaOfEffect(Vector3 point)
		{
			return _focusDistance > _areaOfEffectFocusDistance;
		}

		private float GetFocusDistance(Vector3 point)
		{
			point = GetProjectionOnPlane(point);
			Vector3 focusDistance0 = point.FastSubstract(_kernelFocus0);
			Vector3 focusDistance1 = point.FastSubstract(_kernelFocus1);
			float distanceSum = focusDistance0.magnitude + focusDistance1.magnitude;
			return distanceSum;
		}

		private Vector3 GetProjectionOnPlane(Vector3 point)
		{
			Vector3 distanceToOrigin = point.FastSubstract(_kernelFocus0);
			float dot = Vector3.Dot(distanceToOrigin, _normal);
			return point.FastSubstract(_normal.FastMultiply(dot));
		}
	}
}