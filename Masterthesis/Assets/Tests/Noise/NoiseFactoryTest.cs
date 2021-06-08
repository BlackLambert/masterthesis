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
		private const string _perlinNoiseSettingsPath = "Noise/TestPerlinNoiseSettings";
		private const string _perlinBasedBillowNoiseSettingsPath = "Noise/TestPerlinBillowNoiseSettings";
        private const string _perlinBasedRidgedNoiseSettingsPath = "Noise/TestPerlinRidgedNoiseSettings";
        private const string _loopedNoiseSettingsPath = "Noise/TestLoopedBillowNoiseSettings";
        private const string _threeOctavedNoiseSettingsPath = "Noise/TestThreeOctavedNoiseSettings";
        private const string _emptyOctavedNoiseSettingsPath = "Noise/TestEmptyOctavedNoiseSettings";
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

        [Test(Description = "The Create method creates an OctaveNoise if provided with an OctaveNoiseSetting.")]
        public void CreatesOctaveNoiseOnOctaveNoiseSettings()
		{
            GivenANewNoiseFactory();
            OctaveNoiseSettings settings = Resources.Load<OctaveNoiseSettings>(_threeOctavedNoiseSettingsPath);
            WhenCreateIsCalledWithOctavedNoiseSettings(settings);
            ThenAnOctaveNoiseIsCreated();
        }

        [Test(Description = "A created OctaveNoise has as many Octaves as provided by the OctaveNoiseSettings")]
        public void ACreatedOctaveNoiseHasExpectedOctaveCount()
		{
            GivenANewNoiseFactory();
            OctaveNoiseSettings settings = Resources.Load<OctaveNoiseSettings>(_threeOctavedNoiseSettingsPath);
            WhenCreateIsCalledWithOctavedNoiseSettings(settings);
            ThenTheOctaveNoiseHasExpectedOctaveCount(settings);
        }

        [Test(Description = "The Octaves of a created OctaveNoise have the expected values based on the provided OctaveNoiseSettings")]
        public void OctavesOfCreatedOctaveNoiseHaveExpectedValues()
        {
            GivenANewNoiseFactory();
            OctaveNoiseSettings settings = Resources.Load<OctaveNoiseSettings>(_threeOctavedNoiseSettingsPath);
            WhenCreateIsCalledWithOctavedNoiseSettings(settings);
            ThenTheOctavesHaveValuesBasedOn(settings);
        }

        [Test(Description = "Creation of an octave noise throws an ArgumentException if the provided octave settings have no octaves.")]
        public void EmptyOctavesCauseException()
		{
            GivenANewNoiseFactory();
            OctaveNoiseSettings settings = Resources.Load<OctaveNoiseSettings>(_emptyOctavedNoiseSettingsPath);
            TestDelegate test = () => WhenCreateIsCalledWithOctavedNoiseSettings(settings);
            ThenAnArgumentExceptionIsThrown(test);
        }

		private void GivenANewNoiseFactory()
		{
            Container.Bind<Seed>().AsTransient().WithArguments(_seedValue);
            Container.Bind<NoiseFactory>().To<NoiseFactoryImpl>().AsTransient();
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

        private void WhenCreateIsCalledWithOctavedNoiseSettings(OctaveNoiseSettings settings)
        {
            CreateNoise(settings);
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

        private void ThenAnOctaveNoiseIsCreated()
        {
            Assert.True(_noise is OctaveNoise);
        }

        private void ThenTheOctaveNoiseHasExpectedOctaveCount(OctaveNoiseSettings settings)
        {
            OctaveNoise octaveNoise = (OctaveNoise)_noise;
            Assert.AreEqual(settings.Octaves.Count, octaveNoise.OctavesCopy.Count);
        }

        private void ThenTheOctavesHaveValuesBasedOn(OctaveNoiseSettings settings)
        {
            OctaveNoise octaveNoise = (OctaveNoise)_noise;
            List<OctaveNoise.Octave> octaves = octaveNoise.OctavesCopy;
            for (int i = 0; i < settings.Octaves.Count; i++)
			{
                OctaveSettings octaveSetting = settings.Octaves[i];
                OctaveNoise.Octave octave = octaves[i];
                Assert.AreEqual(octaveSetting.Amplitude, octave.Amplitude);
                Assert.AreEqual(octaveSetting.FrequencyFactor, octave.FrequencyFactor);
                Assert.AreEqual(octaveSetting.NoiseSettings.GetNoiseType(), octave.Noise.NoiseType);
            }
        }

        private void ThenAnArgumentExceptionIsThrown(TestDelegate test)
        {
            Assert.Throws<ArgumentException>(test);
        }

        private void CreateNoise(NoiseSettings settings)
		{
            NoiseFactory factory = Container.Resolve<NoiseFactory>();
            Seed seed = Container.Resolve<Seed>();
            _noise = factory.Create(settings, seed);
        }
    }
}