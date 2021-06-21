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
        private const string _threeOctavedNoiseSettingsPath = "Noise/TestThreeLayeredNoiseSettings";
        private const string _emptyOctavedNoiseSettingsPath = "Noise/TestEmptyLayeredNoiseSettings";
        private const string _octaveNoiseSettingsPath = "Noise/TestOctaveNoiseSettings";
        private const string _simplexNoiseSettingsPath = "Noise/TestSimplexNoiseSettings";
        private const string _noiseValueLimiterSettingsPath = "Noise/TestNoiseValueLimiterSettings";
        private Noise3D _noise;

		[Test]
        public void CreatesPerlinNoiseOnPerlinNoiseSettings()
        {
            GivenANewNoiseFactory();
            WhenCreateIsCalledWithPerlinNoiseSettings();
            ThenANoiseOfGivenTypeIsCreated(typeof(PerlinNoise));
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
            ThenANoiseOfGivenTypeIsCreated(typeof(BillowNoise));
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
            ThenANoiseOfGivenTypeIsCreated(typeof(RidgedNoise));
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
        public void CreatesLayerNoiseOnLayerNoiseSettings()
		{
            GivenANewNoiseFactory();
            LayeredNoiseSettings settings = Resources.Load<LayeredNoiseSettings>(_threeOctavedNoiseSettingsPath);
            WhenCreateIsCalledWithLayerNoiseSettings(settings);
            ThenANoiseOfGivenTypeIsCreated(typeof(LayeredNoise));
        }

        [Test(Description = "A created LayeredNoise has as many Layers as provided by the LayeredNoiseSettings")]
        public void ACreatedLayerNoiseHasExpectedLayersCount()
		{
            GivenANewNoiseFactory();
            LayeredNoiseSettings settings = Resources.Load<LayeredNoiseSettings>(_threeOctavedNoiseSettingsPath);
            WhenCreateIsCalledWithLayerNoiseSettings(settings);
            ThenTheLayerNoiseHasExpectedLayersCount(settings);
        }

        [Test(Description = "The Layers of a created LayeredNoise have the expected values based on the provided LayeredNoiseSettings")]
        public void LayersOfCreatedLayerNoiseHaveExpectedValues()
        {
            GivenANewNoiseFactory();
            LayeredNoiseSettings settings = Resources.Load<LayeredNoiseSettings>(_threeOctavedNoiseSettingsPath);
            WhenCreateIsCalledWithLayerNoiseSettings(settings);
            ThenTheLayersHaveValuesBasedOn(settings);
        }

        [Test(Description = "Creation of an LayeredNoise throws an ArgumentException if the provided octave settings have no layers.")]
        public void EmptyLayersCauseException()
		{
            GivenANewNoiseFactory();
            LayeredNoiseSettings settings = Resources.Load<LayeredNoiseSettings>(_emptyOctavedNoiseSettingsPath);
            TestDelegate test = () => WhenCreateIsCalledWithLayerNoiseSettings(settings);
            ThenAnArgumentExceptionIsThrown(test);
        }

        [Test(Description = "The created LayeredNoise has expected mapping value")]
        public void CreatedLayeredNoiseHaasExpectedMapping()
        {
            GivenANewNoiseFactory();
            LayeredNoiseSettings settings = Resources.Load<LayeredNoiseSettings>(_threeOctavedNoiseSettingsPath);
            WhenCreateIsCalledWithLayerNoiseSettings(settings);
            ThenTheLayeredNoiseHasMappingBasedOn(settings);
        }

		[Test(Description = "The Create method creates an OctaveNoise if provided with an OctaveNoiseSetting.")]
        public void CreatesOctaveNoiseOnOctaveNoiseSettings()
        {
            GivenANewNoiseFactory();
            WhenCreateIsCalledWithOctavedNoiseSettings();
            ThenANoiseOfGivenTypeIsCreated(typeof(OctaveNoise));
        }

        [Test(Description = "The Create method creates an OctaveNoise with all values provided by the Arguments.")]
        public void CreatesOctaveNoiseWithExpectedValues()
        {
            GivenANewNoiseFactory();
            WhenCreateIsCalledWithOctavedNoiseSettings();
            ThenAnOctaveNoiseWithExpectedValuesIsCreated();
        }

        [Test(Description = "The Create method creates an SimplexNoise if provided with an SimplexNoiseSetting.")]
        public void CreatesSimplexNoiseOnSimplexNoiseSettings()
        {
            GivenANewNoiseFactory();
            WhenCreateIsCalledWithSimplexNoiseSettings();
            ThenANoiseOfGivenTypeIsCreated(typeof(SimplexNoise));
        }

        [Test(Description = "The SimplexNoise created has a seed based on provided base seed")]
        public void CreatedSimplexNoiseHasSeedBasedOnProvidedSeed()
        {
            GivenANewNoiseFactory();
            WhenCreateIsCalledWithSimplexNoiseSettings();
            ThenSimplexNoiseHasSeedBasedOnProvidedSeed((SimplexNoise)_noise);
        }

        [Test(Description = "The Create method creates an NoiseValueLimiter if provided with an NoiseValueLimiterSetting.")]
        public void CreatesNoiseValueLimiterOnNoiseValueLimiterSettings()
        {
            GivenANewNoiseFactory();
            WhenCreateIsCalledWithNoiseValueLimiterSettings();
            ThenANoiseOfGivenTypeIsCreated(typeof(NoiseValueLimiter));
        }

        [Test(Description = "The created NoiseValueLimiter has arguments provided by the settings")]
        public void CreatedNoiseValueLimiterHasExpectedProperties()
        {
            GivenANewNoiseFactory();
            WhenCreateIsCalledWithNoiseValueLimiterSettings();
            ThenTheNoiseValueLimiterPropertiesAreAsExpected();
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

        private void WhenCreateIsCalledWithLayerNoiseSettings(LayeredNoiseSettings settings)
        {
            CreateNoise(settings);
        }


        private void WhenCreateIsCalledWithOctavedNoiseSettings()
        {
            OctaveNoiseSettings settings = Resources.Load<OctaveNoiseSettings>(_octaveNoiseSettingsPath);
            CreateNoise(settings);
        }

        private void WhenCreateIsCalledWithSimplexNoiseSettings()
        {
            SimplexNoiseSettings settings = Resources.Load<SimplexNoiseSettings>(_simplexNoiseSettingsPath);
            CreateNoise(settings);
        }

        private void WhenCreateIsCalledWithNoiseValueLimiterSettings()
        {
            NoiseValueLimiterSettings settings = Resources.Load<NoiseValueLimiterSettings>(_noiseValueLimiterSettingsPath);
            CreateNoise(settings);
        }

        private void ThenAnArgumentNullExceptionIsThrown(TestDelegate test)
        {
            Assert.Throws<ArgumentNullException>(test);
        }

        private void ThenPerlinNoiseHasSeedBasedOnProvidedSeed(PerlinNoise noise)
        {
            Seed seed = Container.Resolve<Seed>();
            int expected = seed.Random.Next();
            int actual = noise.Seed.SeedNumber;
            Assert.AreEqual(expected, actual);
        }

        private void ThenARecursionDepthExceptionIsThrown(TestDelegate test)
        {
            Assert.Throws<NoiseFactory.RecursionDepthLimitReachedException>(test);
        }

        private void ThenAnArgumentOutOfRangeExceptionIsThrown(TestDelegate test)
        {
            Assert.Throws<ArgumentOutOfRangeException>(test);
        }

        private void ThenTheLayerNoiseHasExpectedLayersCount(LayeredNoiseSettings settings)
        {
            LayeredNoise octaveNoise = (LayeredNoise)_noise;
            Assert.AreEqual(settings.Layers.Count, octaveNoise.Layers.Length);
        }

        private void ThenTheLayersHaveValuesBasedOn(LayeredNoiseSettings settings)
        {
            LayeredNoise layeredNoise = (LayeredNoise)_noise;
            LayeredNoise.NoiseLayer[] octaves = layeredNoise.Layers;
            for (int i = 0; i < settings.Layers.Count; i++)
			{
                NoiseLayerSettings octaveSetting = settings.Layers[i];
                LayeredNoise.NoiseLayer octave = octaves[i];
                Assert.AreEqual(octaveSetting.Weight, octave.Weight);
                Assert.AreEqual(octaveSetting.FrequencyFactor, octave.FrequencyFactor);
                Assert.AreEqual(octaveSetting.NoiseSettings.GetNoiseType(), octave.Noise.NoiseType);
            }
        }

        private void ThenTheLayeredNoiseHasMappingBasedOn(LayeredNoiseSettings settings)
        {
            LayeredNoise layeredNoise = (LayeredNoise)_noise;
            Assert.AreEqual(settings.Mapping, layeredNoise.Mapping);
        }

        private void ThenAnArgumentExceptionIsThrown(TestDelegate test)
        {
            Assert.Throws<ArgumentException>(test);
        }

        private void ThenAnOctaveNoiseWithExpectedValuesIsCreated()
        {
            OctaveNoiseSettings settings = Resources.Load<OctaveNoiseSettings>(_octaveNoiseSettingsPath);
            OctaveNoise noise = (OctaveNoise)_noise;
            Assert.AreEqual(settings.StartFrequency, noise.StartFrequency);
            Assert.AreEqual(settings.StartWeight, noise.StartWeight);
            Assert.AreEqual(settings.BaseNoise.GetNoiseType(), noise.BaseNoise.NoiseType);
            Assert.AreEqual(settings.OctavesCount, noise.OctavesCount);
        }

        private void ThenSimplexNoiseHasSeedBasedOnProvidedSeed(SimplexNoise noise)
        {
            Seed seed = Container.Resolve<Seed>();
            int expected = seed.Random.Next();
            int actual = noise.Seed.SeedNumber;
            Assert.AreEqual(expected, actual);
        }

        private void ThenANoiseOfGivenTypeIsCreated(Type type)
        {
            Assert.AreEqual(type, _noise.GetType());
        }

        private void ThenTheNoiseValueLimiterPropertiesAreAsExpected()
        {
            NoiseValueLimiterSettings settings = Resources.Load<NoiseValueLimiterSettings>(_noiseValueLimiterSettingsPath);
            NoiseValueLimiter limiter = (NoiseValueLimiter)_noise;
            Assert.AreEqual(settings.ValueLimits.x, limiter.Limits.x);
            Assert.AreEqual(settings.ValueLimits.y, limiter.Limits.y);
            Assert.AreEqual(settings.BaseNoise.GetNoiseType(), limiter.BaseNoise.NoiseType);
        }

        private void CreateNoise(NoiseSettings settings)
		{
            NoiseFactory factory = Container.Resolve<NoiseFactory>();
            Seed seed = Container.Resolve<Seed>();
            _noise = factory.Create(settings, seed);
        }
    }
}