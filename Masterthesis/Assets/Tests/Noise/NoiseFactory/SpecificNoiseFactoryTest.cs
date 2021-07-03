using Zenject;
using NUnit.Framework;
using System;
using UnityEngine;
using System.Collections.Generic;

namespace SBaier.Master.Test
{
    [TestFixture]
    public abstract class SpecificNoiseFactoryTest : ZenjectUnitTestFixture
    {
		private const int _seedValue = 1234;
        protected abstract Type ExpectedNoiseType { get; }
        protected Noise3D _noise;

		[Test]
        public void Create_CreatesExpectedNoise()
        {
            GivenANewNoiseFactory();
            GivenATestNoiseSetting();
            WhenCreateIsCalledWithTestNoiseSettings();
            ThenANoiseOfGivenTypeIsCreated(ExpectedNoiseType);
        }

        [Test]
        public void Create_CreatedNoiseHasExpectedBaseValues()
        {
            GivenANewNoiseFactory();
            GivenATestNoiseSetting();
            WhenCreateIsCalledWithTestNoiseSettings();
            ThenTheCreatedNoiseBaseHasValuesAsExpected();
        }

        protected abstract void GivenATestNoiseSetting();

		protected void GivenANewNoiseFactory()
		{
            Container.Bind<Seed>().AsTransient().WithArguments(_seedValue);
            Container.Bind<NoiseFactory>().To<NoiseFactoryImpl>().AsTransient();
        }

        protected void WhenCreateIsCalledWithTestNoiseSettings()
        {
            NoiseSettings settings = Container.Resolve<NoiseSettings>();
            CreateNoise(settings);
        }

        private void ThenANoiseOfGivenTypeIsCreated(Type type)
        {
            Assert.AreEqual(type, _noise.GetType());
        }

        private void ThenTheCreatedNoiseBaseHasValuesAsExpected()
        {
            NoiseBase noise = (NoiseBase)_noise;
            NoiseSettings settings = Container.Resolve<NoiseSettings>();
            Assert.AreEqual(settings.FrequencyFactor, noise.FrequencyFactor);
            Assert.AreEqual(settings.Weight, noise.Weight);
        }

        private void CreateNoise(NoiseSettings settings)
		{
            NoiseFactory factory = Container.Resolve<NoiseFactory>();
            Seed seed = Container.Resolve<Seed>();
            _noise = factory.Create(settings, seed);
        }
    }
}