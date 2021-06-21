using UnityEngine;

namespace SBaier.Master
{
	public class RidgedNoise : Noise3D
	{
		private BillowNoise _bollowNoise;

		public NoiseType NoiseType => NoiseType.Ridged;

		public RidgedNoise(BillowNoise billowNoise)
		{
			_bollowNoise = billowNoise;
		}

		public double[] Evaluate(Vector2[] points)
		{
			return ApplyNoise(_bollowNoise.Evaluate(points));
		}

		public double[] Evaluate(Vector3[] points)
		{
			return ApplyNoise(_bollowNoise.Evaluate(points));
		}

		public double Evaluate(double x, double y)
		{
			return InvertValue(_bollowNoise.Evaluate(x, y));
		}

		public double Evaluate(double x, double y, double z)
		{
			return InvertValue(_bollowNoise.Evaluate(x, y, z));
		}

		private double[] ApplyNoise(double[] evaluatedValue)
		{
			for (int i = 0; i < evaluatedValue.Length; i++)
				evaluatedValue[i] = InvertValue(evaluatedValue[i]);
			return evaluatedValue;
		}

		private double InvertValue(double billowValue)
		{
			return billowValue * (-1) + 1;
		}
	}
}