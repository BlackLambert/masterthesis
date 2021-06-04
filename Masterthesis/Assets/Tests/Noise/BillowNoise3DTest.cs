using Zenject;
using NUnit.Framework;

namespace SBaier.Master.Test
{
    [TestFixture]
    public class BillowNoise3DTest : Noise3DTest
    {
		private const int _testSeed = 49242;

		protected override double AverageDelta => 0.02;
		protected override double ExpectedAverage => 0.2;

		protected override void GivenANew3DNoise()
		{
			Container.Bind<Seed>().To<Seed>().FromMethod(CreateSeed).AsTransient();
			Container.Bind<PerlinNoise>().To<PerlinNoise>().AsTransient();
			Container.Bind<Noise3D>().To<BillowNoise>().AsTransient();
		}

		private Seed CreateSeed()
		{
			return new Seed(_testSeed);
		}
	}
}