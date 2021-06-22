using System;
using UnityEngine;

namespace SBaier.Master
{
	public class NoiseValueLimiter : Noise3D
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

		public float[] Evaluate2D(Vector2[] points)
		{
			return ApplyNoise(BaseNoise.Evaluate2D(points));
		}

		public float[] Evaluate3D(Vector3[] points)
		{
			return ApplyNoise(BaseNoise.Evaluate3D(points));
		}

		public float Evaluate3D(Vector3 point)
		{
			return Limit(BaseNoise.Evaluate3D(point));
		}

		public float Evaluate2D(Vector2 point)
		{
			return Limit(BaseNoise.Evaluate2D(point));
		}


		private float[] ApplyNoise(float[] baseValues)
		{
			for (int i = 0; i < baseValues.Length; i++)
				baseValues[i] = Limit(baseValues[i]);
			return baseValues;
		}

		private float Limit(float baseNoiseValue)
		{
			float result = baseNoiseValue - Limits.x;
			float upperLimit = Limits.y - Limits.x;
			result = result < 0 ? 0 : result > upperLimit ? upperLimit : result;
			return result;
		}

		private void Check(Vector2 limits)
		{
			if (limits.x < 0 || limits.y > 1 || limits.x > limits.y)
				throw new ArgumentOutOfRangeException();
		}
	}
}