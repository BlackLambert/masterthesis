using System;
using System.Collections.Generic;
using Zenject;

namespace SBaier.Master
{
	public class NoiseFactoryImpl : NoiseFactory
	{
		private int _recursionDepthLimit = 8;

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
				case NoiseType.Octave:
					return CreateOctaveNoise((OctaveNoiseSettings)settings, baseSeed, recursionDepth + 1);
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
			BillowNoise baseNoise = (BillowNoise)Create(settings.BillowNoiseSettings, baseSeed, recursionDepth);
			return new RidgedNoise(baseNoise);
		}

		private OctaveNoise CreateOctaveNoise(OctaveNoiseSettings settings, Seed baseSeed, int recursionDepth)
		{
			if (settings.Octaves.Count == 0)
				throw new ArgumentException();
				
			List<OctaveNoise.Octave> octaves = new List<OctaveNoise.Octave>();
			foreach (OctaveSettings octaveSetting in settings.Octaves)
			{
				Noise3D noise = Create(octaveSetting.NoiseSettings, baseSeed, recursionDepth + 1);
				octaves.Add(new OctaveNoise.Octave(noise, octaveSetting.Amplitude, octaveSetting.FrequencyFactor));
			}
			return new OctaveNoise(octaves);
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