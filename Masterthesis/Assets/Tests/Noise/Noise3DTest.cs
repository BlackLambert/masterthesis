using Zenject;
using NUnit.Framework;
using UnityEngine;

namespace SBaier.Master.Test
{
    [TestFixture]
    public abstract class Noise3DTest : ZenjectUnitTestFixture
    {
		private const int _testSamplesCount = 1000;
		private const int _randomSeed = 1234;
		private readonly Vector2 _valueRange = new Vector2(0, 1);
		protected abstract double AverageDelta { get; }
		protected abstract double ExpectedAverage { get; }
		private readonly Vector3 _testEvaluationPoint = new Vector3(3.2f, 5.8f, -1.7f);
		private readonly Vector3Int _sampleRange = new Vector3Int(100, 100, 100);

		[Test]
		public void AllEvaluatedValuesAreInExpectedRange()
		{
			GivenANew3DNoise();
			ThenAllEvaluatedValuesAreInExpectedRange();
		}

		[Test]
		public void TheAverageOfEvaluatedValuesIsAsExpected()
		{
			GivenANew3DNoise();
			ThenTheAverageOfEvaluatedValuesIsAsExpected();
		}

		[Test]
		public void EvaluatedDataOfTwoNoisesWithSameSeedAreEqual()
		{
			GivenANew3DNoise();
			ThenEvaluatedDataOfTwoNoisesWithSameSeedAreEqual();
		}

		[Test]
		public void TwoEvaluatedValuesWithTheSameInputAreEqual()
		{
			GivenANew3DNoise();
			ThenTwoEvaluatedValuesWithTheSameInputAreEqual();
		}

		protected abstract void GivenANew3DNoise();

		private void ThenAllEvaluatedValuesAreInExpectedRange()
		{
			Noise3D noise = Container.Resolve<Noise3D>();
			System.Random random = new System.Random(_randomSeed);

			for (int i = 0; i < _testSamplesCount; i++)
				TestRandomSampleWithinRange(noise, random);
		}

		private void ThenTheAverageOfEvaluatedValuesIsAsExpected()
		{
			Noise3D noise = Container.Resolve<Noise3D>();
			System.Random random = new System.Random(_randomSeed);
			double sum = 0;

			for (int i = 0; i < _testSamplesCount; i++)
				sum += EvaluateRandom3DSample(noise, random);

			double actual = sum / _testSamplesCount;
			Assert.AreEqual(ExpectedAverage, actual, AverageDelta);
		}

		private void ThenEvaluatedDataOfTwoNoisesWithSameSeedAreEqual()
		{
			Noise3D noiseOne = Container.Resolve<Noise3D>();
			Noise3D noiseTwo = Container.Resolve<Noise3D>();
			System.Random randomOne = new System.Random(_randomSeed);
			System.Random randomTwo = new System.Random(_randomSeed);

			for (int i = 0; i < _testSamplesCount; i++)
			{
				double valueOne = EvaluateRandom3DSample(noiseOne, randomOne);
				double valueTwo = EvaluateRandom3DSample(noiseTwo, randomTwo);

				Assert.AreEqual(valueOne, valueTwo);
			}
		}

		private void ThenTwoEvaluatedValuesWithTheSameInputAreEqual()
		{
			Noise3D noiseOne = Container.Resolve<Noise3D>();
			double evaluatedValueOne = noiseOne.Evaluate(
				_testEvaluationPoint.x,
				_testEvaluationPoint.y,
				_testEvaluationPoint.z);
			double evaluatedValueTwo = noiseOne.Evaluate(
				_testEvaluationPoint.x,
				_testEvaluationPoint.y,
				_testEvaluationPoint.z);
			Assert.AreEqual(evaluatedValueOne, evaluatedValueTwo);
		}

		private void TestRandomSampleWithinRange(Noise3D noise, System.Random random)
		{
			double actual = EvaluateRandom3DSample(noise, random);
			Assert.GreaterOrEqual(actual, _valueRange.x);
			Assert.LessOrEqual(actual, _valueRange.y);
		}

		private double EvaluateRandom3DSample(Noise3D noise, System.Random random)
		{
			double x = random.NextDouble() * random.Next(_sampleRange.x);
			double y = random.NextDouble() * random.Next(_sampleRange.y);
			double z = random.NextDouble() * random.Next(_sampleRange.z);
			return noise.Evaluate(x, y, z);
		}
	}
}