using System;
using Unity.Collections;
using UnityEngine;

namespace SBaier.Master
{
	public class ElipsoidShapePrimitive : ShapingPrimitive
	{
		private Vector3 _kernelFocus0;
		private Vector3 _kernelFocus1;
		private float _kernelFocusDistance;
		private float _areaOfEffectFocusDistance;
		private float _distanceDelta;

		public override float MaxAreaOfEffect { get; }

		public ElipsoidShapePrimitive(Vector3 position, Vector3 stretchDirection, float min, float max, float blendArea, float weight) : 
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
			_areaOfEffectFocusDistance = CalculateFocusDistance(_kernelFocus0, position, min + blendArea);
			_distanceDelta = _areaOfEffectFocusDistance - _kernelFocusDistance;
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

		protected override float GetBlendedValue(Vector3 point)
		{
			float kernelDistance = GetFocusDistance(point);
			float kernelDelta = kernelDistance - _kernelFocusDistance;
			float result = Mathf.Pow(1 - kernelDelta / _distanceDelta, 2);
			return result;
		}

		protected override bool IsInsideKernel(Vector3 point)
		{
			float distanceSum = GetFocusDistance(point);
			return distanceSum <= _kernelFocusDistance;
		}

		public override bool IsOutsideAreaOfEffect(Vector3 point)
		{
			float distanceSum = GetFocusDistance(point);
			return distanceSum > _areaOfEffectFocusDistance;
		}

		private float GetFocusDistance(Vector3 point)
		{
			Vector3 focusDistance0 = point.FastSubstract(_kernelFocus0);
			Vector3 focusDistance1 = point.FastSubstract(_kernelFocus1);
			float distanceSum = focusDistance0.magnitude + focusDistance1.magnitude;
			return distanceSum;
		}

		protected override NativeArray<float> Evaluate(NativeArray<Vector3> points)
		{
			throw new NotImplementedException();
		}
	}
}