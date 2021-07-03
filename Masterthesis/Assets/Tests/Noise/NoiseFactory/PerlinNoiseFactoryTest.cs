using NUnit.Framework;
using System;

namespace SBaier.Master.Test
{
    [TestFixture]
    public class PerlinNoiseFactoryTest : SpecificNoiseFactoryTest
    {
		private const string _perlinNoiseSettingsPath = "Noise/TestPerlinNoiseSettings";

		protected override Type ExpectedNoiseType => typeof(PerlinNoise);

        [Test]
        public void CreatedPerlinNoiseHasSeedBasedOnProvidedSeed()
        {
            GivenANewNoiseFactory();
            GivenATestNoiseSetting();
            WhenCreateIsCalledWithTestNoiseSettings();
            ThenPerlinNoiseHasSeedBasedOnProvidedSeed();
        }

        private void ThenPerlinNoiseHasSeedBasedOnProvidedSeed()
        {
            PerlinNoise noise = (PerlinNoise)_noise;
            Seed seed = Container.Resolve<Seed>();
            int expected = seed.Random.Next();
            int actual = noise.Seed.SeedNumber;
            Assert.AreEqual(expected, actual);
        }

		protected override void GivenATestNoiseSetting()
		{
            Container.Bind(typeof(NoiseSettings), typeof(PerlinNoiseSettings)).FromResource(_perlinNoiseSettingsPath).AsSingle();
		}
	}
}