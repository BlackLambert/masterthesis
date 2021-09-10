using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace SBaier.Master
{
	public class SphericalShapingPrimitive : ShapingPrimitive
	{
		private float _squaredKernalRadius;
		private float _effectRadius;
		private float _squaredEffectRadius;
		public float KernelRadius { get; }

		private float _squaredDistance;

		public override float MaxAreaOfEffect { get; }

		public override float MaxAreaOfEffectSqr { get; }

		public SphericalShapingPrimitive(Vector3 position, float kernelRadius, float blendArea, float weight) : 
			base(position, blendArea, weight)
		{
			KernelRadius = kernelRadius;
			_effectRadius = KernelRadius + blendArea;
			_squaredKernalRadius = kernelRadius * kernelRadius;
			_squaredEffectRadius = _effectRadius * _effectRadius;
			MaxAreaOfEffect = _effectRadius;
			MaxAreaOfEffectSqr = _effectRadius * _effectRadius;
		}

		protected override void InitEvaluation(Vector3 point)
		{
			_squaredDistance = (Position - point).sqrMagnitude;
		}

		protected override bool IsOutsideAreaOfEffect(Vector3 point)
		{
			return _squaredDistance > _squaredEffectRadius;
		}

		protected override bool IsInsideKernel(Vector3 point)
		{
			return _squaredDistance <= _squaredKernalRadius;
		}

		protected override float GetBlendedValue(Vector3 point)
		{
			float distance = Mathf.Sqrt(_squaredDistance);
			float blendDistance = distance - KernelRadius;
			float portion = 1 - (blendDistance / BlendArea);
			return portion;
		}
	}
}