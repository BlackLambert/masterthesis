using Zenject;
using NUnit.Framework;
using UnityEngine;
using System;

namespace SBaier.Master.Test
{
    [TestFixture]
    public class TemperatureSpecturmTest : ZenjectUnitTestFixture
    {
        private Vector2[] _invalidTemperatureSpecturm =
        {
            new Vector2(-32f, -42f),
            new Vector2(0f, -1f),
            new Vector2(23f, 5f),
            new Vector2(72.7f, 72f),
        };

        private Vector2[] _validTemperatureSpecturm =
        {
            new Vector2(-32f, 20f),
            new Vector2(-145f, -66f),
            new Vector2(-345f, 412f),
            new Vector2(-40f, 50f),
            new Vector2(-133f, 895f),
        };

        [Test]
        public void Constructor_ThrowsExpectionOnInvalidValueRange()
        {
			for (int i = 0; i < _invalidTemperatureSpecturm.Length; i++)
			{
                TestDelegate test = () => GivenTemperatureSpectrum(_invalidTemperatureSpecturm[i]);
                ThenThrowsArgumentOutOfRangeException(test);
			}
        }

        [Test]
        public void Delta_ReturnsExpectedValue()
        {
            for (int i = 0; i < _validTemperatureSpecturm.Length; i++)
            {
                TemperatureSpectrum spectrum = GivenTemperatureSpectrum(_validTemperatureSpecturm[i]);
                ThenDeltaIsAsExpected(spectrum, _validTemperatureSpecturm[i]);
            }
        }

		private TemperatureSpectrum GivenTemperatureSpectrum(Vector2 spectrum)
		{
            return new TemperatureSpectrum(spectrum.x, spectrum.y);
        }

        private void ThenThrowsArgumentOutOfRangeException(TestDelegate test)
        {
            Assert.Throws<ArgumentOutOfRangeException>(test);
        }

        private void ThenDeltaIsAsExpected(TemperatureSpectrum spectrum, Vector2 range)
        {
            float expected = range.y - range.x;
            Assert.AreEqual(expected, spectrum.Delta);
        }
    }
}