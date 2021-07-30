using Zenject;
using NUnit.Framework;
using UnityEngine;
using System;

namespace SBaier.Master.Test
{
    [TestFixture]
    public class RandomPointsOnSphereGeneratorTest : ZenjectUnitTestFixture
    {
		private const int _seedNumber = 1234;
		private const float _epsilon = 0.0001f;
		private readonly int[] _amounts =
        {
            1, 5, 45, 22, 9
        };
        private readonly float[] _radius =
        {
            1f, 0.1f, 2.5f, 10f, 4.2f
        };

        private readonly int[] _invalidAmounts =
        {
            -1, 0, -5, -22, -12
        };
        private readonly float[] _invalidRadius =
        {
            -1f, -0.1f, -2.5f, 0f, -4.2f
        };

        private Vector3[] _points;
		private RandomPointsOnSphereGenerator _generator;

		[Test]
        public void Generate_WithAmout_RetrunsExpectetAmoutOfPoints()
        {
			for (int i = 0; i < _amounts.Length; i++)
			{
                GivenADefaultSetup();
                WhenGenerateIsCalledWith(_amounts[i], _radius[i]);
                ThenAmoutOfGeneratedPointsIsAsExpected(_amounts[i]);
                Teardown();
                Setup();
            }
        }

		[Test]
        public void Generate_WithAmout_ThrowsExceptionOnInvalidAmoutOfPoints()
        {
			for (int i = 0; i < _invalidAmounts.Length; i++)
			{
                GivenADefaultSetup();
                TestDelegate test = () => WhenGenerateIsCalledWith(_invalidAmounts[i], _radius[i]);
                ThenThrowsArgumentOutOfRangeException(test);
                Teardown();
                Setup();
            }
        }

        [Test]
        public void Generate_WithAmoutAndRadius_GeneratedPointsAreOnSphere()
        {
            for (int i = 0; i < _amounts.Length; i++)
            {
                GivenADefaultSetup();
                WhenGenerateIsCalledWith(_amounts[i], _radius[i]);
                ThenGeneratedPointsAreOnSphere(_radius[i]);
                Teardown();
                Setup();
            }
        }

        [Test]
        public void Generate_WithRadius_ThrowsExceptionOnIvalidRadius()
        {
            for (int i = 0; i < _invalidRadius.Length; i++)
            {
                GivenADefaultSetup();
                TestDelegate test = () => WhenGenerateIsCalledWith(_amounts[i], _invalidRadius[i]);
                ThenThrowsArgumentOutOfRangeException(test);
                Teardown();
                Setup();
            }
        }

        [Test]
        public void Reset_ResetsTheSeed()
		{
            GivenADefaultSetup();
            WhenResetIsCalledOnRandomPointsOnSphereGenerator();
            ThenSeedIsReset();
		}

		private void GivenADefaultSetup()
		{
            Container.Bind<Seed>().AsSingle().WithArguments(_seedNumber);
            Container.Bind<RandomPointsOnSphereGenerator>().AsTransient();
            _generator = Container.Resolve<RandomPointsOnSphereGenerator>();
        }

        private void WhenGenerateIsCalledWith(int amount, float radius)
        {
            _points = _generator.Generate(amount, radius);
        }

        private void WhenResetIsCalledOnRandomPointsOnSphereGenerator()
        {
            _generator.Reset();
        }

        private void ThenAmoutOfGeneratedPointsIsAsExpected(int amount)
        {
            Assert.AreEqual(amount, _points.Length);
        }

        private void ThenThrowsArgumentOutOfRangeException(TestDelegate test)
        {
            Assert.Throws<ArgumentOutOfRangeException>(test);
        }

        private void ThenGeneratedPointsAreOnSphere(float radius)
        {
            foreach (Vector3 point in _points)
                Assert.AreEqual(radius, point.magnitude, _epsilon);
        }

        private void ThenSeedIsReset()
        {
            Seed seed = Container.Resolve<Seed>();
            Seed newSeed = new Seed(_seedNumber);
            Assert.AreEqual(newSeed.Random.Next(), seed.Random.Next());
        }
    }
}