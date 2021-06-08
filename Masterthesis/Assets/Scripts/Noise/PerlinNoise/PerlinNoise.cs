using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

namespace SBaier.Master
{
	/// <summary>
	/// Deviation of Ken Perlins algorithm introduced in 2002.
	/// </summary>
    public class PerlinNoise : Noise3D, Seedbased
	{
		private const int _permutationCount = 256;
		private const int _doublePermutationCount = _permutationCount * 2;

		private int[] _permutation = new int[_permutationCount];
		private int[] _p = new int[_doublePermutationCount];

		public Seed Seed { get; }

		public NoiseType NoiseType => NoiseType.Perlin;

		public PerlinNoise(Seed seed)
		{
			Seed = seed;
			InitPermutation(seed);
		}

		public double Evaluate(double x, double y)
		{
			return Evaluate(x, y, 0);
		}

		public double Evaluate(double x, double y, double z)
		{
			int X = (int)Math.Floor(x) & 255, // FIND UNIT CUBE THAT
				Y = (int)Math.Floor(y) & 255, // CONTAINS POINT.
				Z = (int)Math.Floor(z) & 255;

			x -= Math.Floor(x); // FIND RELATIVE X,Y,Z
			y -= Math.Floor(y); // OF POINT IN CUBE.
			z -= Math.Floor(z);

			double u = Fade(x), // COMPUTE FADE CURVES
				   v = Fade(y), // FOR EACH OF X,Y,Z.
				   w = Fade(z);

			int A = _p[X] + Y, AA = _p[A] + Z, AB = _p[A + 1] + Z, // HASH COORDINATES OF
				B = _p[X + 1] + Y, BA = _p[B] + Z, BB = _p[B + 1] + Z; // THE 8 CUBE CORNERS,

			double value = Lerp(w, Lerp(v, Lerp(u, Grad(_p[AA], x, y, z),  // AND ADD
										   Grad(_p[BA], x - 1, y, z)), // BLENDED
								   Lerp(u, Grad(_p[AB], x, y - 1, z),  // RESULTS
										   Grad(_p[BB], x - 1, y - 1, z))),// FROM  8
						   Lerp(v, Lerp(u, Grad(_p[AA + 1], x, y, z - 1),  // CORNERS
										   Grad(_p[BA + 1], x - 1, y, z - 1)), // OF CUBE
								   Lerp(u, Grad(_p[AB + 1], x, y - 1, z - 1),
										   Grad(_p[BB + 1], x - 1, y - 1, z - 1))));
			return (value + 1) / 2;
		}


		private void InitPermutation(Seed seed)
		{
			int[] permutation = new int[_permutationCount];
			for (int i = 0; i < permutation.Length; i++)
				permutation[i] = i;

			permutation = permutation.OrderBy(x => seed.Random.Next()).ToArray();
			for (int i = 0; i < permutation.Length; i++)
				_permutation[i] = permutation[i];

			for (int i = 0; i < _permutationCount; i++)
				_p[_permutationCount + i] = _p[i] = permutation[i];
		}

		private double Fade(double t)
		{
			return t * t * t * (t * (t * 6 - 15) + 10);
		}

		private double Lerp(double t, double min, double max)
		{
			return min + t * (max - min);
		}

		private double Grad(int hash, double x, double y, double z)
		{
			int h = hash & 15; // CONVERT LO 4 BITS OF HASH CODE
			double u = h < 8 ? x : y, // INTO 12 GradIENT DIRECTIONS.
				   v = h < 4 ? y : h == 12 || h == 14 ? x : z;
			return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
		}
	}
}