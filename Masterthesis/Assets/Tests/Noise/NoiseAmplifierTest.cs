using Zenject;
using NUnit.Framework;
using System;
using UnityEngine;
using Unity.Collections;

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
		private Noise3D _baseNoise;
		private Noise3D _amplificationNoise;

		protected override void GivenANew3DNoise()
		{
			Container.Bind<Noise3D>().To<NoiseAmplifier>().FromMethod(() => CreateNoise(NoiseAmplifier.Mode.Linear)).AsTransient();
			_noise = Container.Resolve<Noise3D>();
		}

		protected void GivenADefaultSetup(NoiseAmplifier.Mode mode)
		{
			Container.Bind<Noise3D>().To<NoiseAmplifier>().FromMethod(() => CreateNoise(mode)).AsTransient();
			_noise = Container.Resolve<Noise3D>();
		}

		[Test(Description = "Evaluate returns multiplication of Noise A and Noise B")]
		public void Evaluate_ReturnsALinearAmplificationOfBaseNoise()
		{
			GivenADefaultSetup(NoiseAmplifier.Mode.Linear);
			float[] result = WhenCallenWithPoints(_points);
			ThenValuesAreAMultiplicationOfNoiseAValuesAndNoiseBValues(result);
		}

		[Test(Description = "Evaluate returns quadric amplification of Noise A and Noise B")]
		public void Evaluate_ReturnsAQuadricAmplificationOfBaseNoise()
		{
			GivenADefaultSetup(NoiseAmplifier.Mode.Quadric);
			float[] result = WhenCallenWithPoints(_points);
			ThenValuesAreAQuadricAmplificationOfBaseValue(result);
		}

		[Test(Description = "Evaluate returns half quadric amplification of Noise A and Noise B")]
		public void Evaluate_ReturnsAHalfQuadricAmplificationOfBaseNoise()
		{
			GivenADefaultSetup(NoiseAmplifier.Mode.HalfQuadric);
			float[] result = WhenCallenWithPoints(_points);
			ThenValuesAreAHalfQuadricAmplificationOfBaseValue(result);
		}

		private float[] WhenCallenWithPoints(Vector3[] points)
		{
			NativeArray<Vector3> pointsNative = new NativeArray<Vector3>(points, Allocator.TempJob);
			NativeArray<float> resultNative = _noise.Evaluate3D(pointsNative);
			float[] result = resultNative.ToArray();
			pointsNative.Dispose();
			resultNative.Dispose();
			return result;
		}

		private void ThenValuesAreAMultiplicationOfNoiseAValuesAndNoiseBValues(float[] actual)
		{
			for(int i = 0; i < actual.Length; i++)
			{
				float baseValue = _baseNoise.Evaluate3D(_points[i]);
				float amplificationValue = _amplificationNoise.Evaluate3D(_points[i]);
				float expected = baseValue * amplificationValue;
				Assert.AreEqual(expected, actual[i]);
			}
		}

		private void ThenValuesAreAQuadricAmplificationOfBaseValue(float[] actual)
		{
			for(int i = 0; i < actual.Length; i++)
			{
				float baseValueA = _baseNoise.Evaluate3D(_points[i]);
				float baseValueB = _amplificationNoise.Evaluate3D(_points[i]);
				float expected = baseValueA * baseValueB * baseValueB;
				Assert.AreEqual(expected, actual[i]);
			}
		}

		private void ThenValuesAreAHalfQuadricAmplificationOfBaseValue(float[] actual)
		{
			for (int i = 0; i < actual.Length; i++)
			{
				float baseValueA = _baseNoise.Evaluate3D(_points[i]);
				float baseValueB = _amplificationNoise.Evaluate3D(_points[i]);
				float expected = baseValueA * Mathf.Pow(baseValueB, 1.5f);
				Assert.AreEqual(expected, actual[i]);
			}
		}

		private NoiseAmplifier CreateNoise(NoiseAmplifier.Mode mode)
		{
			_baseNoise = new PerlinNoise(new Seed(_testSeed));
			_amplificationNoise = new PerlinNoise(new Seed(_testSeed));
			return new NoiseAmplifier(_baseNoise, _amplificationNoise, mode);
		}
	}
}