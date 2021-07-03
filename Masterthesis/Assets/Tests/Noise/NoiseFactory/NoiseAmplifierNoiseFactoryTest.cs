using NUnit.Framework;
using System;

namespace SBaier.Master.Test
{
    [TestFixture]
    public class NoiseAmplifierNoiseFactoryTest : SpecificNoiseFactoryTest
    {
        private const string _noiseAmplifierSettingsPath = "Noise/TestNoiseAmplifierSettings";

        protected override Type ExpectedNoiseType => typeof(NoiseAmplifier);



        [Test(Description = "The created NoiseAmplifier has property values provided by the settings")]
        public void CreatedNoiseAmplifierHasExpectedProperties()
        {
            GivenANewNoiseFactory();
            GivenATestNoiseSetting();
            WhenCreateIsCalledWithTestNoiseSettings();
            ThenTheNoiseAmplifierPropertiesAreAsExpected();
        }

        protected override void GivenATestNoiseSetting()
        {
            Container.Bind(typeof(NoiseSettings), typeof(NoiseAmplifierSettings)).FromResource(_noiseAmplifierSettingsPath).AsSingle();
        }

        private void ThenTheNoiseAmplifierPropertiesAreAsExpected()
        {
            NoiseAmplifierSettings settings = Container.Resolve<NoiseAmplifierSettings>();
            NoiseAmplifier noise = (NoiseAmplifier)_noise;
            Assert.AreEqual(settings.BaseNoise.GetNoiseType(), noise.BaseNoise.NoiseType);
            Assert.AreEqual(settings.AmplifierNoise.GetNoiseType(), noise.AmplifierNoise.NoiseType);
            Assert.AreEqual(settings.Mode, noise.AmplifierMode);
        }
    }
}