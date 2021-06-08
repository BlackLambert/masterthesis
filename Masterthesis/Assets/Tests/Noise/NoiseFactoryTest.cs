using Zenject;
using NUnit.Framework;
using System;
using UnityEngine;

namespace SBaier.Master.Test
{
    [TestFixture]
    public class NoiseFactoryTest : ZenjectUnitTestFixture
    {
		private const int _seedValue = 1234;
		private const string _perlinNoiseSettingsPath = "Noise/TestPerlinNoiseSettings";
		private const string _perlinBasedBillowNoiseSettingsPath = "Noise/TestPerlinBillowNoiseSettings";
        private const string _perlinBasedRidgedNoiseSettingsPath = "Noise/TestPerlinRidgedNoiseSettings";
        private const string _loopedNoiseSettingsPath = "Noise/TestLoopedBillowNoiseSettings";
        private Noise3D _noise;

		[Test]
        public void CreatesPerlinNoiseOnPerlinNoiseSettings()
        {
            GivenANewNoiseFactory();
            WhenCreateIsCalledWithPerlinNoiseSettings();
            ThenAPerlinNoiseIsCreated();
        }

        [Test]
        public void CreatedPerlinNoiseHasSeedBasedOnProvidedSeed()
        {
            GivenANewNoiseFactory();
            WhenCreateIsCalledWithPerlinNoiseSettings();
            ThenPerlinNoiseHasSeedBasedOnProvidedSeed((PerlinNoise)_noise);
        }

		[Test]
        public void CreatesBillowNoiseOnBillowNoiseSettings()
        {
            GivenANewNoiseFactory();
            WhenCreateIsCalledWithBillowNoiseSettings();
            ThenBillowNoiseIsCreated();
        }

        [Test]
        public void CreatedBillowsBaseNoiseHasExpectedSeed()
        {
            GivenANewNoiseFactory();
            WhenCreateIsCalledWithBillowNoiseSettings();
            BillowNoise billowNoise = (BillowNoise)_noise;
            ThenPerlinNoiseHasSeedBasedOnProvidedSeed((PerlinNoise)billowNoise.BaseNoise);
        }

        [Test]
        public void CreatesRidgedNoiseOnRidgedNoiseSettings()
		{
            GivenANewNoiseFactory();
            WhenCreateIsCalledWithRidgedNoiseSettings();
            ThenRidgedNoiseIsCreated();
        }

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

		private void GivenANewNoiseFactory()
		{
            Container.Bind<Seed>().AsTransient().WithArguments(_seedValue);
            Container.Bind<NoiseFactory>().AsTransient();
        }

        private void WhenCreateIsCalledWithPerlinNoiseSettings()
        {
            PerlinNoiseSettings settings = Resources.Load<PerlinNoiseSettings>(_perlinNoiseSettingsPath);
            CreateNoise(settings);
        }

        private void WhenCreateIsCalledWithBillowNoiseSettings()
        {
            BillowNoiseSettings settings = Resources.Load<BillowNoiseSettings>(_perlinBasedBillowNoiseSettingsPath);
            CreateNoise(settings);
        }

        private void WhenCreateIsCalledWithRidgedNoiseSettings()
        {
            RidgedNoiseSettings settings = Resources.Load<RidgedNoiseSettings>(_perlinBasedRidgedNoiseSettingsPath);
            CreateNoise(settings);
        }

        private void WhenCreateIsCalledWithSettingsNull()
        {
            CreateNoise(null);
        }

        private void WhenCreateIsCalledWithInfiniteLoopedSettings()
        {
            BillowNoiseSettings settings = Resources.Load<BillowNoiseSettings>(_loopedNoiseSettingsPath);
            CreateNoise(settings);
        }

        private void WhenRecursionDepthLimitIsChangedToANegativeValue(NoiseFactory factory)
        {
            factory.RecursionDepthLimit = -1;
        }

        private void ThenAPerlinNoiseIsCreated()
        {
            Assert.True(_noise is PerlinNoise);
        }

        private void ThenBillowNoiseIsCreated()
        {
            Assert.True(_noise is BillowNoise);
        }

        private void ThenAnArgumentNullExceptionIsThrown(TestDelegate test)
        {
            Assert.Throws<ArgumentNullException>(test);
        }

        private void ThenPerlinNoiseHasSeedBasedOnProvidedSeed(PerlinNoise _noise)
        {
            PerlinNoise perlinNoise = (PerlinNoise)_noise;
            Seed seed = Container.Resolve<Seed>();
            int expected = seed.Random.Next();
            int actual = perlinNoise.Seed.SeedNumber;
            Assert.AreEqual(expected, actual);
        }

        private void ThenRidgedNoiseIsCreated()
        {
            Assert.True(_noise is RidgedNoise);
        }

        private void ThenARecursionDepthExceptionIsThrown(TestDelegate test)
        {
            Assert.Throws<NoiseFactory.RecursionDepthLimitReachedException>(test);
        }

        private void ThenAnArgumentOutOfRangeExceptionIsThrown(TestDelegate test)
        {
            Assert.Throws<ArgumentOutOfRangeException>(test);
        }

        private void CreateNoise(NoiseSettings settings)
		{
            NoiseFactory factory = Container.Resolve<NoiseFactory>();
            Seed seed = Container.Resolve<Seed>();
            _noise = factory.Create(settings, seed);
        }
    }
}