using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace SBaier.Master
{
	public class OctaveNoise : Noise3D
	{
		private const int _fillBaseEvaluationPointsInnerloopBatchCount = 64;

		public int OctavesCount { get; }
		public Noise3D BaseNoise { get; }
		public float StartFrequency { get; }
		public float StartWeight { get; }

		public NoiseType NoiseType => NoiseType.Octave;


		public OctaveNoise(Arguments args)
		{
			OctavesCount = args.OctavesCount;
			BaseNoise = args.BaseNoise;
			StartFrequency = args.StartFrequency;
			StartWeight = args.StartWeight;
		}

		public NativeArray<float> Evaluate2D(NativeArray<Vector2> points)
		{
			NativeArray<Vector2> baseEvaluationPoints = FillBaseEvaluationPoints(points);
			NativeArray<float> baseValues = BaseNoise.Evaluate2D(baseEvaluationPoints);
			NativeArray<float> result = CalculateResult(baseValues);
			baseEvaluationPoints.Dispose();
			baseValues.Dispose();
			return result;
		}

		public NativeArray<float> Evaluate3D(NativeArray<Vector3> points)
		{
			//float time = Time.realtimeSinceStartup;
			NativeArray<Vector3> baseEvaluationPoints = FillBaseEvaluationPoints(points);
			//Debug.Log($"FillBaseEvaluationPoints {Time.realtimeSinceStartup - time}");
			//time = Time.realtimeSinceStartup;
			NativeArray<float> baseValues = BaseNoise.Evaluate3D(baseEvaluationPoints);
			//Debug.Log($"Evaluate3DBaseValues {Time.realtimeSinceStartup - time}");
			//time = Time.realtimeSinceStartup;
			NativeArray<float> result = CalculateResult(baseValues);
			//Debug.Log($"CalculateResult {Time.realtimeSinceStartup - time}");
			baseEvaluationPoints.Dispose();
			baseValues.Dispose();
			return result;
		}

		public float Evaluate3D(Vector3 point)
		{
			NativeArray<Vector3> points = new NativeArray<Vector3>(1, Allocator.TempJob);
			points[0] = point;
			NativeArray<float> results = Evaluate3D(points);
			float result = results[0];
			results.Dispose();
			points.Dispose();
			return result;
		}

		public float Evaluate2D(Vector2 point)
		{
			NativeArray<Vector2> points = new NativeArray<Vector2>(1, Allocator.TempJob);
			points[0] = point;
			NativeArray<float> results = Evaluate2D(points);
			float result = results[0];
			results.Dispose();
			points.Dispose();
			return result;
		}

		private NativeArray<float> CalculateResult(NativeArray<float> baseValues)
		{
			int pointsCount = baseValues.Length / OctavesCount;
			NativeArray<float> nativeResult = new NativeArray<float>(pointsCount, Allocator.TempJob);
			NativeArray<float> weights = new NativeArray<float>(GetWeights(), Allocator.TempJob);

			CalculateResultJob job = new CalculateResultJob()
			{
				_baseValues = baseValues,
				_result = nativeResult,
				_weights = weights,
				_octavesAmount = OctavesCount
			};

			job.Schedule(pointsCount, _fillBaseEvaluationPointsInnerloopBatchCount).Complete();
			weights.Dispose();
			return nativeResult;
		}

		private NativeArray<Vector3> FillBaseEvaluationPoints(NativeArray<Vector3> points)
		{
			NativeArray<Vector3> nativeResult = new NativeArray<Vector3>(points.Length * OctavesCount, Allocator.TempJob);
			NativeArray<float> ffs = new NativeArray<float>(GetFrequencyFactors(), Allocator.TempJob);

			FillBaseEvaluationPoints3DJob job = new FillBaseEvaluationPoints3DJob()
			{
				_formerPoints = points,
				_frequencyFactors = ffs,
				_octavesAmount = OctavesCount,
				_points = nativeResult
			};

			job.Schedule(nativeResult.Length, _fillBaseEvaluationPointsInnerloopBatchCount).Complete();
			ffs.Dispose();
			return nativeResult;
		}

		private NativeArray<Vector2> FillBaseEvaluationPoints(NativeArray<Vector2> points)
		{
			NativeArray<Vector2> nativeResult = new NativeArray<Vector2>(points.Length * OctavesCount, Allocator.TempJob);
			NativeArray<float> ffs = new NativeArray<float>(GetFrequencyFactors(), Allocator.TempJob);

			FillBaseEvaluationPoints2DJob job = new FillBaseEvaluationPoints2DJob()
			{
				_formerPoints = points,
				_frequencyFactors = ffs,
				_octavesAmount = OctavesCount,
				_points = nativeResult
			};

			job.Schedule(nativeResult.Length, _fillBaseEvaluationPointsInnerloopBatchCount).Complete();
			ffs.Dispose();
			return nativeResult;
		}

		private float[] GetFrequencyFactors()
		{
			float[] result = new float[OctavesCount];
			for(int i = 0; i < result.Length; i++)
				result [i] = StartFrequency * Mathf.Pow(2, i);
			return result;
		}

		private float[] GetWeights()
		{
			float[] result = new float[OctavesCount];
			for(int i = 0; i < result.Length; i++)
				result [i] = StartWeight / Mathf.Pow(2, i);
			return result;
		}

		public float EvaluateOctaves(Func<float, float> baseEvaluation)
		{
			float result = 0;
			for (int octave = 0; octave < OctavesCount; octave++)
				result += EvaluateOctave(octave, baseEvaluation);
			return Clamp01(result + 0.5f);
		}

		private float EvaluateOctave(int octave, Func<float, float> baseEvaluation)
		{
			float factor = Mathf.Pow(2, octave);
			float ff = StartFrequency * factor;
			float weight = StartWeight / factor;
			return (baseEvaluation(ff) - 0.5f) * weight;
		}

		private float Clamp01(float result)
		{
			return (result > 1) ? 1 : (result < 0) ? 0 : result;
		}

		public class Arguments
		{
			public int OctavesCount { get; }
			public Noise3D BaseNoise { get; }
			public float StartFrequency { get; }
			public float StartWeight { get; }


			public Arguments(int octavesCount, Noise3D baseNoise, float startFrequency, float startWeight)
			{
				CheckStartWeightOutOfRange(startWeight);
				CheckStartFrequencyOutOfRange(startFrequency);

				OctavesCount = octavesCount;
				BaseNoise = baseNoise;
				StartFrequency = startFrequency;
				StartWeight = startWeight;
			}

			private void CheckStartWeightOutOfRange(float startWeight)
			{
				if (startWeight < 0)
					throw new ArgumentOutOfRangeException();
			}

			private void CheckStartFrequencyOutOfRange(float startFrequency)
			{
				if (startFrequency < 0)
					throw new ArgumentOutOfRangeException();
			}
		}

		[BurstCompile]
		public struct FillBaseEvaluationPoints3DJob : IJobParallelFor
		{
			[WriteOnly]
			public NativeArray<Vector3> _points;
			[ReadOnly]
			public NativeArray<Vector3> _formerPoints;
			[ReadOnly]
			public int _octavesAmount;
			[ReadOnly]
			public NativeArray<float> _frequencyFactors;

			public void Execute(int index)
			{
				int ovtave = index / _formerPoints.Length;
				_points[index] = _formerPoints[index % _formerPoints.Length] * _frequencyFactors[ovtave];
			}
		}

		[BurstCompile]
		public struct FillBaseEvaluationPoints2DJob : IJobParallelFor
		{
			[WriteOnly]
			public NativeArray<Vector2> _points;
			[ReadOnly]
			public NativeArray<Vector2> _formerPoints;
			[ReadOnly]
			public int _octavesAmount;
			[ReadOnly]
			public NativeArray<float> _frequencyFactors;

			public void Execute(int index)
			{
				int ovtave = index / _formerPoints.Length;
				_points[index] = _formerPoints[index % _formerPoints.Length] * _frequencyFactors[ovtave];
			}
		}

		[BurstCompile]
		public struct CalculateResultJob : IJobParallelFor
		{
			[WriteOnly]
			public NativeArray<float> _result;
			[ReadOnly]
			public NativeArray<float> _baseValues;
			[ReadOnly]
			public int _octavesAmount;
			[ReadOnly]
			public NativeArray<float> _weights;

			public void Execute(int index)
			{
				float value = 0;
				for (int octave = 0; octave < _octavesAmount; octave++)
				{
					float weight = _weights[octave];
					value += (_baseValues[index + _result.Length * octave] - 0.5f) * weight;
				}

				_result[index] = Clamp01(value + 0.5f);
			}

			private float Clamp01(float result)
			{
				return (result > 1) ? 1 : (result < 0) ? 0 : result;
			}
		}
	}
}