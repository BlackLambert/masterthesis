using System;
using Unity.Collections;
using UnityEngine;

namespace SBaier.Master
{
	public class NoiseValueLimiter : NoiseBase, Noise3D
	{
		public Vector2 Limits { get; }
		public Noise3D BaseNoise { get; }

		public NoiseValueLimiter(Vector2 limits, Noise3D baseNoise)
		{
			Check(limits);

			Limits = limits;
			BaseNoise = baseNoise;
		}

		public NoiseType NoiseType => NoiseType.NoiseValueLimiter;

		public NativeArray<float> Evaluate3D(NativeArray<Vector3> points)
		{
			ApplyFrequencyFactor(points);
			return ApplyNoise(BaseNoise.Evaluate3D(points));
		}

		public NativeArray<float> Evaluate2D(NativeArray<Vector2> points)
		{
			ApplyFrequencyFactor(points);
			return ApplyNoise(BaseNoise.Evaluate2D(points));
		}

		public float Evaluate3D(Vector3 point)
		{
			Vector2 limits = Limits;
			return Limit(BaseNoise.Evaluate3D(ApplyFrequencyFactor3D(point)), limits);
		}

		public float Evaluate2D(Vector2 point)
		{
			Vector2 limits = Limits;
			return Limit(BaseNoise.Evaluate2D(ApplyFrequencyFactor2D(point)), limits);
		}


		private NativeArray<float> ApplyNoise(NativeArray<float> baseValues)
		{
			Vector2 limits = Limits;
			for (int i = 0; i < baseValues.Length; i++)
				baseValues[i] = Limit(baseValues[i], limits);
			return baseValues;
		}

		private float Limit(float baseNoiseValue, Vector2 limits)
		{
			float result = baseNoiseValue - limits.x;
			float upperLimit = limits.y - limits.x;
			result = result < 0 ? 0 : result > upperLimit ? upperLimit : result;
			return MathUtil.Clamp01( result * Weight);
		}

		private void Check(Vector2 limits)
		{
			if (limits.x < 0 || limits.y > 1 || limits.x > limits.y)
				throw new ArgumentOutOfRangeException();
		}
	}
}