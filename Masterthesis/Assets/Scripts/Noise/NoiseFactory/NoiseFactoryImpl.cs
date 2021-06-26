using System;
using System.Collections.Generic;
using Zenject;

namespace SBaier.Master
{
	public class NoiseFactoryImpl : NoiseFactory
	{
		private int _recursionDepthLimit = 20;

		public int RecursionDepthLimit
		{
			get => _recursionDepthLimit;
			set
			{
				if (value < 0)
					throw new ArgumentOutOfRangeException();
				_recursionDepthLimit = value;
			}
		}

		public Noise3D Create(NoiseSettings settings, Seed baseSeed)
		{
			return Create(settings, baseSeed, 0);
		}

		private Noise3D Create(NoiseSettings settings, Seed baseSeed, int recursionDepth)
		{
			CheckRecursionDepth(recursionDepth);
			CheckNotNull(settings);
			CheckNotNull(baseSeed);

			switch (settings.GetNoiseType())
			{
				case NoiseType.Perlin:
					return CreatePerlinNoise(baseSeed);
				case NoiseType.Billow:
					return CreateBillowNoise((BillowNoiseSettings)settings, baseSeed, recursionDepth + 1);
				case NoiseType.Ridged:
					return CreateRidgedNoise((RidgedNoiseSettings)settings, baseSeed, recursionDepth + 1);
				case NoiseType.Layered:
					return CreateLayeredNoise((LayeredNoiseSettings)settings, baseSeed, recursionDepth + 1);
				case NoiseType.Octave:
					return CreateOctaveNoise((OctaveNoiseSettings)settings, baseSeed, recursionDepth + 1);
				case NoiseType.Simplex:
					return CreateSimplexNoise(baseSeed);
				case NoiseType.NoiseValueLimiter:
					return CreateNoiseValueLimiter((NoiseValueLimiterSettings)settings, baseSeed, recursionDepth + 1);
				case NoiseType.Static:
					return CreateStaticValueNoise((StaticValueNoiseSettings)settings);
				case NoiseType.Amplifier:
					return CreateNoiseAmplifier((NoiseAmplifierSettings)settings, baseSeed, recursionDepth + 1);
				default:
					throw new NotImplementedException();
			}
		}

		private PerlinNoise CreatePerlinNoise(Seed baseSeed)
		{
			return new PerlinNoise(CreateSeedBasedOn(baseSeed));
		}

		private BillowNoise CreateBillowNoise(BillowNoiseSettings settings, Seed baseSeed, int recursionDepth)
		{
			Noise3D baseNoise = Create(settings.BaseNoiseSettings, baseSeed, recursionDepth);
			return new BillowNoise(baseNoise);
		}

		private RidgedNoise CreateRidgedNoise(RidgedNoiseSettings settings, Seed baseSeed, int recursionDepth)
		{
			Noise3D baseNoise = Create(settings.BaseNoiseSettings, baseSeed, recursionDepth);
			return new RidgedNoise(baseNoise);
		}

		private LayeredNoise CreateLayeredNoise(LayeredNoiseSettings settings, Seed baseSeed, int recursionDepth)
		{
			if (settings.Layers.Count == 0)
				throw new ArgumentException();
				
			List<LayeredNoise.NoiseLayer> layers = new List<LayeredNoise.NoiseLayer>();
			foreach (NoiseLayerSettings layerSetting in settings.Layers)
			{
				Noise3D noise = Create(layerSetting.NoiseSettings, baseSeed, recursionDepth + 1);
				layers.Add(new LayeredNoise.NoiseLayer(noise, layerSetting.Weight, layerSetting.FrequencyFactor));
			}
			return new LayeredNoise(layers, settings.Mapping);
		}

		private OctaveNoise CreateOctaveNoise(OctaveNoiseSettings settings, Seed baseSeed, int recursionDepth)
		{
			Noise3D baseNoise = Create(settings.BaseNoise, baseSeed, recursionDepth + 1);
			return new OctaveNoise(new OctaveNoise.Arguments(settings.OctavesCount, baseNoise, settings.StartFrequency, settings.StartWeight));
		}

		private SimplexNoise CreateSimplexNoise(Seed baseSeed)
		{
			return new SimplexNoise(CreateSeedBasedOn(baseSeed));
		}

		private NoiseValueLimiter CreateNoiseValueLimiter(NoiseValueLimiterSettings settings, Seed baseSeed, int recursionDepth)
		{
			Noise3D baseNoise = Create(settings.BaseNoise, baseSeed, recursionDepth);
			return new NoiseValueLimiter(settings.ValueLimits, baseNoise);
		}

		private StaticValueNoise CreateStaticValueNoise(StaticValueNoiseSettings settings)
		{
			return new StaticValueNoise(settings.Value);
		}

		private NoiseAmplifier CreateNoiseAmplifier(NoiseAmplifierSettings settings, Seed baseSeed, int recursionDepth)
		{
			Noise3D baseA = Create(settings.BaseNoise, baseSeed, recursionDepth);
			Noise3D baseB = Create(settings.AmplifierNoise, baseSeed, recursionDepth);
			return new NoiseAmplifier(baseA, baseB, settings.Mode);
		}

		private Seed CreateSeedBasedOn(Seed seed)
		{
			int seedValue = seed.Random.Next();
			return new Seed(seedValue);
		}

		private void CheckRecursionDepth(int recursionDepth)
		{
			if (recursionDepth > _recursionDepthLimit)
				throw new NoiseFactory.RecursionDepthLimitReachedException();
		}

		private void CheckNotNull(object argument)
		{
			if (argument == null)
				throw new ArgumentNullException();
		}
	}
}