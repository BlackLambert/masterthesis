using Zenject;
using NUnit.Framework;
using UnityEngine;
using System;
using Unity.Collections;

namespace SBaier.Master.Test
{
    [TestFixture]
    public abstract class NoiseTest : ZenjectUnitTestFixture
    {
		private const int _testSamplesCount = 10;
		private const int _randomSeed = 1234;
		private const double _epsilon = 0.0001;
		private readonly float[] _frequencyFactors = new float[] {0.1f, 0.0001f, 1.2f, 2.6f, 1.0f, 1.0f, 1.0f};
		private readonly float[] _invalidFrequencyFactors = new float[] { -0.1f, 0.0f, -1.2f};
		private readonly float[] _weights = new float[] { 0.7f, 1.0f, 2.5f, 0.2f, 1.0f, 0.0f, 131.0f };
		private readonly float[] _invalidWeights = new float[] { -0.0001f, -0.5f, -1.8f};
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
			for (int i = 0; i < _frequencyFactors.Length; i++)
			{ 
				GivenANew3DNoise();
				_noiseOne = Container.Resolve<Noise3D>();
				WhenBaseValuesAreSetTo(_noiseOne, _frequencyFactors[i], _weights[i]);
				ThenAllEvaluatedValuesAreInExpectedRange();
				Teardown();
				Setup();
			}
		}

		[Test]
		public void EvaluatedDataOfTwoNoisesWithSameSeedAreEqual()
		{
			for (int i = 0; i < _frequencyFactors.Length; i++)
			{
				GivenANew3DNoise();
				_noiseOne = Container.Resolve<Noise3D>();
				_noiseTwo = Container.Resolve<Noise3D>();
				WhenBaseValuesAreSetTo(_noiseOne, _frequencyFactors[i], _weights[i]);
				WhenBaseValuesAreSetTo(_noiseTwo, _frequencyFactors[i], _weights[i]);
				ThenEvaluatedDataOfTwoNoisesWithSameSeedAreEqual();
				Teardown();
				Setup();
			}
		}

		[Test]
		public void TwoEvaluatedValuesWithTheSameInputAreEqual()
		{
			for (int i = 0; i < _frequencyFactors.Length; i++)
			{
				GivenANew3DNoise();
				_noiseOne = Container.Resolve<Noise3D>();
				WhenBaseValuesAreSetTo(_noiseOne, _frequencyFactors[i], _weights[i]);
				ThenTwoEvaluatedValuesWithTheSameInputAreEqual();
				Teardown();
				Setup();
			}
		}

		[Test]
		public void NoiseTypeReturnsExpectedValue()
		{
			GivenANew3DNoise();
			_noiseOne = Container.Resolve<Noise3D>();
			ThenNoiseTypeReturnsExpectedValue();
		}

		[Test (Description = "Evaluate called with Vector3 Array returns same values as single vector3 evaluation")]
		public void Evaluate_WithVector3Array_EvaluatesLikeSingleVector3()
		{
			for (int i = 0; i < _frequencyFactors.Length; i++)
			{
				GivenANew3DNoise();
				_noiseOne = Container.Resolve<Noise3D>();
				WhenBaseValuesAreSetTo(_noiseOne, _frequencyFactors[i], _weights[i]);
				ThenEvaluateWithVector3ArrayReturnsSameValuesAsSingleEvaluation();
				Teardown();
				Setup();
			}
		}

		[Test(Description = "Evaluate called with Vector2 Array returns same values as single vector2 evaluation")]
		public void Evaluate_WithVector2Array_EvaluatesLikeSingleVector2()
		{
			for (int i = 0; i < _frequencyFactors.Length; i++)
			{
				GivenANew3DNoise();
				_noiseOne = Container.Resolve<Noise3D>();
				WhenBaseValuesAreSetTo(_noiseOne, _frequencyFactors[i], _weights[i]);
				ThenEvaluateWithVector2ArrayReturnsSameValuesAsSingleEvaluation();
				Teardown();
				Setup();
			}
		}

		[Test(Description = "The FrequencyFactor property returns value put into the constructor")]
		public void FrequencyFactor_ReturnsExpectedValue()
		{
			for(int i = 0; i < _frequencyFactors.Length; i++)
			{
				GivenANew3DNoise();
				_noiseOne = Container.Resolve<Noise3D>();
				WhenFrequencyFactorIsSetTo(_noiseOne, _frequencyFactors[i]);
				ThenFrequencyFactorReturns(_frequencyFactors[i]);
				Teardown();
				Setup();
			}
		}

		[Test(Description = "The StartWeight property returns value put into the constructor")]
		public void StartWeight_ReturnsExpectedValue()
		{
			for (int i = 0; i < _weights.Length; i++)
			{
				GivenANew3DNoise();
				_noiseOne = Container.Resolve<Noise3D>();
				WhenWeightIsSetTo(_noiseOne, _weights[i]);
				ThenWeightReturns(_weights[i]);
				Teardown();
				Setup();
			}
		}

		[Test(Description = "The constructor throws an ArgumentOutOfRangeException if the Start Weight is out of range.")]
		public void ThrowsExceptionOnStartWeightOutOfRange()
		{
			for (int i = 0; i < _invalidWeights.Length; i++)
			{
				GivenANew3DNoise();
				_noiseOne = Container.Resolve<Noise3D>();
				TestDelegate test = () => WhenWeightIsSetTo(_noiseOne, _invalidWeights[i]);
				ThenThrowsArgumentOutOfRangeException(test);
				Teardown();
				Setup();
			}
		}

		[Test(Description = "The constructor throws an ArgumentOutOfRangeException if the Start Frequency is smaller than 1.")]
		public void ThrowsExceptionOnStartFrequencyOutOfRange()
		{
			for (int i = 0; i < _invalidFrequencyFactors.Length; i++)
			{
				GivenANew3DNoise();
				_noiseOne = Container.Resolve<Noise3D>();
				TestDelegate test = () => WhenFrequencyFactorIsSetTo(_noiseOne, _invalidFrequencyFactors[i]);
				ThenThrowsArgumentOutOfRangeException(test);
				Teardown();
				Setup();
			}
		}

		protected abstract void GivenANew3DNoise();


		private float[] WhenNoiseEvaluate2DIsCalledWithTestPoints(Noise2D noise)
		{
			NativeArray<Vector2> pointsNative = new NativeArray<Vector2>(_testEvaluationPoints2D, Allocator.TempJob);
			NativeArray<float> resultNative = noise.Evaluate2D(pointsNative);
			float[] result = resultNative.ToArray();
			pointsNative.Dispose();
			resultNative.Dispose();
			return result;
		}

		protected void WhenWeightIsSetTo(Noise3D noise, float value)
		{
			NoiseBase noiseBase = (NoiseBase)noise;
			noiseBase.Weight = value;
		}

		protected void WhenFrequencyFactorIsSetTo(Noise3D noise, float value)
		{
			NoiseBase noiseBase = (NoiseBase)noise;
			noiseBase.FrequencyFactor = value;
		}

		protected void WhenBaseValuesAreSetTo(Noise3D noise, float frequencyFactor, float weight)
		{
			NoiseBase noiseBase = (NoiseBase)noise;
			noiseBase.FrequencyFactor = frequencyFactor;
			noiseBase.Weight = weight;
		}

		private void ThenAllEvaluatedValuesAreInExpectedRange()
		{
			System.Random random = new System.Random(_randomSeed);
			
			for (int i = 0; i < _testSamplesCount; i++)
				TestRandomSampleWithinRange(_noiseOne, random);
		}

		private void ThenEvaluatedDataOfTwoNoisesWithSameSeedAreEqual()
		{
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
			double evaluatedValueOne = _noiseOne.Evaluate3D(
				_testEvaluationPoint);
			double evaluatedValueTwo = _noiseOne.Evaluate3D(
				_testEvaluationPoint);
			Assert.AreEqual(evaluatedValueOne, evaluatedValueTwo);
		}

		private void ThenNoiseTypeReturnsExpectedValue()
		{
			Assert.AreEqual(ExpectedNoiseType, _noiseOne.NoiseType);
		}

		private void ThenEvaluateWithVector3ArrayReturnsSameValuesAsSingleEvaluation()
		{
			_noiseOne = Container.Resolve<Noise3D>();
			float[] result = WhenNoiseEvaluate3DIsCalledWithTestPoints(_noiseOne);
			for (int i = 0; i < _testEvaluationPoints.Length; i++)
			{
				Vector3 v = _testEvaluationPoints[i];
				float expected = _noiseOne.Evaluate3D(v);
				Assert.AreEqual(expected, result[i], _epsilon);
			}
		}

		private void ThenFrequencyFactorReturns(float expectedFrequencyFactor)
		{
			NoiseBase noise = (NoiseBase)_noiseOne;
			Assert.AreEqual(expectedFrequencyFactor, noise.FrequencyFactor);
		}

		private void ThenWeightReturns(float weight)
		{
			NoiseBase noise = (NoiseBase)_noiseOne;
			Assert.AreEqual(weight, noise.Weight);
		}

		private float[] WhenNoiseEvaluate3DIsCalledWithTestPoints(Noise3D noise)
		{
			NativeArray<Vector3> pointsNative = new NativeArray<Vector3>(_testEvaluationPoints, Allocator.TempJob);
			NativeArray<float> resultNative = noise.Evaluate3D(pointsNative);
			float[] result = resultNative.ToArray();
			pointsNative.Dispose();
			resultNative.Dispose();
			return result;
		}

		private void ThenEvaluateWithVector2ArrayReturnsSameValuesAsSingleEvaluation()
		{
			_noiseOne = Container.Resolve<Noise3D>();
			float[] result = WhenNoiseEvaluate2DIsCalledWithTestPoints(_noiseOne);
			for (int i = 0; i < _testEvaluationPoints2D.Length; i++)
			{
				Vector2 v = _testEvaluationPoints2D[i];
				float expected = _noiseOne.Evaluate2D(v);
				Assert.AreEqual(expected, result[i], _epsilon);
			}
		}

		private void ThenThrowsArgumentOutOfRangeException(TestDelegate test)
		{
			Assert.Throws<ArgumentOutOfRangeException>(test);
		}

		private void TestRandomSampleWithinRange(Noise3D noise, System.Random random)
		{
			double actual = EvaluateRandom3DSample(noise, random);
			Assert.GreaterOrEqual(actual, _valueRange.x);
			Assert.LessOrEqual(actual, _valueRange.y);
		}

		private float EvaluateRandom3DSample(Noise3D noise, System.Random random)
		{
			float x = (float) random.NextDouble() * random.Next(_sampleRange.x);
			float y = (float) random.NextDouble() * random.Next(_sampleRange.y);
			float z = (float) random.NextDouble() * random.Next(_sampleRange.z);
			return noise.Evaluate3D(new Vector3(x, y, z));
		}
	}
}