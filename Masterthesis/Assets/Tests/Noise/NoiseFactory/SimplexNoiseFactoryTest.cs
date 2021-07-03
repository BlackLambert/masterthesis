using Zenject;
using NUnit.Framework;
using System;
using UnityEngine;
using System.Collections.Generic;

namespace SBaier.Master.Test
{
    [TestFixture]
    public class SimplexNoiseFactoryTest : SpecificNoiseFactoryTest
    {
        private const string _simplexNoiseSettingsPath = "Noise/TestSimplexNoiseSettings";

		protected override Type ExpectedNoiseType => typeof(SimplexNoise);

		[Test(Description = "The SimplexNoise created has a seed based on provided base seed")]
        public void CreatedSimplexNoiseHasSeedBasedOnProvidedSeed()
        {
            GivenANewNoiseFactory();
            GivenATestNoiseSetting();
            WhenCreateIsCalledWithTestNoiseSettings();
            ThenSimplexNoiseHasSeedBasedOnProvidedSeed();
        }
        private void ThenSimplexNoiseHasSeedBasedOnProvidedSeed()
        {
            SimplexNoise noise = (SimplexNoise)_noise;
            Seed seed = Container.Resolve<Seed>();
            int expected = seed.Random.Next();
            int actual = noise.Seed.SeedNumber;
            Assert.AreEqual(expected, actual);
        }

		protected override void GivenATestNoiseSetting()
		{
            Container.Bind(typeof(NoiseSettings), typeof(SimplexNoiseSettings)).FromResource(_simplexNoiseSettingsPath).AsSingle();
        }
	}
}