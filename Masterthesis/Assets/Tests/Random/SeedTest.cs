using Zenject;
using NUnit.Framework;
using System;

namespace SBaier.Master.Test
{
    [TestFixture]
    public class SeedTest : ZenjectUnitTestFixture
    {
		private const int _testSeed = 143254;
		private const int _testSamples = 10;
		private System.Random _testRandom;

		[Test]
        public void RandomSeedOfSeedEqualsTestRandomSeed()
        {
            GivenANewSeed();
            GivenASameSeededRandom();
            ThenSeedNextIntsEqualsRandomNextInts();
        }

        [Test]
        public void SeedValueEqualsConstructorInput()
		{
            GivenANewSeed();
            ThenTheSeedValueEqualsTheConstructorInput();
        }
        [Test]
        public void Reset_SameNumbersAreGeneratedAfterReset()
        {
            GivenANewSeed();
            Seed seed = Container.Resolve<Seed>();
            int[] expected = WhenNextIsCalledXTimes(seed, _testSamples);
            WhenResetIsCalledOn(seed);
            int[] actual = WhenNextIsCalledXTimes(seed, _testSamples);
            Assert.AreEqual(expected, actual);
        }

		private void GivenANewSeed()
		{
            Seed seed = new Seed(_testSeed);
            Container.Bind<Seed>().To<Seed>().FromInstance(seed).AsSingle();
        }

        private void GivenASameSeededRandom()
        {
            _testRandom = new System.Random(_testSeed);
        }

        private int[] WhenNextIsCalledXTimes(Seed seed, int testSamples)
        {
            int[] result = new int[testSamples];
            for (int i = 0; i < testSamples; i++)
                result[i] = seed.Random.Next();
            return result;
        }

        private void WhenResetIsCalledOn(Seed seed)
        {
            seed.Reset();
        }

        private void ThenSeedNextIntsEqualsRandomNextInts()
        {
            Seed seed = Container.Resolve<Seed>();
            int testCycles = 100;
            for(int i = 0; i < testCycles; i++)
			{
                int expected = _testRandom.Next();
                int actual = seed.Random.Next();
                Assert.AreEqual(expected, actual);
            }
        }

        private void ThenTheSeedValueEqualsTheConstructorInput()
        {
            Seed seed = Container.Resolve<Seed>();
            Assert.AreEqual(_testSeed, seed.SeedNumber);
        }
    }
}