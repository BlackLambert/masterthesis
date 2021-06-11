using System;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
	public class LayeredNoise : Noise3D
	{
		public LayeredNoise(List<NoiseLayer> octaves)
		{
			_octaves = octaves;
		}

		private List<NoiseLayer> _octaves;
		public List<NoiseLayer> OctavesCopy => new List<NoiseLayer>(_octaves);

		public NoiseType NoiseType => NoiseType.Layered;

		public double Evaluate(double x, double y, double z)
		{
			double result = 0;
			foreach (NoiseLayer octave in _octaves)
			{
				double ff = octave.FrequencyFactor;
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

		public class NoiseLayer
		{
			public Noise3D Noise { get; }
			public double Amplitude { get; }
			public double FrequencyFactor { get; }

			public NoiseLayer(
				Noise3D noise,
				double amplitude,
				double frequencyFactor)
			{
				Noise = noise;
				Amplitude = amplitude;
				FrequencyFactor = frequencyFactor;
			}
		}
	}
}