using Zenject;
using NUnit.Framework;
using UnityEngine;

namespace SBaier.Master.Test
{
    [TestFixture]
    public abstract class Noise2DTest : ZenjectUnitTestFixture
    {
		private const int _testSamplesCount = 1000;
		private const int _randomSeed = 1234;
		private readonly Vector2 _valueRange = new Vector2(0, 1);
		private readonly Vector2 _testEvaluationPoint = new Vector2(3.2f, 5.8f);
		private readonly Vector2Int _sampleRange = new Vector2Int(100, 100);

		protected abstract double AverageDelta { get; }
		protected abstract double ExpectedAverage { get; }


		[Test]
		public void AllEvaluatedValuesAreInExpectedRange()
		{
			GivenANew2DNoise();
			ThenAllEvaluatedValuesAreInExpectedRange();
		}

		[Test]
		public void TheAverageOfEvaluatedValuesIsAsExpected()
		{
			GivenANew2DNoise();
			ThenTheAverageOfEvaluatedValuesIsAsExpected();
		}

		[Test]
		public void EvaluatedDataOfTwoNoisesWithSameSeedAreEqual()
		{
			GivenANew2DNoise();
			ThenEvaluatedDataOfTwoNoisesWithSameSeedAreEqual();
		}

		[Test]
		public void TwoEvaluatedValuesWithTheSameInputAreEqual()
		{
			GivenANew2DNoise();
			ThenTwoEvaluatedValuesWithTheSameInputAreEqual();
		}

		protected abstract void GivenANew2DNoise();

		private void ThenAllEvaluatedValuesAreInExpectedRange()
		{
			Noise2D noise = Container.Resolve<Noise2D>();
			System.Random random = new System.Random(_randomSeed);

			for (int i = 0; i < _testSamplesCount; i++)
				TestRandomSampleWithinRange(noise, random);
		}

		private void ThenTheAverageOfEvaluatedValuesIsAsExpected()
		{
			Noise2D noise = Container.Resolve<Noise2D>();
			System.Random random = new System.Random(_randomSeed);
			double sum = 0;

			for (int i = 0; i < _testSamplesCount; i++)
				sum += EvaluateRandom2DSample(noise, random);

			double actual = sum / _testSamplesCount;
			Assert.AreEqual(ExpectedAverage, actual, AverageDelta);
		}

		private void ThenEvaluatedDataOfTwoNoisesWithSameSeedAreEqual()
		{
			Noise2D noiseOne = Container.Resolve<Noise2D>();
			Noise2D noiseTwo = Container.Resolve<Noise2D>();
			System.Random randomOne = new System.Random(_randomSeed);
			System.Random randomTwo = new System.Random(_randomSeed);

			for (int i = 0; i < _testSamplesCount; i++)
			{
				double valueOne = EvaluateRandom2DSample(noiseOne, randomOne);
				double valueTwo = EvaluateRandom2DSample(noiseTwo, randomTwo);

				Assert.AreEqual(valueOne, valueTwo);
			}
		}

		private void ThenTwoEvaluatedValuesWithTheSameInputAreEqual()
		{
			Noise2D noiseOne = Container.Resolve<Noise2D>();
			double evaluatedValueOne = noiseOne.Evaluate(
				_testEvaluationPoint.x,
				_testEvaluationPoint.y);
			double evaluatedValueTwo = noiseOne.Evaluate(
				_testEvaluationPoint.x,
				_testEvaluationPoint.y);
			Assert.AreEqual(evaluatedValueOne, evaluatedValueTwo);
		}

		private void TestRandomSampleWithinRange(Noise2D noise, System.Random random)
		{
			double actual = EvaluateRandom2DSample(noise, random);
			Assert.GreaterOrEqual(actual, _valueRange.x);
			Assert.LessOrEqual(actual, _valueRange.y);
		}

		private double EvaluateRandom2DSample(Noise2D noise, System.Random random)
		{
			double x = random.NextDouble() * random.Next(_sampleRange.x);
			double y = random.NextDouble() * random.Next(_sampleRange.y);
			return noise.Evaluate(x, y);
		}
	}
}