using Zenject;
using NUnit.Framework;
using System;

namespace SBaier.Master.Test
{
    [TestFixture]
    public class BillowNoiseTest : NoiseTest
    {
		private const int _testSeed = 49242;

		protected override NoiseType ExpectedNoiseType => NoiseType.Billow;

		protected override void GivenANew3DNoise()
		{
			Container.Bind<Noise3D>().To<BillowNoise>().FromMethod(CreateNoise).AsTransient();
		}

		private BillowNoise CreateNoise()
		{
			return new BillowNoise(new PerlinNoise(new Seed(_testSeed)));
		}
	}
}