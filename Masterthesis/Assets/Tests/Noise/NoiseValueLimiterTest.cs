using Zenject;
using NUnit.Framework;
using UnityEngine;
using System;

namespace SBaier.Master.Test
{
	[TestFixture]
	public class NoiseValueLimiterTest : NoiseTest
	{
		private const int _baseSeed = 1234;
		private const float _epsilon = 0.001f;

		private readonly Vector2[] _invalidLimits = new Vector2[]
		{
			new Vector2(-0.1f, 1),
			new Vector2(0.3f, 0.2f),
			new Vector2(-0.7f, -0.5f),
			new Vector2(0, 1.01f),
			new Vector2(1f, 1.3f)
		};

		private readonly Vector2[] _validLimits = new Vector2[]
		{
			new Vector2(0, 1),
			new Vector2(0, 0),
			new Vector2(1, 1),
			new Vector2(0.3f, 1),
			new Vector2(0, 0.6f),
			new Vector2(0.3f, 0.6f)
		};

		private readonly Vector3[] _testVertices = new Vector3[]
		 {
			Vector3.zero,
			new Vector3(1,4,-1),
			new Vector3(-2,5.5f, 3),
			new Vector3(-3.1f, -2.4f, 1),
			new Vector3(4.5f, -0.2f, -7.1f),
			new Vector3(0.1f, 0.75f, 0.5f),
			new Vector3(25.7f, -104.7f, 34)
		 };

		protected override NoiseType ExpectedNoiseType => NoiseType.NoiseValueLimiter;
		private double _evaluatedValue;
		private NoiseValueLimiter _limiter;
		private Noise3D _baseNoise;

		public override void Setup()
		{
			base.Setup();
			_evaluatedValue = -1;
		}

		[Test]
		public void Evaluate_TestValuesAreAsExpected()
		{
			foreach (Vector2 limits in _validLimits)
			{
				GivenADefaultSetup(limits);
				foreach(Vector3 testVertex in _testVertices)
				{
					WhenEvaluateCalledWith(testVertex);
					double baseEvaluatedValue = _baseNoise.Evaluate(testVertex.x, testVertex.y, testVertex.z);
					ThenEvaluatedValueIsAsExpected(baseEvaluatedValue, limits);
				}
				Teardown();
				Setup();
			}
		}

		[Test]
		public void ThrowsExpectionOnInvalidLimits()
		{
			foreach (Vector2 limits in _invalidLimits)
			{
				TestDelegate test = () => CreateLimiter(limits);
				ThenThrowsArgumentOutOfRangeException(test);
			}
		}

		protected override void GivenANew3DNoise()
		{
			Container.Bind<Noise3D>().To<NoiseValueLimiter>().FromMethod(() => CreateLimiter(_validLimits[0])).AsTransient();
		}

		private void GivenADefaultSetup(Vector2 limits)
		{
			Container.Bind<NoiseValueLimiter>().To<NoiseValueLimiter>().FromMethod(() => CreateLimiter(limits)).AsTransient();
			_limiter = Container.Resolve<NoiseValueLimiter>();
		}

		private NoiseValueLimiter CreateLimiter(Vector2 limits)
		{
			_baseNoise = new PerlinNoise(new Seed(_baseSeed));
			return new NoiseValueLimiter(limits, _baseNoise);
		}

		private void WhenEvaluateCalledWith(Vector3 testVertex)
		{
			_evaluatedValue = _limiter.Evaluate(testVertex.x, testVertex.y, testVertex.z);
		}

		private void ThenEvaluatedValueIsAsExpected(double baseNoiseValue, Vector2 limits)
		{
			float value = (float) baseNoiseValue - limits.x;
			value = Mathf.Clamp(value, 0, limits.y - limits.x);
			Assert.AreEqual(value, (float) _evaluatedValue, _epsilon);
		}

		private void ThenThrowsArgumentOutOfRangeException(TestDelegate test)
		{
			Assert.Throws<ArgumentOutOfRangeException>(test);
		}
	}
}