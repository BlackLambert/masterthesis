using System;
using UnityEngine;

namespace SBaier.Master
{
	public class BillowNoise : Noise3D
	{
		public Noise3D BaseNoise { get; }

		public NoiseType NoiseType => NoiseType.Billow;

		public BillowNoise(Noise3D baseNoise)
		{
			BaseNoise = baseNoise;
		}

		public double[] Evaluate(Vector3[] points)
		{
			return ApplyNoise(BaseNoise.Evaluate(points));
		}

		public double[] Evaluate(Vector2[] points)
		{
			return ApplyNoise(BaseNoise.Evaluate(points));
		}

		public double Evaluate(double x, double y)
		{
			return ApplyNoise(BaseNoise.Evaluate(x, y));
		}

		public double Evaluate(double x, double y, double z)
		{
			return ApplyNoise(BaseNoise.Evaluate(x, y, z));
		}

		private static double[] ApplyNoise(double[] evaluatedValue)
		{
			for (int i = 0; i < evaluatedValue.Length; i++)
				evaluatedValue[i] = ApplyNoise(evaluatedValue[i]);
			return evaluatedValue;
		}

		private static double ApplyNoise(double baseValue)
		{
			double stretchedValue = StretchToNegativeValueRange(baseValue);
			return Math.Abs(stretchedValue);
		}

		private static double StretchToNegativeValueRange(double perlinValue)
		{
			return perlinValue * 2 - 1;
		}
	}
}