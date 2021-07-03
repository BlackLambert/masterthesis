using System;
using Unity.Collections;
using UnityEngine;

namespace SBaier.Master
{
	public class StaticValueNoise : NoiseBase, Noise3D
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
			return GetValue();
		}

		public NativeArray<float> Evaluate3D(NativeArray<Vector3> points)
		{
			return CreateResult(points.Length);
		}

		public float Evaluate2D(Vector2 point)
		{
			return GetValue();
		}

		public NativeArray<float> Evaluate2D(NativeArray<Vector2> points)
		{
			return CreateResult(points.Length);
		}

		private NativeArray<float> CreateResult(int amount)
		{
			NativeArray<float> result = new NativeArray<float>(amount, Allocator.TempJob);
			int resultLength = result.Length;
			for (int i = 0; i < resultLength; i++)
				result[i] = GetValue();
			return result;
		}

		private float GetValue()
		{
			return MathUtil.Clamp01(Value * Weight);
		}

		private void CheckAmount(float value)
		{
			if (value < 0 || value > 1)
				throw new ArgumentOutOfRangeException();
		}
	}
}