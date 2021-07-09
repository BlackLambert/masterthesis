using Zenject;
using NUnit.Framework;
using System;
using Moq;
using UnityEngine;

namespace SBaier.Master.Test
{
    [TestFixture]
    public class OctaveNoiseTest : NoiseTest
    {
		private const int _octavesCount = 3;
		private const float _startFrequency = 4;
		private const float _startWeight = 0.5f;
		private const int _testSeed = 1234;
		private readonly Vector3 _testValue = new Vector3(2.1f, 4.7f, -2.4f);

		private OctaveNoise _noise;
		private Noise3D _baseNoise;
		private OctaveNoise.Arguments _args;

		protected override NoiseType ExpectedNoiseType => NoiseType.Octave;

		
		[Test(Description = "The OctavesCount property returns value put into the constructor")]
        public void OctavesCount_ReturnsExpectedValue()
        {
			GivenANew3DNoise();
			ThenOctavesCountReturns(_octavesCount);
        }

		[Test(Description = "The BaseNoise property returns value put into the constructor")]
		public void BaseNoise_ReturnsExpectedValue()
		{
			GivenANew3DNoise();
			ThenBaseNoiseReturns(_baseNoise);
		}

		[Test(Description = "The evaluated values are as expected")]
		public void Evaluate_ReturnsExpectedValue()
		{
			GivenANew3DNoise();
			WhenBaseValuesAreSetTo(_noise, _startFrequency, _startWeight);
			ThenTheTestValueEvaluatesToExpectedValue();
		}

		protected override void GivenANew3DNoise()
		{
			GivenAnOctaveNoise(CreateValidArgs);
		}

		private void GivenAnOctaveNoise(Func<OctaveNoise.Arguments> args)
		{
			Container.Bind<OctaveNoise.Arguments>().FromMethod(args).AsTransient();
			Container.Bind(typeof(Noise3D), typeof(OctaveNoise)).To<OctaveNoise>().AsTransient();
			_noise = Container.Resolve<OctaveNoise>();
		}

		private void ThenOctavesCountReturns(int octaves)
		{
			Assert.AreEqual(octaves, _noise.OctavesCount);
		}

		private void ThenBaseNoiseReturns(Noise3D baseNoise)
		{
			Assert.AreEqual(baseNoise, _noise.BaseNoise);
		}

		private void ThenTheTestValueEvaluatesToExpectedValue()
		{
			float actual = _noise.Evaluate3D(_testValue);
			float expected = CreateExpected(_testValue);
			Assert.AreEqual(expected, actual);
		}

		private Noise3D CreateBaseNoise()
		{
			return new PerlinNoise(new Seed(_testSeed));
		}

		private OctaveNoise.Arguments CreateValidArgs()
		{
			_baseNoise = CreateBaseNoise();
			return new OctaveNoise.Arguments(_octavesCount, _baseNoise);
		}

		private float CreateExpected(Vector3 testValue)
		{
			float result = 0;
			for(int i = 0; i < _octavesCount; i++)
			{
				float factor = Mathf.Pow(2, i);
				float ff = _startFrequency * factor;
				float weight = 1 / factor;
				float baseValue = _baseNoise.Evaluate3D(testValue * ff) * 2 - 1;
				result += baseValue * weight;
			}
			result = (result + 1) / 2;
			return (float) MathUtil.Clamp01(result * _noise.Weight);
		}
	}
}