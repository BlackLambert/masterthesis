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

		private OctaveNoise _noise;
		private ICollection<OctaveNoise.Octave> _octaves;

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

		protected override void GivenANew3DNoise()
		{
			Container.Bind<ICollection<OctaveNoise.Octave>>().FromMethod(CreateValidOctaves).AsSingle();
			Container.Bind(typeof(Noise3D), typeof(OctaveNoise)).To<OctaveNoise>().AsTransient();
		}

		private void ThenOctavesReturnsProvidedValues()
		{
			_octaves = Container.Resolve<ICollection<OctaveNoise.Octave>>();
			_noise = Container.Resolve<OctaveNoise>();
			Assert.True(Enumerable.SequenceEqual(_octaves, _noise.Octaves));
		}

		private void ThenEvaluateReturnsOctavedValue()
		{
			_octaves = Container.Resolve<ICollection<OctaveNoise.Octave>>();
			_noise = Container.Resolve<OctaveNoise>();
			double sum = _octaves.Sum(o => o.Noise.Evaluate(_testEvaluationPoint.x, _testEvaluationPoint.y, _testEvaluationPoint.z) * o.Amplitude);
			double expected = (sum > 1) ? 1 : (sum < 0) ? 0 : sum;
			double actual = _noise.Evaluate(_testEvaluationPoint.x, _testEvaluationPoint.y, _testEvaluationPoint.z);
			Assert.AreEqual(expected, actual);
		}

		private ICollection<OctaveNoise.Octave> CreateValidOctaves()
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
	}
}