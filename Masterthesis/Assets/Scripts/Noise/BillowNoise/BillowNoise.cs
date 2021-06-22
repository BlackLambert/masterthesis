using System;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using Unity.Burst;

namespace SBaier.Master
{
	public class BillowNoise : Noise3D
	{
		private const int _innerloopBatchCount = 1;

		public Noise3D BaseNoise { get; }

		public NoiseType NoiseType => NoiseType.Billow;

		public BillowNoise(Noise3D baseNoise)
		{
			BaseNoise = baseNoise;
		}

		public float[] Evaluate3D(Vector3[] points)
		{
			return ApplyNoise(BaseNoise.Evaluate3D(points));
		}

		public float Evaluate3D(Vector3 point)
		{
			return ApplyNoise(BaseNoise.Evaluate3D(point));
		}

		public float[] Evaluate2D(Vector2[] points)
		{
			return ApplyNoise(BaseNoise.Evaluate2D(points));
		}

		public float Evaluate2D(Vector2 point)
		{
			return ApplyNoise(BaseNoise.Evaluate2D(point));
		}

		private static float[] ApplyNoise(float[] evaluatedValues)
		{
			return ApplyNoiseParallel(evaluatedValues);
		}

		private static float[] ApplyNoiseSequencial(float[] evaluatedValues)
		{
			for (int i = 0; i < evaluatedValues.Length; i++)
				evaluatedValues[i] = ApplyNoise(evaluatedValues[i]);
			return evaluatedValues;
		}

		private static float[] ApplyNoiseParallel(float[] evaluatedValues)
		{
			NativeArray<float> result = new NativeArray<float>(evaluatedValues, Allocator.TempJob);
			EvaluateJob job = new EvaluateJob { _result = result };
			job.Schedule(result.Length, _innerloopBatchCount).Complete();
			evaluatedValues = result.ToArray();
			result.Dispose();
			return evaluatedValues;
		}

		private static float ApplyNoise(float baseValue)
		{
			float stretchedValue = StretchToNegativeValueRange(baseValue);
			return Mathf.Abs(stretchedValue);
		}

		private static float StretchToNegativeValueRange(float baseValue)
		{
			return baseValue * 2 - 1;
		}

		[BurstCompile]
		public struct EvaluateJob : IJobParallelFor
		{
			public NativeArray<float> _result;

			public void Execute(int index)
			{
				_result[index] = Math.Abs(_result[index] * 2 - 1);
			}
		}
	}
}