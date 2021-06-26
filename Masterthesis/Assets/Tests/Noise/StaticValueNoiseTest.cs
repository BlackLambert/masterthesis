using Zenject;
using NUnit.Framework;
using System;
using UnityEngine;
using Unity.Collections;

namespace SBaier.Master.Test
{
    [TestFixture]
    public class StaticValueNoiseTest : NoiseTest
    {
		private const float _validValue = 0.2f;
		private readonly float[] _invalidAmounts = new float[]
		{
			-0.2f,
			-0.0001f,
			-3.24f,
			1.001f,
			2.42f,
			5.6f
		};

		private readonly Vector3[] _points = new Vector3[]
		{
			Vector3.zero,
			new Vector3(-3.2f, 5.9f, 0.4f),
			new Vector3(1f, -5.3f, 3.6f),
			new Vector3(9.2f, -0.5f, 4.6f)
		};

		private Noise3D _noise;


		protected override NoiseType ExpectedNoiseType => NoiseType.Static;

		protected override void GivenANew3DNoise()
		{
			Container.Bind<Noise3D>().To<StaticValueNoise>().AsTransient().WithArguments(_validValue);
			_noise = Container.Resolve<Noise3D>();
		}

		protected void GivenAStaticValueNoiseWithInvalidAmount(float value)
		{
			new StaticValueNoise(value);
		}

		[Test (Description = "Trying to create a StaticNoise with invalid amount causes ArgumentOutOfRangeException")]
		public void Constructor_InvalidAmountThrowsException()
		{
			foreach (float arg in _invalidAmounts)
			{
				TestDelegate test = () => GivenAStaticValueNoiseWithInvalidAmount(arg);
				ThenAnArgumentOutOfRangeExceptionIsThrown(test);
				Teardown();
				Setup();
			}
		}

		[Test (Description = "All input values evaluate to the provided valid value")]
		public void Evaluate_ReturnsValueProvidedByTheConstructor()
		{
			GivenANew3DNoise();
			float[] result = WhenEvaluateIsCalledWith(_points);
			ThenAllValuesEqualValidValue(result);
		}

		private float[] WhenEvaluateIsCalledWith(Vector3[] points)
		{
			NativeArray<Vector3> pointsNative = new NativeArray<Vector3>(points, Allocator.TempJob);
			NativeArray<float> resultNative = _noise.Evaluate3D(pointsNative);
			float[] result = resultNative.ToArray();
			pointsNative.Dispose();
			resultNative.Dispose();
			return result;
		}

		private void ThenAnArgumentOutOfRangeExceptionIsThrown(TestDelegate test)
		{
			Assert.Throws<ArgumentOutOfRangeException>(test);
		}

		private void ThenAllValuesEqualValidValue(float[] result)
		{
			foreach (float value in result)
				Assert.AreEqual(_validValue, value);
		}
	}
}