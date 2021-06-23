using Zenject;
using NUnit.Framework;
using System;
using UnityEngine;

namespace SBaier.Master.Test
{
    [TestFixture]
    public class NoiseAmplifierTest : NoiseTest
    {
		private const int _testSeed = 49242;
		private readonly Vector3[] _points = new Vector3[]
		{
			Vector3.zero,
			new Vector3(-3.2f, 5.9f, 0.4f),
			new Vector3(1f, -5.3f, 3.6f),
			new Vector3(9.2f, -0.5f, 4.6f)
		};

		protected override NoiseType ExpectedNoiseType => NoiseType.Amplifier;
		private Noise3D _noise;
		private Noise3D _baseNoiseA;
		private Noise3D _baseNoiseB;

		protected override void GivenANew3DNoise()
		{
			Container.Bind<Noise3D>().To<NoiseAmplifier>().FromMethod(CreateNoise).AsTransient();
			_noise = Container.Resolve<Noise3D>();
		}

		[Test(Description = "Evaluate returns multiplication of Noise A and Noise B")]
		public void Evaluate_ReturnsAMultiplicationOfNoiseAValuesAndNoiseBValues()
		{
			GivenANew3DNoise();
			float[] result = WhenCallenWithPoints(_points);
			ThenValuesAreAMultiplicationOfNoiseAValuesAndNoiseBValues(result);
		}

		private float[] WhenCallenWithPoints(Vector3[] points)
		{
			return _noise.Evaluate3D(points);
		}

		private void ThenValuesAreAMultiplicationOfNoiseAValuesAndNoiseBValues(float[] result)
		{
			for(int i = 0; i < result.Length; i++)
			{
				float baseValueA = _baseNoiseA.Evaluate3D(_points[i]);
				float baseValueB = _baseNoiseB.Evaluate3D(_points[i]);
				float expected = baseValueA * baseValueB;
				Assert.AreEqual(expected, result[i]);
			}
		}

		private NoiseAmplifier CreateNoise()
		{
			_baseNoiseA = new PerlinNoise(new Seed(_testSeed));
			_baseNoiseB = new PerlinNoise(new Seed(_testSeed));
			return new NoiseAmplifier(_baseNoiseA, _baseNoiseB);
		}
	}
}