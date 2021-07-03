using Unity.Collections;
using UnityEngine;

namespace SBaier.Master
{
	public class RidgedNoise : NoiseBase, Noise3D
	{
		private Noise3D _baseNoise;

		public NoiseType NoiseType => NoiseType.Ridged;

		public RidgedNoise(Noise3D baseNoise)
		{
			_baseNoise = baseNoise;
		}

		public float Evaluate2D(Vector2 point)
		{
			return InvertValue(_baseNoise.Evaluate2D(ApplyFrequencyFactor2D(point)));
		}

		public float Evaluate3D(Vector3 point)
		{
			return InvertValue(_baseNoise.Evaluate3D(ApplyFrequencyFactor3D(point)));
		}

		public NativeArray<float> Evaluate3D(NativeArray<Vector3> points)
		{
			ApplyFrequencyFactor(points);
			return ApplyNoise(_baseNoise.Evaluate3D(points));
		}

		public NativeArray<float> Evaluate2D(NativeArray<Vector2> points)
		{
			ApplyFrequencyFactor(points);
			return ApplyNoise(_baseNoise.Evaluate2D(points));
		}

		private NativeArray<float> ApplyNoise(NativeArray<float> evaluatedValue)
		{
			int evaluatedValueLength = evaluatedValue.Length;
			for (int i = 0; i < evaluatedValueLength; i++)
				evaluatedValue[i] = InvertValue(evaluatedValue[i]) * Weight;
			return evaluatedValue;
		}

		private float InvertValue(float billowValue)
		{
			return billowValue * (-1) + 1;
		}
	}
}