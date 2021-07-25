using Zenject;
using NUnit.Framework;
using System;
using UnityEngine;

namespace SBaier.Master.Test
{
    [TestFixture]
    public class PlanetDataTest : ZenjectUnitTestFixture
    {
        private const int _seed = 1234;

        private Vector2[] _validPlanetDimensions =
        {
            new Vector2(0.1f, 3f),
            new Vector2(3f, 4f),
            new Vector2(20f, 30f),
            new Vector2(5f, 5.5f),
            new Vector2(2.4f, 3.2f),
        };
	}
}