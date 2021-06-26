using System;
using Unity.Collections;
using UnityEngine;

namespace SBaier.Master
{
	public class NoiseAmplifier : Noise3D
	{
		public NoiseAmplifier(Noise3D baseNoise, 
			Noise3D amplifierNoise,
			Mode mode)
		{
			BaseNoise = baseNoise;
			AmplifierNoise = amplifierNoise;
			AmplifierMode = mode;
		}

		public NoiseType NoiseType => NoiseType.Amplifier;

		public Noise3D BaseNoise { get; }
		public Noise3D AmplifierNoise { get; }
		public Mode AmplifierMode { get; }

		public float Evaluate2D(Vector2 point)
		{
			float baseValue = BaseNoise.Evaluate2D(point);
			float amplifierValue = AmplifierNoise.Evaluate2D(point);
			return baseValue * GetAmplifierFactor(amplifierValue);
		}

		public float Evaluate3D(Vector3 point)
		{
			float baseValue = BaseNoise.Evaluate3D(point);
			float amplifierValue = AmplifierNoise.Evaluate3D(point);
			return baseValue * GetAmplifierFactor(amplifierValue);
		}

		public NativeArray<float> Evaluate2D(NativeArray<Vector2> points)
		{
			NativeArray<float> baseValues = BaseNoise.Evaluate2D(points);
			NativeArray<float> amplifierValues = AmplifierNoise.Evaluate2D(points);
			for (int i = 0; i < points.Length; i++)
				baseValues[i] = baseValues[i] * GetAmplifierFactor(amplifierValues[i]);
			amplifierValues.Dispose();
			return baseValues;
		}

		public NativeArray<float> Evaluate3D(NativeArray<Vector3> points)
		{
			NativeArray<float> baseValues = BaseNoise.Evaluate3D(points);
			NativeArray<float> amplifierValues = AmplifierNoise.Evaluate3D(points);
			for (int i = 0; i < points.Length; i++)
				baseValues[i] = baseValues[i] * GetAmplifierFactor(amplifierValues[i]);
			amplifierValues.Dispose();
			return baseValues;
		}

		private float GetAmplifierFactor(float amplifierValue)
		{
			switch(AmplifierMode)
			{
				case Mode.Linear:
					return amplifierValue;
				case Mode.Quadric:
					return amplifierValue * amplifierValue;
				case Mode.HalfQuadric:
					return Mathf.Pow(amplifierValue, 1.5f);
				default:
					throw new NotImplementedException();
			}
		}

		public enum Mode
		{
			Linear = 0,
			Quadric = 1,
			HalfQuadric = 2
		}
	}
}