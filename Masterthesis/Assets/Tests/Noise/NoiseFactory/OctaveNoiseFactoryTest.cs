using NUnit.Framework;
using System;

namespace SBaier.Master.Test
{
    [TestFixture]
    public class OctaveNoiseFactoryTest : SpecificNoiseFactoryTest
    {
        private const string _octaveNoiseSettingsPath = "Noise/TestOctaveNoiseSettings";

        protected override Type ExpectedNoiseType => typeof(OctaveNoise);



        [Test(Description = "The Create method creates an OctaveNoise with all values provided by the Arguments.")]
        public void CreatesOctaveNoiseWithExpectedValues()
        {
            GivenANewNoiseFactory();
            GivenATestNoiseSetting();
            WhenCreateIsCalledWithTestNoiseSettings();
            ThenAnOctaveNoiseWithExpectedValuesIsCreated();
        }

        protected override void GivenATestNoiseSetting()
        {
            Container.Bind(typeof(NoiseSettings), typeof(OctaveNoiseSettings)).FromResource(_octaveNoiseSettingsPath).AsSingle();
        }

        private void ThenAnOctaveNoiseWithExpectedValuesIsCreated()
        {
            OctaveNoiseSettings settings = Container.Resolve<OctaveNoiseSettings>();
            OctaveNoise noise = (OctaveNoise)_noise;
            Assert.AreEqual(settings.BaseNoise.GetNoiseType(), noise.BaseNoise.NoiseType);
            Assert.AreEqual(settings.OctavesCount, noise.OctavesCount);
        }
    }
}