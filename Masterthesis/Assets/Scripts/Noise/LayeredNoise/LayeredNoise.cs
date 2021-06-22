using System;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
	public class LayeredNoise : Noise3D
	{
		public NoiseLayer[] Layers { get; }
		public MappingMode Mapping { get; }
		public NoiseType NoiseType => NoiseType.Layered;

		public LayeredNoise(List<NoiseLayer> layers, MappingMode mapping)
		{
			Layers = layers.ToArray();
			Mapping = mapping;
		}

		public float[] Evaluate2D(Vector2[] points)
		{
			float[] result = new float[points.Length];
			float mappingValue = GetMappingValue();

			// Base evaluations
			for (int layer = 0; layer < Layers.Length; layer++)
			{
				Vector2[] baseEvaluationPoints = new Vector2[points.Length];
				points.CopyTo(baseEvaluationPoints, 0);
				float ff = Layers[layer].FrequencyFactor;

				// Apply FrequencyFactor to evaluation points
				for (int j = 0; j < baseEvaluationPoints.Length; j++)
					baseEvaluationPoints[j] *= ff;

				// Evaluate
				float[] baseEvaluations = Layers[layer].Noise.Evaluate2D(baseEvaluationPoints);

				// Add weight base value to result
				float weight = Layers[layer].Weight;
				for (int i = 0; i < baseEvaluations.Length; i++)
					result[i] += (baseEvaluations[i] - mappingValue) * weight;
			}

			// Clamp result values
			for (int i = 0; i < result.Length; i++)
				result[i] = Clamp01(result[i] + mappingValue);

			return result;
		}

		public float[] Evaluate3D(Vector3[] points)
		{
			float[] result = new float[points.Length];
			float mappingValue = GetMappingValue();

			// Base evaluations
			for (int layer = 0; layer < Layers.Length; layer++)
			{
				Vector3[] baseEvaluationPoints = new Vector3[points.Length];
				points.CopyTo(baseEvaluationPoints, 0);
				float ff = Layers[layer].FrequencyFactor;

				// Apply FrequencyFactor to evaluation points
				for (int j = 0; j < baseEvaluationPoints.Length; j++)
					baseEvaluationPoints[j] *= ff;

				// Evaluate
				float[] baseEvaluations = Layers[layer].Noise.Evaluate3D(baseEvaluationPoints);

				// Add weight base value to result
				float weight = Layers[layer].Weight;
				for (int i = 0; i < baseEvaluations.Length; i++)
					result[i] += (baseEvaluations[i] - mappingValue) * weight;
			}

			// Clamp result values
			for (int i = 0; i < result.Length; i++)
				result[i] = Clamp01(result[i] + mappingValue);

			return result;
		}



		public float Evaluate2D(Vector2 point)
		{
			Func<NoiseLayer, float> baseEvaluation =
				l => l.Noise.Evaluate2D(point * l.FrequencyFactor);
			return ApplyNoise(baseEvaluation);
		}

		public float Evaluate3D(Vector3 point)
		{
			Func<NoiseLayer, float> baseEvaluation = 
				l => l.Noise.Evaluate3D(point * l.FrequencyFactor);
			return ApplyNoise(baseEvaluation);
		}

		private float ApplyNoise(Func<NoiseLayer, float> baseEvaluation)
		{
			float mappingValue = GetMappingValue();
			float result = 0;
			foreach (NoiseLayer layer in Layers)
			{
				float evaluatedValue = baseEvaluation(layer) - mappingValue;
				result += evaluatedValue * layer.Weight;
			}
			return Clamp01(result + mappingValue);
		}

		private float GetMappingValue()
		{
			return Mapping == MappingMode.ZeroToOne ? 0 : 0.5f;
		}

		private float Clamp01(float result)
		{
			return (result > 1) ? 1 : (result < 0) ? 0 : result;
		}

		public class NoiseLayer
		{
			public Noise3D Noise { get; }
			public float Weight { get; }
			public float FrequencyFactor { get; }

			public NoiseLayer(
				Noise3D noise,
				float weight,
				float frequencyFactor)
			{
				Noise = noise;
				Weight = weight;
				FrequencyFactor = frequencyFactor;
			}
		}

		public enum MappingMode
		{
			NegOneToOne = 0,
			ZeroToOne = 1
		}
	}
}