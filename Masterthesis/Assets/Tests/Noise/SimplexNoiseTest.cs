using Zenject;
using NUnit.Framework;
using System;
using UnityEngine;

namespace SBaier.Master.Test
{
    [TestFixture]
    public class SimplexNoiseTest : NoiseTest
	{
		private const int _testSeed = 1234;

		protected override NoiseType ExpectedNoiseType => NoiseType.Simplex;

		protected override void GivenANew3DNoise()
		{
			Container.Bind<Seed>().To<Seed>().FromMethod(CreateSeed).AsTransient();
			Container.Bind(typeof(Noise3D), typeof(Noise2D)).To<SimplexNoise>().AsTransient();
		}

		private Seed CreateSeed()
		{
			return new Seed(_testSeed);
		}
	}
}