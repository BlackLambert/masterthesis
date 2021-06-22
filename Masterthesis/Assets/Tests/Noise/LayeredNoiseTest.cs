using Zenject;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using UnityEngine;

namespace SBaier.Master.Test
{
	[TestFixture]
	public class LayeredNoiseTest : NoiseTest
	{
		private const int _randomSeed = 43623;
		private const LayeredNoise.MappingMode _defaultMappingMode = LayeredNoise.MappingMode.NegOneToOne;
		private const double _epsilon = 0.0001;
		private readonly Vector3[] _testEvaluationPoints = new Vector3[]
		{
			Vector3.zero,
			new Vector3(-1.7f, 5.3f, -9.0f),
			new Vector3(6.2f, 12.3f, 1.2f),
			new Vector3(-0.2f, -5.3f, -21.2f)
		};

		protected override NoiseType ExpectedNoiseType => NoiseType.Layered;

		private LayeredNoise _noise;
		private List<LayeredNoise.NoiseLayer> _layers;

		[Test (Description ="The LayersCopy property returns a copy of the layers put into the constructor.")]
		public void LayersReturnProvidedValues()
		{
			GivenALayeredNoise(_defaultMappingMode);
			ThenLayersCopyReturnProvidedValues();
		}

		[Test (Description ="The Mapping property returns a the MappingMode put into the constructor.")]
		public void MappingReturnsProvidedValue()
		{
			foreach (LayeredNoise.MappingMode mode in Enum.GetValues(typeof(LayeredNoise.MappingMode)))
			{
				GivenALayeredNoise(mode);
				ThenMappingReturnProvidedValue(mode);
				Teardown();
				Setup();
			}
		}

		[Test(Description = "Evaluate with NegOneToOne Mapping returns expected values")]
		public void EvaluateReturnsNegOneToOneLayeredValue()
		{
			GivenALayeredNoise(LayeredNoise.MappingMode.NegOneToOne);
			foreach (Vector3 testEvaluationPoint in _testEvaluationPoints)
			{
				ThenEvaluateReturnsLayeredValue(LayeredNoise.MappingMode.NegOneToOne, testEvaluationPoint);
			}
		}

		[Test(Description = "Evaluate with ZeroToOne Mapping returns expected values")]
		public void EvaluateReturnsZeroToOneLayeredValue()
		{
			GivenALayeredNoise(LayeredNoise.MappingMode.ZeroToOne);
			foreach (Vector3 testEvaluationPoint in _testEvaluationPoints)
			{
				ThenEvaluateReturnsLayeredValue(LayeredNoise.MappingMode.ZeroToOne, testEvaluationPoint);
			}
		}

		protected override void GivenANew3DNoise()
		{
			Container.Bind<List<LayeredNoise.NoiseLayer>>().FromMethod(CreateValidLayers).AsTransient();
			Container.Bind(typeof(Noise3D)).To<LayeredNoise>().AsTransient().WithArguments(_defaultMappingMode);
		}

		protected void GivenALayeredNoise(LayeredNoise.MappingMode mappingMode)
		{
			Container.Bind<List<LayeredNoise.NoiseLayer>>().FromMethod(CreateValidLayers).AsSingle();
			Container.Bind(typeof(LayeredNoise)).To<LayeredNoise>().AsTransient().WithArguments(mappingMode);
		}

		private void ThenLayersCopyReturnProvidedValues()
		{
			_layers = Container.Resolve<List<LayeredNoise.NoiseLayer>>();
			LayeredNoise noise = Container.Resolve<LayeredNoise>();
			LayeredNoise.NoiseLayer[] noiseLayers = noise.Layers;

			Assert.True(Enumerable.SequenceEqual(_layers, noiseLayers));
			Assert.AreEqual(_layers, noiseLayers);
		}

		private void ThenEvaluateReturnsLayeredValue(LayeredNoise.MappingMode mappingMode, Vector3 testEvaluationPoint)
		{
			_layers = Container.Resolve<List<LayeredNoise.NoiseLayer>>();
			LayeredNoise noise = Container.Resolve<LayeredNoise>();
			LayeredNoise.NoiseLayer[] noiseLayers = noise.Layers;

			float sum = _layers.Sum((layer) => OctaveNoiseSum(layer, mappingMode, testEvaluationPoint));
			if (mappingMode == LayeredNoise.MappingMode.NegOneToOne)
				sum += 0.5f;
			float expected = Mathf.Clamp01(sum);
			float actual = noise.Evaluate3D(testEvaluationPoint);

			Assert.AreEqual(expected, actual, _epsilon);
		}

		private float OctaveNoiseSum(LayeredNoise.NoiseLayer layer, LayeredNoise.MappingMode mappingMode, Vector3 testEvaluationPoint)
		{
			float ff = layer.FrequencyFactor;
			float evaluatedValue = layer.Noise.Evaluate3D(testEvaluationPoint * ff);
			if (mappingMode == LayeredNoise.MappingMode.NegOneToOne)
				evaluatedValue -= 0.5f;
			return evaluatedValue * layer.Weight;
		}

		private void ThenMappingReturnProvidedValue(LayeredNoise.MappingMode expected)
		{
			LayeredNoise noise = Container.Resolve<LayeredNoise>();
			Assert.AreEqual(expected, noise.Mapping);
		}

		private List<LayeredNoise.NoiseLayer> CreateValidLayers()
		{
			return new List<LayeredNoise.NoiseLayer>()
			{
				new LayeredNoise.NoiseLayer(CreateNoise(), 1, 1),
				new LayeredNoise.NoiseLayer(CreateNoise(), 0.75f, 0.5f),
				new LayeredNoise.NoiseLayer(CreateNoise(), 0.5f, 0.25f),
				new LayeredNoise.NoiseLayer(CreateNoise(), 0.25f, 0.1f),
				new LayeredNoise.NoiseLayer(CreateNoise(), 1.8f, 10f),
				new LayeredNoise.NoiseLayer(CreateNoise(), 2.1f, 0.75f)
			};
		}

		private Noise3D CreateNoise()
		{
			Seed seed = new Seed(_randomSeed);
			PerlinNoise noise = new PerlinNoise(seed);
			return noise;
		}
	}
}