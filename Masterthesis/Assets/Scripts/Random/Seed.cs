
using System;

namespace SBaier.Master
{
    public class Seed
    {
		public Seed(int seed)
		{
			Random = new System.Random(seed);
			SeedNumber = seed;
		}

		public System.Random Random { get; private set; }
		public int SeedNumber { get; }

		public void Reset()
		{
			Random = new Random(SeedNumber);
		}
	}
}