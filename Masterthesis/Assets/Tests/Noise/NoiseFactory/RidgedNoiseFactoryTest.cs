using NUnit.Framework;
using System;

namespace SBaier.Master.Test
{
    [TestFixture]
    public class RidgedNoiseFactoryTest : SpecificNoiseFactoryTest
    {
        private const string _perlinBasedRidgedNoiseSettingsPath = "Noise/TestPerlinRidgedNoiseSettings";

        protected override Type ExpectedNoiseType => typeof(RidgedNoise);

		protected override void GivenATestNoiseSetting()
        {
            Container.Bind(typeof(NoiseSettings), typeof(RidgedNoiseSettings)).FromResource(_perlinBasedRidgedNoiseSettingsPath).AsSingle();
        }
    }
}