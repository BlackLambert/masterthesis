using Zenject;
using NUnit.Framework;
using UnityEngine;
using System;

namespace SBaier.Master.Test
{
    [TestFixture]
    public class PlanetDimensionsTest : ZenjectUnitTestFixture
    {
        private Vector2[] _invalidPlanetDimensions =
        {
            new Vector2(-0.1f, 3f),
            new Vector2(0f, 3f),
            new Vector2(0f, 0f),
            new Vector2(4f, 0f),
            new Vector2(4f, -3f),
            new Vector2(-3f, -2f),
        };

        private Vector2[] _validPlanetDimensions =
        {
            new Vector2(0.1f, 3f),
            new Vector2(3f, 4f),
            new Vector2(20f, 30f),
            new Vector2(5f, 5.5f),
            new Vector2(2.4f, 3.2f),
        };

        private PlanetDimensions _planetDimension;

        [Test]
        public void Constructor_ThrowsExceptionOnIvalidPlanetDimensions()
        {
            for (int i = 0; i < _invalidPlanetDimensions.Length; i++)
            {
                TestDelegate test = () => GivenANewPlanetData(_invalidPlanetDimensions[i]);
                ThenThrowsArgumentException(test);
                Teardown();
                Setup();
            }
        }

        [Test]
        public void VariableAreaThickness_ReturnsExpectedValue()
        {
            for (int i = 0; i < _validPlanetDimensions.Length; i++)
            {
                _planetDimension = GivenANewPlanetData(_validPlanetDimensions[i]);
                ThenVariableAreaThicknessIsAsExpected();
                Teardown();
                Setup();
            }
        }

        private void ThenVariableAreaThicknessIsAsExpected()
        {
            float expected = _planetDimension.AtmosphereThickness - _planetDimension.KernalThickness;
            Assert.AreEqual(expected, _planetDimension.VariableAreaThickness);
        }

        private PlanetDimensions GivenANewPlanetData(Vector2 planetDimesions)
        {
            return new PlanetDimensions(planetDimesions.x, planetDimesions.y);
        }

        private void ThenThrowsArgumentException(TestDelegate test)
        {
            Assert.Throws<ArgumentOutOfRangeException>(test);
        }
    }
}