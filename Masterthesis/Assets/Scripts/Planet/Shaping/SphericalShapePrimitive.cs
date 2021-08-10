using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace SBaier.Master
{
	public class SphericalShapePrimitive : ShapingPrimitive
	{
		private float _squaredKernalRadius;
		private float _effectRadius;
		private float _squaredEffectRadius;
		public float KernelRadius { get; }

		public override float MaxAreaOfEffect { get; }

		public SphericalShapePrimitive(Vector3 position, float kernelRadius, float blendArea, float weight) : 
			base(position, blendArea, weight)
		{
			KernelRadius = kernelRadius;
			_effectRadius = KernelRadius + blendArea;
			_squaredKernalRadius = kernelRadius * kernelRadius;
			_squaredEffectRadius = _effectRadius * _effectRadius;
			MaxAreaOfEffect = _effectRadius;
		}

		public override bool IsOutsideAreaOfEffect(Vector3 point)
		{
			float squaredDistance = (Position - point).sqrMagnitude;
			return squaredDistance > _squaredEffectRadius;
		}

		protected override bool IsInsideKernel(Vector3 point)
		{
			float squaredDistance = (Position - point).sqrMagnitude;
			return squaredDistance <= _squaredKernalRadius;
		}

		protected override float GetBlendedValue(Vector3 point)
		{
			float squaredDistance = (Position - point).sqrMagnitude;
			float distance = Mathf.Sqrt(squaredDistance);
			float blendDistance = distance - KernelRadius;
			float portion = 1 - (blendDistance / BlendArea);
			return portion;
		}

		protected override NativeArray<float> Evaluate(NativeArray<Vector3> points)
		{
			throw new System.NotImplementedException();
		}
	}
}