

using System;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace SBaier.Master
{
	public class OctaveNoise : Noise3D
	{
		public int OctavesCount { get; }
		public Noise3D BaseNoise { get; }
		public float StartFrequency { get; }
		public double StartWeight { get; }

		public NoiseType NoiseType => NoiseType.Octave;


		public OctaveNoise(Arguments args)
		{
			OctavesCount = args.OctavesCount;
			BaseNoise = args.BaseNoise;
			StartFrequency = args.StartFrequency;
			StartWeight = args.StartWeight;
		}

		public double[] Evaluate(Vector2[] points)
		{
			double[] result = new double[points.Length];

			// Base evaluations
			for (int octave = 0; octave < OctavesCount; octave++)
			{
				Vector2[] baseEvaluationPoints = new Vector2[points.Length];
				points.CopyTo(baseEvaluationPoints, 0);
				float factor = Mathf.Pow(2, octave);
				float ff = StartFrequency * factor;

				// Apply FrequencyFactor to evaluation points
				for (int j = 0; j < baseEvaluationPoints.Length; j++)
					baseEvaluationPoints[j] *= ff;

				// Evaluate
				double[] baseEvaluations = BaseNoise.Evaluate(baseEvaluationPoints);

				// Add weight base value to result
				double weight = StartWeight / factor;
				for (int i = 0; i < baseEvaluations.Length; i++)
					result[i] += (baseEvaluations[i] - 0.5f) * weight;
			}

			// Clamp result values
			for (int i = 0; i < result.Length; i++)
				result[i] = Clamp01(result[i] + 0.5f);

			return result;
		}

		public double[] Evaluate(Vector3[] points)
		{
			double[] result = new double[points.Length];

			// Base evaluations
			for (int octave = 0; octave < OctavesCount; octave++)
			{
				Vector3[] baseEvaluationPoints = new Vector3[points.Length];
				points.CopyTo(baseEvaluationPoints, 0);
				float factor = Mathf.Pow(2, octave);
				float ff = StartFrequency * factor;

				// Apply FrequencyFactor to evaluation points
				for (int j = 0; j < baseEvaluationPoints.Length; j++)
					baseEvaluationPoints[j] *= ff;

				// Evaluate
				double[] baseEvaluations = BaseNoise.Evaluate(baseEvaluationPoints);

				// Add weight base value to result
				double weight = StartWeight / factor;
				for (int i = 0; i < baseEvaluations.Length; i++)
					result[i] += (baseEvaluations[i] - 0.5f) * weight;
			}

			// Clamp result values
			for (int i = 0; i < result.Length; i++)
				result[i] = Clamp01(result[i] + 0.5f);

			return result;
		}

		public double Evaluate(double x, double y, double z)
		{
			Func<double, double> baseEvaluation = ff => BaseNoise.Evaluate(x * ff, y * ff, z * ff);
			return EvaluateOctaves(baseEvaluation);
		}

		public double Evaluate(double x, double y)
		{
			Func<double, double> baseEvaluation = ff => BaseNoise.Evaluate(x * ff, y * ff);
			return EvaluateOctaves(baseEvaluation);
		}

		public double EvaluateOctaves(Func<double, double> baseEvaluation)
		{
			double result = 0;
			for (int octave = 0; octave < OctavesCount; octave++)
				result += EvaluateOctave(octave, baseEvaluation);
			return Clamp01(result + 0.5f);
		}

		private double EvaluateOctave(int octave, Func<double, double> baseEvaluation)
		{
			double factor = Math.Pow(2, octave);
			double ff = StartFrequency * factor;
			double weight = StartWeight / factor;
			return (baseEvaluation(ff) - 0.5f) * weight;
		}

		private double Clamp01(double result)
		{
			return (result > 1) ? 1 : (result < 0) ? 0 : result;
		}

		public class Arguments
		{
			public int OctavesCount { get; }
			public Noise3D BaseNoise { get; }
			public float StartFrequency { get; }
			public double StartWeight { get; }


			public Arguments(int octavesCount, Noise3D baseNoise, float startFrequency, double startWeight)
			{
				CheckStartWeightOutOfRange(startWeight);
				CheckStartFrequencyOutOfRange(startFrequency);

				OctavesCount = octavesCount;
				BaseNoise = baseNoise;
				StartFrequency = startFrequency;
				StartWeight = startWeight;
			}

			private void CheckStartWeightOutOfRange(double startWeight)
			{
				if (startWeight < 0)
					throw new ArgumentOutOfRangeException();
			}

			private void CheckStartFrequencyOutOfRange(double startFrequency)
			{
				if (startFrequency < 0)
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}