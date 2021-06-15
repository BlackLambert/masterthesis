using System;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
	public class LayeredNoise : Noise3D
	{
		private List<NoiseLayer> _layers;
		public List<NoiseLayer> LayersCopy => new List<NoiseLayer>(_layers);
		public MappingMode Mapping { get; }
		public NoiseType NoiseType => NoiseType.Layered;

		public LayeredNoise(List<NoiseLayer> layers, MappingMode mapping)
		{
			_layers = layers;
			Mapping = mapping;
		}

		public double Evaluate(double x, double y, double z)
		{
			double mappingValue = Mapping == MappingMode.ZeroToOne ? 0 : 0.5;
			double result = 0;
			foreach (NoiseLayer layer in _layers)
			{
				double ff = layer.FrequencyFactor;
				double evaluatedValue = layer.Noise.Evaluate(x * ff, y * ff, z * ff) - mappingValue;
				result += evaluatedValue * layer.Weight;
			}
			return Clamp01(result + mappingValue);
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
			public double Weight { get; }
			public double FrequencyFactor { get; }

			public NoiseLayer(
				Noise3D noise,
				double weight,
				double frequencyFactor)
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