using Zenject;
using NUnit.Framework;
using UnityEngine;
using System;

namespace SBaier.Master.Test
{
    [TestFixture]
    public abstract class NoiseTest : ZenjectUnitTestFixture
    {
		private const int _testSamplesCount = 1000;
		private const int _randomSeed = 1234;
		private const double _epsilon = 0.0001;
		private readonly Vector2 _valueRange = new Vector2(0, 1);
		private readonly Vector3 _testEvaluationPoint = new Vector3(3.2f, 5.8f, -1.7f);
		private readonly Vector3[] _testEvaluationPoints = new Vector3[]
		{
			Vector3.zero,
			Vector3.up,
			Vector3.zero,
			new Vector3 (-1.3f, 0.1f, -2.8f),
			new Vector3 (7.4f, -3.2f, 7.1f)
		};
		private readonly Vector2[] _testEvaluationPoints2D = new Vector2[]
		{
			Vector2.zero,
			Vector2.up,
			Vector2.zero,
			new Vector2 (-1.3f, 0.1f),
			new Vector2 (7.4f, -3.2f)
		};
		private readonly Vector3Int _sampleRange = new Vector3Int(100, 100, 100);

		private Noise3D _noiseOne;
		private Noise3D _noiseTwo;

		protected abstract NoiseType ExpectedNoiseType { get; }

		[Test]
		public void AllEvaluatedValuesAreInExpectedRange()
		{
			GivenANew3DNoise();
			ThenAllEvaluatedValuesAreInExpectedRange();
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

		[Test]
		public void NoiseTypeReturnsExpectedValue()
		{
			GivenANew3DNoise();
			ThenNoiseTypeReturnsExpectedValue();
		}

		[Test (Description = "Evaluate called with Vector3 Array returns same values as single vector3 evaluation")]
		public void Evaluate_WithVector3Array_EvaluatesLikeSingleVector3()
		{
			GivenANew3DNoise();
			ThenEvaluateWithVector3ArrayReturnsSameValuesAsSingleEvaluation();
		}

		[Test(Description = "Evaluate called with Vector2 Array returns same values as single vector2 evaluation")]
		public void Evaluate_WithVector2Array_EvaluatesLikeSingleVector2()
		{
			GivenANew3DNoise();
			ThenEvaluateWithVector2ArrayReturnsSameValuesAsSingleEvaluation();
		}

		protected abstract void GivenANew3DNoise();

		private void ThenAllEvaluatedValuesAreInExpectedRange()
		{
			_noiseOne = Container.Resolve<Noise3D>();
			System.Random random = new System.Random(_randomSeed);
			
			for (int i = 0; i < _testSamplesCount; i++)
				TestRandomSampleWithinRange(_noiseOne, random);
		}

		private void ThenEvaluatedDataOfTwoNoisesWithSameSeedAreEqual()
		{
			_noiseOne = Container.Resolve<Noise3D>();
			_noiseTwo = Container.Resolve<Noise3D>();
			Assert.AreNotSame(_noiseOne, _noiseTwo);
			System.Random randomOne = new System.Random(_randomSeed);
			System.Random randomTwo = new System.Random(_randomSeed);

			for (int i = 0; i < _testSamplesCount; i++)
			{
				double valueOne = EvaluateRandom3DSample(_noiseOne, randomOne);
				double valueTwo = EvaluateRandom3DSample(_noiseTwo, randomTwo);

				Assert.AreEqual(valueOne, valueTwo);
			}
		}

		private void ThenTwoEvaluatedValuesWithTheSameInputAreEqual()
		{
			_noiseOne = Container.Resolve<Noise3D>();
			double evaluatedValueOne = _noiseOne.Evaluate(
				_testEvaluationPoint.x,
				_testEvaluationPoint.y,
				_testEvaluationPoint.z);
			double evaluatedValueTwo = _noiseOne.Evaluate(
				_testEvaluationPoint.x,
				_testEvaluationPoint.y,
				_testEvaluationPoint.z);
			Assert.AreEqual(evaluatedValueOne, evaluatedValueTwo);
		}

		private void ThenNoiseTypeReturnsExpectedValue()
		{
			_noiseOne = Container.Resolve<Noise3D>();
			Assert.AreEqual(ExpectedNoiseType, _noiseOne.NoiseType);
		}

		private void ThenEvaluateWithVector3ArrayReturnsSameValuesAsSingleEvaluation()
		{
			_noiseOne = Container.Resolve<Noise3D>();
			double[] result = _noiseOne.Evaluate(_testEvaluationPoints);
			for(int i = 0; i < _testEvaluationPoints.Length; i++)
			{
				Vector3 v = _testEvaluationPoints[i];
				double expected = _noiseOne.Evaluate(v.x, v.y, v.z);
				Assert.AreEqual(expected, result[i], _epsilon);
			}
		}

		private void ThenEvaluateWithVector2ArrayReturnsSameValuesAsSingleEvaluation()
		{
			_noiseOne = Container.Resolve<Noise3D>();
			double[] result = _noiseOne.Evaluate(_testEvaluationPoints2D);
			for (int i = 0; i < _testEvaluationPoints2D.Length; i++)
			{
				Vector2 v = _testEvaluationPoints2D[i];
				double expected = _noiseOne.Evaluate(v.x, v.y);
				Assert.AreEqual(expected, result[i], _epsilon);
			}
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