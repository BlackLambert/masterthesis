using Zenject;
using NUnit.Framework;

namespace SBaier.Master.Test
{
    [TestFixture]
    public class RidgedNoiseTest : NoiseTest
    {
		private const int _testSeed = 49242;

		protected override NoiseType ExpectedNoiseType => NoiseType.Ridged;

		protected override void GivenANew3DNoise()
		{
			Container.Bind<Noise3D>().To<RidgedNoise>().FromMethod(CreateNoise).AsTransient();
		}

		private RidgedNoise CreateNoise()
		{
			return new RidgedNoise(new BillowNoise(new PerlinNoise(new Seed(_testSeed))));
		}
	}
}