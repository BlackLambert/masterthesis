using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace SBaier.Master
{
	public class NoiseFactoryImpl : NoiseFactory
	{
		private int _recursionDepthLimit = 20;
		private Dictionary<NoiseSettings, Noise3D> _createdNoise = new Dictionary<NoiseSettings, Noise3D>();

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
			Noise3D result = GetResult(settings, baseSeed, recursionDepth);
			return result;
		}

		private Noise3D GetResult(NoiseSettings settings, Seed baseSeed, int recursionDepth)
		{
			if (_createdNoise.ContainsKey(settings))
				return _createdNoise[settings];
			Noise3D result = CreateNoise(settings, baseSeed, recursionDepth);
			_createdNoise.Add(settings, result);
			SetBaseValues(settings, result);
			return result;
		}

		private Noise3D CreateNoise(NoiseSettings settings, Seed baseSeed, int recursionDepth)
		{
			switch (settings.GetNoiseType())
			{
				case NoiseType.Perlin:
					return CreatePerlinNoise((PerlinNoiseSettings)settings, baseSeed);
				case NoiseType.Billow:
					return CreateBillowNoise((BillowNoiseSettings)settings, baseSeed, recursionDepth + 1);
				case NoiseType.Ridged:
					return CreateRidgedNoise((RidgedNoiseSettings)settings, baseSeed, recursionDepth + 1);
				case NoiseType.Layered:
					return CreateLayeredNoise((LayeredNoiseSettings)settings, baseSeed, recursionDepth + 1);
				case NoiseType.Octave:
					return CreateOctaveNoise((OctaveNoiseSettings)settings, baseSeed, recursionDepth + 1);
				case NoiseType.Simplex:
					return CreateSimplexNoise((SimplexNoiseSettings)settings, baseSeed);
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

		private void SetBaseValues(NoiseSettings settings, Noise3D result)
		{
			NoiseBase noiseBase = result as NoiseBase;
			noiseBase.FrequencyFactor = settings.FrequencyFactor;
			noiseBase.Weight = settings.Weight;
		}

		private PerlinNoise CreatePerlinNoise(PerlinNoiseSettings settings, Seed baseSeed)
		{
			Seed seed = CreateSeedBasedOn(baseSeed);
			return new PerlinNoise(seed);
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

			Noise3D[] layers = new Noise3D[settings.Layers.Count];
			for (int i = 0; i < settings.Layers.Count; i++)
				layers[i] = Create(settings.Layers[i], baseSeed, recursionDepth + 1);
			return new LayeredNoise(layers, settings.Mapping);
		}

		private OctaveNoise CreateOctaveNoise(OctaveNoiseSettings settings, Seed baseSeed, int recursionDepth)
		{
			Noise3D baseNoise = Create(settings.BaseNoise, baseSeed, recursionDepth + 1);
			return new OctaveNoise(new OctaveNoise.Arguments(settings.OctavesCount, baseNoise));
		}

		private SimplexNoise CreateSimplexNoise(SimplexNoiseSettings settings, Seed baseSeed)
		{
			Seed seed = CreateSeedBasedOn(baseSeed);
			return new SimplexNoise(seed);
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

		public void ClearCache()
		{
			_createdNoise.Clear();
		}
	}
}