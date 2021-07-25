using Zenject;
using NUnit.Framework;
using System;

namespace SBaier.Master.Test
{
    [TestFixture]
    public class PlanetAxisDataTest : ZenjectUnitTestFixture
    {
        private float[] _invalidAgles =
        {
            -1f, -22f, 90.1f, -0.1f, 118f
        };

        private float[] _validAgles =
        {
            0, 23, 90, 60, 10
        };

        private float[] _validSecondsPerRevolution =
        {
            3f, 90f, 60f, 32f, 0.1f
        };

        private float[] _invalidSecondsPerRevolution =
        {
            0, -2f, -10f, -100f, -0.5f
        };

        [Test]
        public void Constructor_InvalidAngleThrowsExcpetion()
        {
			for (int i = 0; i < _invalidAgles.Length; i++)
			{
                TestDelegate test = () => GivenAPlanetAxisData(_invalidAgles[i], _validSecondsPerRevolution[i]);
                ThenThrowsArgumentOutOfRangeException(test);
                Teardown();
                Setup();
			}
        }

        [Test]
        public void Constructor_InvalidSecondsPerRevolutionThrowsExcpetion()
        {
			for (int i = 0; i < _invalidSecondsPerRevolution.Length; i++)
			{
                TestDelegate test = () => GivenAPlanetAxisData(_validAgles[i], _invalidSecondsPerRevolution[i]);
                ThenThrowsArgumentOutOfRangeException(test);
                Teardown();
                Setup();
			}
        }

        [Test]
        public void RotationSpeed_HasExpectedValue()
        {
			for (int i = 0; i < _validAgles.Length; i++)
			{
                PlanetAxisData data = GivenAPlanetAxisData(_validAgles[i], _validSecondsPerRevolution[i]);
                ThenRotationSpeedIsAsExpected(data);
                Teardown();
                Setup();
			}
        }

		private PlanetAxisData GivenAPlanetAxisData(float angle, float secondsPerRevolution)
		{
            return new PlanetAxisData(angle, secondsPerRevolution);
		}

		private void ThenThrowsArgumentOutOfRangeException(TestDelegate test)
		{
            Assert.Throws<ArgumentOutOfRangeException>(test);
        }

        private void ThenRotationSpeedIsAsExpected(PlanetAxisData data)
        {
            float expected = 360 / data.SecondsPerRevolution;
            Assert.AreEqual(expected, data.RotationPerSecond);
        }
    }
}