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
		private readonly Vector2 _startWeightRange = new Vector2(0, 1); 
		private const double _startWeight = 0.5;
		private const float _startFrequencySmallerOne = -1.3f;
		private const int _testSeed = 1234;
		private const double _doubleDelta = 0.001;
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

		[Test(Description = "The StartFrequency property returns value put into the constructor")]
		public void StartFrequency_ReturnsExpectedValue()
		{
			GivenANew3DNoise();
			ThenStartFrequencyReturns(_startFrequency);
		}

		[Test(Description = "The StartWeight property returns value put into the constructor")]
		public void StartWeight_ReturnsExpectedValue()
		{
			GivenANew3DNoise();
			ThenStartWeightReturns(_startWeight);
		}

		[Test(Description = "The constructor throws an ArgumentOutOfRangeException if the Start Weight is out of range.")]
		public void ThrowsExceptionOnStartWeightOutOfRange()
		{
			TestDelegate test = () => CreateTestArgsWithStartWeightSmallerThanRange();
			ThenThrowsArgumentOutOfRangeException(test);
		}

		[Test(Description = "The constructor throws an ArgumentOutOfRangeException if the Start Frequency is smaller than 1.")]
		public void ThrowsExceptionOnStartFrequencyOutOfRange()
		{
			TestDelegate test = () => CreateTestArgsWithStartFrequencySmallerThanOne();
			ThenThrowsArgumentOutOfRangeException(test);
		}

		[Test(Description = "The evaluated values are as expected")]
		public void Evaluate_ReturnsExpectedValue()
		{
			GivenANew3DNoise();
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

		private void ThenStartFrequencyReturns(double startFrequency)
		{
			Assert.AreEqual(startFrequency, _noise.StartFrequency);
		}

		private void ThenStartWeightReturns(double startWeight)
		{
			Assert.AreEqual(startWeight, _noise.StartWeight);
		}

		private void ThenThrowsArgumentOutOfRangeException(TestDelegate test)
		{
			Assert.Throws<ArgumentOutOfRangeException>(test);
		}

		private void ThenTheTestValueEvaluatesToExpectedValue()
		{
			double actual = _noise.Evaluate(_testValue.x, _testValue.y, _testValue.z);
			double expected = CreateExpected(_testValue);
			Assert.AreEqual(expected, actual);
		}

		private Noise3D CreateBaseNoise()
		{
			return new PerlinNoise(new Seed(_testSeed));
		}

		private OctaveNoise.Arguments CreateValidArgs()
		{
			_baseNoise = CreateBaseNoise();
			return new OctaveNoise.Arguments(_octavesCount, _baseNoise, _startFrequency, _startWeight);
		}

		private OctaveNoise.Arguments CreateTestArgsWithStartWeightSmallerThanRange()
		{
			_baseNoise = CreateBaseNoise();
			return new OctaveNoise.Arguments(_octavesCount, _baseNoise, _startFrequency, _startWeightRange.x - _doubleDelta);
		}

		private OctaveNoise.Arguments CreateTestArgsWithStartWeightLargerThanRange()
		{
			_baseNoise = CreateBaseNoise();
			return new OctaveNoise.Arguments(_octavesCount, _baseNoise, _startFrequency, _startWeightRange.y + _doubleDelta);
		}

		private OctaveNoise.Arguments CreateTestArgsWithStartFrequencySmallerThanOne()
		{
			_baseNoise = CreateBaseNoise();
			return new OctaveNoise.Arguments(_octavesCount, _baseNoise, _startFrequencySmallerOne, 1 - _doubleDelta);
		}

		private double CreateExpected(Vector3 testValue)
		{
			double result = 0;
			for(int i = 0; i < _octavesCount; i++)
			{
				double factor = Math.Pow(2, i);
				double ff = _startFrequency * factor;
				double weight = _startWeight / factor;
				result += (_baseNoise.Evaluate(testValue.x * ff, testValue.y * ff, testValue.z * ff) - 0.5f) * weight;
			}
			return Clamp01(result + 0.5f);
		}

		private double Clamp01(double result)
		{
			return (result > 1) ? 1 : (result < 0) ? 0 : result;
		}
	}
}