using Zenject;
using NUnit.Framework;

namespace SBaier.Master.Test
{
    [TestFixture]
    public class RidgedNoise2DTest : Noise2DTest
    {
		private const int _testSeed = 49242;

		protected override double AverageDelta => 0.02;
		protected override double ExpectedAverage => 0.8;

		protected override void GivenANew2DNoise()
		{
			Container.Bind<Seed>().To<Seed>().FromMethod(CreateSeed).AsTransient();
			Container.Bind<PerlinNoise>().To<PerlinNoise>().AsTransient();
			Container.Bind<BillowNoise>().To<BillowNoise>().AsTransient();
			Container.Bind<Noise2D>().To<RidgedNoise>().AsTransient();
		}

		private Seed CreateSeed()
		{
			return new Seed(_testSeed);
		}
	}
}