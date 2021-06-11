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
		private readonly Vector3 _testEvaluationPoint = new Vector3(3.2f, 5.8f, -1.7f);

		protected override NoiseType ExpectedNoiseType => NoiseType.Layered;

		[Test]
		public void OctavesReturnsProvidedValues()
		{
			GivenANew3DNoise();
			ThenOctavesReturnsProvidedValues();
		}

		[Test]
		public void EvaluateReturnsOctavedValue()
		{
			GivenANew3DNoise();
			ThenEvaluateReturnsOctavedValue();
		}

		[Test (Description = "The OctabesCopy property returns a copy of the octave list provided as argument to the constructor.")]
		public void OctavesCopyReturnsExpectedValue()
		{
			GivenANew3DNoise();
			ThenOctavesCopyReturnsCopyOfProvidedOctaves();
		}

		protected override void GivenANew3DNoise()
		{
			Container.Bind<List<LayeredNoise.NoiseLayer>>().FromMethod(CreateValidOctaves).AsSingle();
			Container.Bind(typeof(Noise3D), typeof(LayeredNoise)).To<LayeredNoise>().AsTransient();
		}

		private void ThenOctavesReturnsProvidedValues()
		{
			List<LayeredNoise.NoiseLayer> octaves = Container.Resolve<List<LayeredNoise.NoiseLayer>>();
			LayeredNoise noise = Container.Resolve<LayeredNoise>();
			Assert.True(Enumerable.SequenceEqual(octaves, noise.OctavesCopy));
		}

		private void ThenEvaluateReturnsOctavedValue()
		{
			List<LayeredNoise.NoiseLayer> octaves = Container.Resolve<List<LayeredNoise.NoiseLayer>>();
			LayeredNoise noise = Container.Resolve<LayeredNoise>();
			List<LayeredNoise.NoiseLayer> noiseOctaves = noise.OctavesCopy;
			Assert.AreEqual(octaves, noiseOctaves);
			double sum = octaves.Sum(OctaveNoiseSum);
			double expected = Clamp01(sum + 0.5);
			double actual = noise.Evaluate(_testEvaluationPoint.x, _testEvaluationPoint.y, _testEvaluationPoint.z);
			Assert.AreEqual(expected, actual);
		}

		private double OctaveNoiseSum(LayeredNoise.NoiseLayer octave)
		{
			double ff = octave.FrequencyFactor;
			double evaluatedValue = octave.Noise.Evaluate(_testEvaluationPoint.x * ff, _testEvaluationPoint.y * ff, _testEvaluationPoint.z * ff) - 0.5;
			return evaluatedValue * octave.Amplitude;
		}

		private void ThenOctavesCopyReturnsCopyOfProvidedOctaves()
		{
			List<LayeredNoise.NoiseLayer> octaves = Container.Resolve<List<LayeredNoise.NoiseLayer>>();
			LayeredNoise noise = Container.Resolve<LayeredNoise>();
			List<LayeredNoise.NoiseLayer> octavesCopy = noise.OctavesCopy;
			Assert.AreEqual(octaves, octavesCopy);
			Assert.AreNotSame(octaves, octavesCopy);
		}

		private List<LayeredNoise.NoiseLayer> CreateValidOctaves()
		{
			return new List<LayeredNoise.NoiseLayer>()
			{
				new LayeredNoise.NoiseLayer(CreateNoise(), 1, 1),
				new LayeredNoise.NoiseLayer(CreateNoise(), 0.75f, 0.5f),
				new LayeredNoise.NoiseLayer(CreateNoise(), 0.5f, 0.25f),
				new LayeredNoise.NoiseLayer(CreateNoise(), 0.25f, 0.1f)
			};
		}

		private Noise3D CreateNoise()
		{
			Seed seed = new Seed(_randomSeed);
			PerlinNoise noise = new PerlinNoise(seed);
			return noise;
		}

		private double Clamp01(double result)
		{
			return (result > 1) ? 1 : (result < 0) ? 0 : result;
		}
	}
}