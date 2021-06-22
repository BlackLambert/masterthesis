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

		public float Evaluate2D(Vector2 point)
		{
			return InvertValue(_bollowNoise.Evaluate2D(point));
		}

		public float[] Evaluate2D(Vector2[] points)
		{
			return ApplyNoise(_bollowNoise.Evaluate2D(points));
		}

		public float Evaluate3D(Vector3 point)
		{
			return InvertValue(_bollowNoise.Evaluate3D(point));
		}

		public float[] Evaluate3D(Vector3[] points)
		{
			return ApplyNoise(_bollowNoise.Evaluate3D(points));
		}

		private float[] ApplyNoise(float[] evaluatedValue)
		{
			for (int i = 0; i < evaluatedValue.Length; i++)
				evaluatedValue[i] = InvertValue(evaluatedValue[i]);
			return evaluatedValue;
		}

		private float InvertValue(float billowValue)
		{
			return billowValue * (-1) + 1;
		}
	}
}