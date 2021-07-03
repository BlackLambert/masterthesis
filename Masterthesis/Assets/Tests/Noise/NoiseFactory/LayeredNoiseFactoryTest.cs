using NUnit.Framework;
using System;

namespace SBaier.Master.Test
{
    [TestFixture]
    public class LayeredNoiseFactoryTest : SpecificNoiseFactoryTest
    {
        private const string _threeLayeredNoiseSettingsPath = "Noise/TestThreeLayeredNoiseSettings";
        private const string _emptyLayeredNoiseSettingsPath = "Noise/TestEmptyLayeredNoiseSettings";

        protected override Type ExpectedNoiseType => typeof(LayeredNoise);


        [Test(Description = "A created LayeredNoise has as many Layers as provided by the LayeredNoiseSettings")]
        public void ACreatedLayerNoiseHasExpectedLayersCount()
        {
            GivenANewNoiseFactory();
            GivenATestNoiseSetting();
            WhenCreateIsCalledWithTestNoiseSettings();
            ThenTheLayerNoiseHasExpectedLayersCount();
        }

        [Test(Description = "The Layers of a created LayeredNoise have the expected values based on the provided LayeredNoiseSettings")]
        public void LayersOfCreatedLayerNoiseHaveExpectedValues()
        {
            GivenANewNoiseFactory();
            GivenATestNoiseSetting();
            WhenCreateIsCalledWithTestNoiseSettings();
            ThenTheLayersHaveValuesBasedOn();
        }

        [Test(Description = "Creation of an LayeredNoise throws an ArgumentException if the provided octave settings have no layers.")]
        public void EmptyLayersCauseException()
        {
            GivenANewNoiseFactory();
            GivenAnEmptyLayersNoiseSetting();
            TestDelegate test = () => WhenCreateIsCalledWithTestNoiseSettings();
            ThenAnArgumentExceptionIsThrown(test);
        }

		[Test(Description = "The created LayeredNoise has expected mapping value")]
        public void CreatedLayeredNoiseHasExpectedMapping()
        {
            GivenANewNoiseFactory();
            GivenATestNoiseSetting();
            WhenCreateIsCalledWithTestNoiseSettings();
            ThenTheLayeredNoiseHasMappingBasedOn();
        }

        protected override void GivenATestNoiseSetting()
        {
            Container.Bind(typeof(NoiseSettings), typeof(LayeredNoiseSettings)).FromResource(_threeLayeredNoiseSettingsPath).AsSingle();
        }

        private void GivenAnEmptyLayersNoiseSetting()
        {
            Container.Bind(typeof(NoiseSettings), typeof(LayeredNoiseSettings)).FromResource(_emptyLayeredNoiseSettingsPath).AsSingle();
        }

        private void ThenAnArgumentExceptionIsThrown(TestDelegate test)
        {
            Assert.Throws<ArgumentException>(test);
        }

        private void ThenTheLayerNoiseHasExpectedLayersCount()
        {
            LayeredNoiseSettings settings = Container.Resolve<LayeredNoiseSettings>();
            LayeredNoise octaveNoise = (LayeredNoise)_noise;
            Assert.AreEqual(settings.Layers.Count, octaveNoise.Layers.Length);
        }

        private void ThenTheLayersHaveValuesBasedOn()
        {
            LayeredNoiseSettings settings = Container.Resolve<LayeredNoiseSettings>();
            LayeredNoise layeredNoise = (LayeredNoise)_noise;
            Noise3D[] layers = layeredNoise.Layers;
            for (int i = 0; i < settings.Layers.Count; i++)
            {
                NoiseSettings layerSetting = settings.Layers[i];
                Noise3D layer = layers[i];
                Assert.AreEqual(layerSetting.Weight, layer.Weight);
                Assert.AreEqual(layerSetting.FrequencyFactor, layer.FrequencyFactor);
                Assert.AreEqual(layerSetting.GetNoiseType(), layer.NoiseType);
            }
        }

        private void ThenTheLayeredNoiseHasMappingBasedOn()
        {
            LayeredNoiseSettings settings = Container.Resolve<LayeredNoiseSettings>();
            LayeredNoise layeredNoise = (LayeredNoise)_noise;
            Assert.AreEqual(settings.Mapping, layeredNoise.Mapping);
        }
    }
}