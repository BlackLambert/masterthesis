using Zenject;
using NUnit.Framework;
using System;
using UnityEngine;
using System.Collections.Generic;

namespace SBaier.Master.Test
{
    [TestFixture]
    public class BillowNoiseFactoryTest : SpecificNoiseFactoryTest
    {
		private const string _perlinBasedBillowNoiseSettingsPath = "Noise/TestPerlinBillowNoiseSettings";

		protected override Type ExpectedNoiseType => typeof(BillowNoise);

		[Test]
        public void CreatedBillowsBaseNoiseHasExpectedSeed()
        {
            GivenANewNoiseFactory();
            GivenATestNoiseSetting();
            WhenCreateIsCalledWithTestNoiseSettings();
            BillowNoise billowNoise = (BillowNoise)_noise;
            ThenPerlinNoiseHasSeedBasedOnProvidedSeed((PerlinNoise)billowNoise.BaseNoise);
        }

		protected override void GivenATestNoiseSetting()
        {
            Container.Bind(typeof(NoiseSettings), typeof(BillowNoiseSettings)).FromResource(_perlinBasedBillowNoiseSettingsPath).AsSingle();
        }

		private void ThenPerlinNoiseHasSeedBasedOnProvidedSeed(PerlinNoise noise)
        {
            Seed seed = Container.Resolve<Seed>();
            int expected = seed.Random.Next();
            int actual = noise.Seed.SeedNumber;
            Assert.AreEqual(expected, actual);
        }

    }
}