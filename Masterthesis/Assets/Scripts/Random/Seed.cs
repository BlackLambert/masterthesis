using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public class Seed
    {
		public Seed(int seed)
		{
			Random = new System.Random(seed);
			SeedNumber = seed;
		}

		public System.Random Random { get; }
		public double SeedNumber { get; }
	}
}