using Zenject;
using NUnit.Framework;

namespace SBaier.Master.Test
{
    [TestFixture]
    public class RidgedNoiseTest : NoiseTest
    {
		private const int _testSeed = 49242;


		protected override void GivenANew3DNoise()
		{
			Container.Bind<Seed>().To<Seed>().FromMethod(CreateSeed).AsTransient();
			Container.Bind<PerlinNoise>().To<PerlinNoise>().AsTransient();
			PerlinNoise baseNoise = Container.Resolve<PerlinNoise>();
			Container.Bind<BillowNoise>().To<BillowNoise>().AsTransient().WithArguments(baseNoise);
			Container.Bind<Noise3D>().To<RidgedNoise>().AsTransient();
		}

		private Seed CreateSeed()
		{
			return new Seed(_testSeed);
		}
	}
}