using UnityEngine;

namespace SBaier.Master
{
	public class NoiseAmplifier : Noise3D
	{
		public NoiseAmplifier(Noise3D baseNoise, Noise3D amplifierNoise)
		{
			BaseNoise = baseNoise;
			AmplifierNoise = amplifierNoise;
		}

		public NoiseType NoiseType => NoiseType.Amplifier;

		public Noise3D BaseNoise { get; }
		public Noise3D AmplifierNoise { get; }

		public float Evaluate2D(Vector2 point)
		{
			float baseValue = BaseNoise.Evaluate2D(point);
			float amplifierValue = AmplifierNoise.Evaluate2D(point);
			return baseValue * amplifierValue * amplifierValue;
		}

		public float Evaluate3D(Vector3 point)
		{
			float baseValue = BaseNoise.Evaluate3D(point);
			float amplifierValue = AmplifierNoise.Evaluate3D(point);
			return baseValue * amplifierValue * amplifierValue;
		}

		public float[] Evaluate2D(Vector2[] points)
		{
			float[] baseValues = BaseNoise.Evaluate2D(points);
			float[] amplifierValues = AmplifierNoise.Evaluate2D(points);
			for (int i = 0; i < points.Length; i++)
				baseValues[i] = baseValues[i] * amplifierValues[i] * amplifierValues[i];
			return baseValues;
		}

		public float[] Evaluate3D(Vector3[] points)
		{
			float[] baseValues = BaseNoise.Evaluate3D(points);
			float[] amplifierValues = AmplifierNoise.Evaluate3D(points);
			for (int i = 0; i < points.Length; i++)
				baseValues[i] = baseValues[i] * amplifierValues[i] * amplifierValues[i];
			return baseValues;
		}
	}
}