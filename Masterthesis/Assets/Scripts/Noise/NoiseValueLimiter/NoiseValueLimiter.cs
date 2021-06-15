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

		public double Evaluate(double x, double y, double z)
		{
			return Limit(BaseNoise.Evaluate(x, y, z));
		}

		public double Evaluate(double x, double y)
		{
			return Limit(BaseNoise.Evaluate(x, y));
		}

		private double Limit(double baseNoiseValue)
		{
			double result = baseNoiseValue - Limits.x;
			double upperLimit = Limits.y - Limits.x;
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