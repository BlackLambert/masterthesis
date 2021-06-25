using UnityEngine;

namespace SBaier.Master
{
	public class RidgedNoise : Noise3D
	{
		private Noise3D _baseNoise;

		public NoiseType NoiseType => NoiseType.Ridged;

		public RidgedNoise(Noise3D baseNoise)
		{
			_baseNoise = baseNoise;
		}

		public float Evaluate2D(Vector2 point)
		{
			return InvertValue(_baseNoise.Evaluate2D(point));
		}

		public float[] Evaluate2D(Vector2[] points)
		{
			return ApplyNoise(_baseNoise.Evaluate2D(points));
		}

		public float Evaluate3D(Vector3 point)
		{
			return InvertValue(_baseNoise.Evaluate3D(point));
		}

		public float[] Evaluate3D(Vector3[] points)
		{
			return ApplyNoise(_baseNoise.Evaluate3D(points));
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