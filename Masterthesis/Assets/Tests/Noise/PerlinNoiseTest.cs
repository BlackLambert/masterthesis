using Zenject;
using NUnit.Framework;
using System;
using UnityEngine;

namespace SBaier.Master.Test
{
    [TestFixture]
    public class PerlinNoiseTest : NoiseTest
	{
		private const int _testSeed = 49242;

		protected override NoiseType ExpectedNoiseType => NoiseType.Perlin;

		protected override void GivenANew3DNoise()
		{
			Container.Bind<Seed>().To<Seed>().FromMethod(CreateSeed).AsTransient();
			Container.Bind<Noise3D>().To<PerlinNoise>().AsTransient();
		}

		private Seed CreateSeed()
		{
			return new Seed(_testSeed);
		}
	}
}