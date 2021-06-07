using Zenject;
using NUnit.Framework;
using System;

namespace SBaier.Master.Test
{
    [TestFixture]
    public class NoiseFactoryTest : ZenjectUnitTestFixture
    {
        [Test]
        public void CreatesPerlinNoiseOnPerlineNoiseSettings()
        {
            GivenANewNoiseFactory();
        }

		private void GivenANewNoiseFactory()
		{
			throw new NotImplementedException();
		}
	}
}