using Zenject;
using NUnit.Framework;
using System;
using UnityEngine;
using System.Collections.Generic;

namespace SBaier.Master.Test
{
    [TestFixture]
    public class NoiseFactoryTest : ZenjectUnitTestFixture
    {
		private const int _seedValue = 1234;
        private const string _loopedNoiseSettingsPath = "Noise/TestLoopedBillowNoiseSettings";
        private readonly string[] _sameNoiseTestSettingPaths = new string[]
        {
            "Noise/TestSimplexNoiseSettings",
            "Noise/TestThreeLayeredNoiseSettings",
            "Noise/TestPerlinNoiseSettings",
            "Noise/TestOctaveNoiseSettings",
        };

        private Noise3D _noise;
        private Noise3D _noiseTwo;
        private NoiseSettings _settings;

        [Test]
        public void ThrowsExceptionOnNullArgument()
        {
            GivenANewNoiseFactory();
            TestDelegate test = () => WhenCreateIsCalledWithSettingsNull();
            ThenAnArgumentNullExceptionIsThrown(test);
        }

        [Test(Description = "The Noise Creation Process stops if Recursion Depth Limit is reached."), Timeout(2000)]
        public void CreationStopsIfRecursionDepthIsReached()
        {
            GivenANewNoiseFactory();
            TestDelegate test = () => WhenCreateIsCalledWithInfiniteLoopedSettings();
            ThenARecursionDepthExceptionIsThrown(test);
        }

        [Test(Description = "The RecursionDepthLimit property throws an ArgumentOutOfRangeException if set to a negative value.")]
        public void RecursionDepthLimitThrowsExceptionIfSetToNegativeValue()
        {
            GivenANewNoiseFactory();
            NoiseFactory factory = Container.Resolve<NoiseFactory>();
            TestDelegate test = () => WhenRecursionDepthLimitIsChangedToANegativeValue(factory);
            ThenAnArgumentOutOfRangeExceptionIsThrown(test);
        }

        [Test]
        public void NoiseCreatedFromTheSameSettingsAreTheSame()
        {
            for (int i = 0; i < _sameNoiseTestSettingPaths.Length; i++)
            {
                GivenANewNoiseFactory();
                WhenCalledWithSameSettings(_sameNoiseTestSettingPaths[i]);
                ThenNoiseOneEqualsNoiseTwo();
                Teardown();
                Setup();
            }
        }

		private void GivenANewNoiseFactory()
		{
            Container.Bind<Seed>().AsTransient().WithArguments(_seedValue);
            Container.Bind<NoiseFactory>().To<NoiseFactoryImpl>().AsTransient();
        }


        private void WhenCreateIsCalledWithSettingsNull()
        {
            CreateNoise(null);
        }

        private void WhenCreateIsCalledWithInfiniteLoopedSettings()
        {
            _settings = Resources.Load<BillowNoiseSettings>(_loopedNoiseSettingsPath);
            CreateNoise(_settings);
        }

        private void WhenRecursionDepthLimitIsChangedToANegativeValue(NoiseFactory factory)
        {
            factory.RecursionDepthLimit = -1;
        }

        private void WhenCalledWithSameSettings(string settingsPath)
        {
            NoiseFactory factory = Container.Resolve<NoiseFactory>();
            _settings = Resources.Load<NoiseSettings>(settingsPath);
            Seed seed = Container.Resolve<Seed>();
            _noise = factory.Create(_settings, seed);
            _noiseTwo = factory.Create(_settings, seed);
        }

        private void ThenAnArgumentNullExceptionIsThrown(TestDelegate test)
        {
            Assert.Throws<ArgumentNullException>(test);
        }

        private void ThenARecursionDepthExceptionIsThrown(TestDelegate test)
        {
            Assert.Throws<NoiseFactory.RecursionDepthLimitReachedException>(test);
        }

        private void ThenAnArgumentOutOfRangeExceptionIsThrown(TestDelegate test)
        {
            Assert.Throws<ArgumentOutOfRangeException>(test);
        }

        private void ThenNoiseOneEqualsNoiseTwo()
        {
            Assert.AreSame(_noise, _noiseTwo);
        }

        private void CreateNoise(NoiseSettings settings)
		{
            NoiseFactory factory = Container.Resolve<NoiseFactory>();
            Seed seed = Container.Resolve<Seed>();
            _noise = factory.Create(settings, seed);
        }
    }
}