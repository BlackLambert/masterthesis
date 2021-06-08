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
	public class OctaveNoiseTest : NoiseTest
	{
		private const int _randomSeed = 43623;
		private readonly Vector3 _testEvaluationPoint = new Vector3(3.2f, 5.8f, -1.7f);

		protected override NoiseType ExpectedNoiseType => NoiseType.Octave;

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
			Container.Bind<List<OctaveNoise.Octave>>().FromMethod(CreateValidOctaves).AsSingle();
			Container.Bind(typeof(Noise3D), typeof(OctaveNoise)).To<OctaveNoise>().AsTransient();
		}

		private void ThenOctavesReturnsProvidedValues()
		{
			List<OctaveNoise.Octave> octaves = Container.Resolve<List<OctaveNoise.Octave>>();
			OctaveNoise noise = Container.Resolve<OctaveNoise>();
			Assert.True(Enumerable.SequenceEqual(octaves, noise.OctavesCopy));
		}

		private void ThenEvaluateReturnsOctavedValue()
		{
			List<OctaveNoise.Octave> octaves = Container.Resolve<List<OctaveNoise.Octave>>();
			OctaveNoise noise = Container.Resolve<OctaveNoise>();
			List<OctaveNoise.Octave> noiseOctaves = noise.OctavesCopy;
			Assert.AreEqual(octaves, noiseOctaves);
			double sum = octaves.Sum(OctaveNoiseSum);
			double expected = Clamp01(sum + 0.5);
			double actual = noise.Evaluate(_testEvaluationPoint.x, _testEvaluationPoint.y, _testEvaluationPoint.z);
			Assert.AreEqual(expected, actual);
		}

		private double OctaveNoiseSum(OctaveNoise.Octave octave)
		{
			double ff = octave.FrequencyFactor;
			double evaluatedValue = octave.Noise.Evaluate(_testEvaluationPoint.x * ff, _testEvaluationPoint.y * ff, _testEvaluationPoint.z * ff) - 0.5;
			return evaluatedValue * octave.Amplitude;
		}

		private void ThenOctavesCopyReturnsCopyOfProvidedOctaves()
		{
			List<OctaveNoise.Octave> octaves = Container.Resolve<List<OctaveNoise.Octave>>();
			OctaveNoise noise = Container.Resolve<OctaveNoise>();
			List<OctaveNoise.Octave> octavesCopy = noise.OctavesCopy;
			Assert.AreEqual(octaves, octavesCopy);
			Assert.AreNotSame(octaves, octavesCopy);
		}

		private List<OctaveNoise.Octave> CreateValidOctaves()
		{
			return new List<OctaveNoise.Octave>()
			{
				new OctaveNoise.Octave(CreateNoise(), 1, 1),
				new OctaveNoise.Octave(CreateNoise(), 0.75f, 0.5f),
				new OctaveNoise.Octave(CreateNoise(), 0.5f, 0.25f),
				new OctaveNoise.Octave(CreateNoise(), 0.25f, 0.1f)
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