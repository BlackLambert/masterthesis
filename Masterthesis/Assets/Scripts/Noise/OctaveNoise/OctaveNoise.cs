using System;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
	public class OctaveNoise : Noise3D
	{
		public OctaveNoise(ICollection<Octave> octaves)
		{
			Octaves = octaves;
		}

		public ICollection<Octave> Octaves { get; }

		public double Evaluate(double x, double y, double z)
		{
			double result = 0;
			foreach (Octave octave in Octaves)
			{
				float ff = octave.FrequencyFactor;
				double evaluatedValue = octave.Noise.Evaluate(x * ff, y * ff, z * ff) - 0.5;
				result += evaluatedValue * octave.Amplitude;
			}
			return Clamp01(result + 0.5);
		}

		private double Clamp01(double result)
		{
			return (result > 1) ? 1 : (result < 0) ? 0 : result;
		}

		public double Evaluate(double x, double y)
		{
			return Evaluate(x, y, 0);
		}

		public class Octave
		{
			public Noise3D Noise { get; }
			public float Amplitude { get; }
			public float FrequencyFactor { get; }

			public Octave(
				Noise3D noise,
				float amplitude,
				float frequencyFactor)
			{
				Noise = noise;
				Amplitude = amplitude;
				FrequencyFactor = frequencyFactor;
			}
		}
	}
}