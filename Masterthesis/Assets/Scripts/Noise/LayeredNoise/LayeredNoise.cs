using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace SBaier.Master
{
	public class LayeredNoise : NoiseBase, Noise3D
	{
		public Noise3D[] Layers { get; }
		public MappingMode Mapping { get; }
		public NoiseType NoiseType => NoiseType.Layered;

		public LayeredNoise(Noise3D[] layers, MappingMode mapping)
		{
			Layers = layers;
			Mapping = mapping;
		}

		public NativeArray<float> Evaluate2D(NativeArray<Vector2> points)
		{
			NativeArray<float> result = new NativeArray<float>(points.Length, Allocator.TempJob);
			float mappingValue = GetMappingValue();

			// Base evaluations
			for (int layer = 0; layer < Layers.Length; layer++)
			{
				NativeArray<Vector2> baseEvaluationPoints = new NativeArray<Vector2>(points, Allocator.TempJob);
				ApplyFrequencyFactor(baseEvaluationPoints);

				// Evaluate
				NativeArray<float> baseEvaluations = Layers[layer].Evaluate2D(baseEvaluationPoints);

				// Add weight base value to result
				int baseEvaluationsLength = baseEvaluations.Length;
				for (int i = 0; i < baseEvaluationsLength; i++)
					result[i] += (baseEvaluations[i] - mappingValue);
				baseEvaluationPoints.Dispose();
				baseEvaluations.Dispose();
			}


			// Clamp result values
			int resultLength = result.Length;
			for (int i = 0; i < resultLength; i++)
				result[i] = MathUtil.Clamp01((result[i] + mappingValue) * Weight);

			return result;
		}

		public NativeArray<float> Evaluate3D(NativeArray<Vector3> points)
		{
			NativeArray<float> result = new NativeArray<float>(points.Length, Allocator.TempJob);
			float mappingValue = GetMappingValue();

			// Base evaluations
			for (int layer = 0; layer < Layers.Length; layer++)
			{
				NativeArray<Vector3> baseEvaluationPoints = new NativeArray<Vector3>(points, Allocator.TempJob);
				ApplyFrequencyFactor(baseEvaluationPoints);

				// Evaluate
				NativeArray<float> baseEvaluations = Layers[layer].Evaluate3D(baseEvaluationPoints);

				// Add weight base value to result
				int baseEvaluationsLength = baseEvaluations.Length;
				for (int i = 0; i < baseEvaluationsLength; i++)
					result[i] += (baseEvaluations[i] - mappingValue);
				baseEvaluationPoints.Dispose();
				baseEvaluations.Dispose();
			}

			// Clamp result values
			int resultLength = result.Length;
			for (int i = 0; i < resultLength; i++)
				result[i] = MathUtil.Clamp01((result[i] + mappingValue) * Weight);

			return result;
		}



		public float Evaluate2D(Vector2 point)
		{
			Func<Noise3D, float> baseEvaluation =
				l => l.Evaluate2D(ApplyFrequencyFactor2D(point));
			return ApplyNoise(baseEvaluation);
		}

		public float Evaluate3D(Vector3 point)
		{
			Func<Noise3D, float> baseEvaluation = 
				l => l.Evaluate3D(ApplyFrequencyFactor3D(point));
			return ApplyNoise(baseEvaluation);
		}

		private float ApplyNoise(Func<Noise3D, float> baseEvaluation)
		{
			float mappingValue = GetMappingValue();
			float result = 0;
			foreach (Noise3D layer in Layers)
			{
				float evaluatedValue = baseEvaluation(layer) - mappingValue;
				result += evaluatedValue;
			}
			return MathUtil.Clamp01((result + mappingValue)*Weight);
		}

		private float GetMappingValue()
		{
			return Mapping == MappingMode.ZeroToOne ? 0 : 0.5f;
		}

		public enum MappingMode
		{
			NegOneToOne = 0,
			ZeroToOne = 1
		}
	}
}