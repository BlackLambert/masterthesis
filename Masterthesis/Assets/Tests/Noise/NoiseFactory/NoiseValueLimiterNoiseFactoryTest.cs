using NUnit.Framework;
using System;

namespace SBaier.Master.Test
{
    [TestFixture]
    public class NoiseValueLimiterNoiseFactoryTest : SpecificNoiseFactoryTest
    {
        private const string _noiseValueLimiterSettingsPath = "Noise/TestNoiseValueLimiterSettings";

        protected override Type ExpectedNoiseType => typeof(NoiseValueLimiter);


        [Test(Description = "The created NoiseValueLimiter has arguments provided by the settings")]
        public void CreatedNoiseValueLimiterHasExpectedProperties()
        {
            GivenANewNoiseFactory();
            GivenATestNoiseSetting();
            WhenCreateIsCalledWithTestNoiseSettings();
            ThenTheNoiseValueLimiterPropertiesAreAsExpected();
        }

        protected override void GivenATestNoiseSetting()
        {
            Container.Bind(typeof(NoiseSettings), typeof(NoiseValueLimiterSettings)).FromResource(_noiseValueLimiterSettingsPath).AsSingle();
        }

        private void ThenTheNoiseValueLimiterPropertiesAreAsExpected()
        {
            NoiseValueLimiterSettings settings = Container.Resolve<NoiseValueLimiterSettings>();
            NoiseValueLimiter limiter = (NoiseValueLimiter)_noise;
            Assert.AreEqual(settings.ValueLimits.x, limiter.Limits.x);
            Assert.AreEqual(settings.ValueLimits.y, limiter.Limits.y);
            Assert.AreEqual(settings.BaseNoise.GetNoiseType(), limiter.BaseNoise.NoiseType);
        }
    }
}