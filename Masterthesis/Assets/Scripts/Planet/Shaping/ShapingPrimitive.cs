using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace SBaier.Master
{
	public abstract class ShapingPrimitive
	{
		public Vector3 Position { get; }
		public float BlendArea { get; }
		public float Weight { get; }
		public abstract float MaxAreaOfEffect { get; }
		public abstract float MaxAreaOfEffectSqr { get; }

		public ShapingPrimitive(Vector3 position, float blendArea, float weight)
		{
			ValidateWeight(weight);
			ValidateBlendArea(blendArea);
			Position = position;
			BlendArea = blendArea;
			Weight = weight;
		}

		private void ValidateWeight(float weight)
		{
			if (weight < 0 || weight > 1)
				throw new ArgumentOutOfRangeException();
		}

		private void ValidateBlendArea(float blendArea)
		{
			if (blendArea < 0)
				throw new ArgumentException();
		}

		public float Evaluate(Vector3 point)
		{
			InitEvaluation(point);
			if (IsInsideKernel(point))
				return Weight;
			if (IsOutsideAreaOfEffect(point))
				return 0;
			return GetBlendedValue(point) * Weight;
		}

		protected abstract void InitEvaluation(Vector3 point);
		protected abstract float GetBlendedValue(Vector3 point);
		protected abstract bool IsInsideKernel(Vector3 point);
		protected abstract bool IsOutsideAreaOfEffect(Vector3 point);
	}
}