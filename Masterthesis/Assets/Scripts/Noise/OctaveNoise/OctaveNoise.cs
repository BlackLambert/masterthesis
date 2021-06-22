using System;
using UnityEngine;

namespace SBaier.Master
{
	public class OctaveNoise : Noise3D
	{
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

		public float[] Evaluate2D(Vector2[] points)
		{
			float[] result = new float[points.Length];

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
				float[] baseEvaluations = BaseNoise.Evaluate2D(baseEvaluationPoints);

				// Add weight base value to result
				float weight = StartWeight / factor;
				for (int i = 0; i < baseEvaluations.Length; i++)
					result[i] += (baseEvaluations[i] - 0.5f) * weight;
			}

			// Clamp result values
			for (int i = 0; i < result.Length; i++)
				result[i] = Clamp01(result[i] + 0.5f);

			return result;
		}

		public float[] Evaluate3D(Vector3[] points)
		{
			float[] result = new float[points.Length];

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
				float[] baseEvaluations = BaseNoise.Evaluate3D(baseEvaluationPoints);

				// Add weight base value to result
				float weight = StartWeight / factor;
				for (int i = 0; i < baseEvaluations.Length; i++)
					result[i] += (baseEvaluations[i] - 0.5f) * weight;
			}

			// Clamp result values
			for (int i = 0; i < result.Length; i++)
				result[i] = Clamp01(result[i] + 0.5f);

			return result;
		}

		public float Evaluate3D(Vector3 point)
		{
			Func<float, float> baseEvaluation = ff => BaseNoise.Evaluate3D(point * ff);
			return EvaluateOctaves(baseEvaluation);
		}

		public float Evaluate2D(Vector2 point)
		{
			Func<float, float> baseEvaluation = ff => BaseNoise.Evaluate2D(point * ff);
			return EvaluateOctaves(baseEvaluation);
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
	}
}