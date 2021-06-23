using System;
using UnityEngine;

namespace SBaier.Master
{
	public class StaticValueNoise : Noise3D
	{
		public float Value { get; }

		public NoiseType NoiseType => NoiseType.Static;

		public StaticValueNoise(float value)
		{
			CheckAmount(value);
			Value = value;
		}

		public float Evaluate3D(Vector3 point)
		{
			return Value;
		}

		public float[] Evaluate3D(Vector3[] points)
		{
			return CreateResult(points.Length);
		}

		public float Evaluate2D(Vector2 point)
		{
			return Value;
		}

		public float[] Evaluate2D(Vector2[] points)
		{
			return CreateResult(points.Length);
		}

		private float[] CreateResult(int amount)
		{
			float[] result = new float[amount];
			for (int i = 0; i < result.Length; i++)
				result[i] = Value;
			return result;
		}

		private void CheckAmount(float value)
		{
			if (value < 0 || value > 1)
				throw new ArgumentOutOfRangeException();
		}
	}
}