using NUnit.Framework;
using System;

namespace SBaier.Master.Test
{
    [TestFixture]
    public class StaticValueNoiseFactoryTest : SpecificNoiseFactoryTest
    {
        private const string _staticValueNoiseSettingsPath = "Noise/TestStaticValueNoiseSettings";

        protected override Type ExpectedNoiseType => typeof(StaticValueNoise);

		protected override void GivenATestNoiseSetting()
        {
            Container.Bind(typeof(NoiseSettings), typeof(StaticValueNoiseSettings)).FromResource(_staticValueNoiseSettingsPath).AsSingle();
        }

        [Test(Description = "The created StaticValueNoise has property values provided by the settings")]
        public void CreatedStaticValueNoiseHasExpectedProperties()
        {
            GivenANewNoiseFactory();
            GivenATestNoiseSetting();
            WhenCreateIsCalledWithTestNoiseSettings();
            ThenTheStaticValueNoisePropertiesAreAsExpected();
        }

        private void ThenTheStaticValueNoisePropertiesAreAsExpected()
        {
            StaticValueNoiseSettings settings = Container.Resolve<StaticValueNoiseSettings>();
            StaticValueNoise noise = (StaticValueNoise)_noise;
            Assert.AreEqual(settings.Value, noise.Value);
        }
    }
}