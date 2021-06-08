using System;
using Zenject;

namespace SBaier.Master
{
	public class NoiseFactory
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

		private Noise3D CreateRidgedNoise(RidgedNoiseSettings settings, Seed baseSeed, int recursionDepth)
		{
			BillowNoise baseNoise = (BillowNoise)Create(settings.BillowNoiseSettings, baseSeed, recursionDepth);
			return new RidgedNoise(baseNoise);
		}

		private Seed CreateSeedBasedOn(Seed seed)
		{
			int seedValue = seed.Random.Next();
			return new Seed(seedValue);
		}

		private void CheckRecursionDepth(int recursionDepth)
		{
			if (recursionDepth > _recursionDepthLimit)
				throw new RecursionDepthLimitReachedException();
		}

		private void CheckNotNull(object argument)
		{
			if (argument == null)
				throw new ArgumentNullException();
		}

		public class RecursionDepthLimitReachedException : Exception
		{

		}
	}
}