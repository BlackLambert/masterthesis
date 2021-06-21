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

		public double[] Evaluate(Vector2[] points)
		{
			double[] result = new double[points.Length];
			double mappingValue = Mapping == MappingMode.ZeroToOne ? 0 : 0.5;

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
				double[] baseEvaluations = Layers[layer].Noise.Evaluate(baseEvaluationPoints);

				// Add weight base value to result
				double weight = Layers[layer].Weight;
				for (int i = 0; i < baseEvaluations.Length; i++)
					result[i] += (baseEvaluations[i] - mappingValue) * weight;
			}

			// Clamp result values
			for (int i = 0; i < result.Length; i++)
				result[i] = Clamp01(result[i] + mappingValue);

			return result;
		}

		public double[] Evaluate(Vector3[] points)
		{
			double[] result = new double[points.Length];
			double mappingValue = Mapping == MappingMode.ZeroToOne ? 0 : 0.5;

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
				double[] baseEvaluations = Layers[layer].Noise.Evaluate(baseEvaluationPoints);

				// Add weight base value to result
				double weight = Layers[layer].Weight;
				for (int i = 0; i < baseEvaluations.Length; i++)
					result[i] += (baseEvaluations[i] - mappingValue) * weight;
			}

			// Clamp result values
			for (int i = 0; i < result.Length; i++)
				result[i] = Clamp01(result[i] + mappingValue);

			return result;
		}



		public double Evaluate(double x, double y)
		{
			Func<NoiseLayer, double> baseEvaluation =
				l => l.Noise.Evaluate(x * l.FrequencyFactor, y * l.FrequencyFactor);
			return ApplyNoise(baseEvaluation);
		}

		public double Evaluate(double x, double y, double z)
		{
			Func<NoiseLayer, double> baseEvaluation = 
				l => l.Noise.Evaluate(x * l.FrequencyFactor, y * l.FrequencyFactor, z * l.FrequencyFactor);
			return ApplyNoise(baseEvaluation);
		}

		private double ApplyNoise(Func<NoiseLayer, double> baseEvaluation)
		{
			double mappingValue = Mapping == MappingMode.ZeroToOne ? 0 : 0.5;
			double result = 0;
			foreach (NoiseLayer layer in Layers)
			{
				double evaluatedValue = baseEvaluation(layer) - mappingValue;
				result += evaluatedValue * layer.Weight;
			}
			return Clamp01(result + mappingValue);
		}

		private double Clamp01(double result)
		{
			return (result > 1) ? 1 : (result < 0) ? 0 : result;
		}

		public class NoiseLayer
		{
			public Noise3D Noise { get; }
			public double Weight { get; }
			public float FrequencyFactor { get; }

			public NoiseLayer(
				Noise3D noise,
				double weight,
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