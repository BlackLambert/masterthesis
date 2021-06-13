using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace SBaier.Master
{
	/// <summary>
	/// Deviation of Ken Perlins algorithm introduced in 2002.
	/// Optimized by the implementation of Stefan Gustavson and Peter Eastman 2005/2012.
	/// Quelle: https://weber.itn.liu.se/~stegu/simplexnoise
	/// </summary>
	public class PerlinNoise : Noise3D, Seedbased
	{
		private readonly static short[][] _grad3 = new short[][] 
		{
			new short[] {1,1,0},
			new short[] {-1,1,0},
			new short[] {1,-1,0},
			new short[] {-1,-1,0},
			new short[] {1,0,1},
			new short[] {-1,0,1},
			new short[] {1,0,-1},
			new short[] {-1,0,-1},
			new short[] {0,1,1},
			new short[] {0,-1,1},
			new short[] {0,1,-1},
			new short[] {0,-1,-1}
		};

		private const short _permutationCount = 256;
		private const short _doublePermutationCount = _permutationCount * 2;

		private readonly short[] _dP = new short[_doublePermutationCount];
		private readonly short[] _dPMod12 = new short[_doublePermutationCount];

		public Seed Seed { get; }

		public NoiseType NoiseType => NoiseType.Perlin;

		public PerlinNoise(Seed seed)
		{
			Seed = seed;
			InitPermutation(seed);
		}

		public double Evaluate(double x, double y)
		{
			short flooredX = Floor(x);
			short flooredY = Floor(y);

			short X = (short)(flooredX & 255);
			short Y = (short)(flooredY & 255);

			short gi00 = _dPMod12[X + _dP[Y]];
			short gi01 = _dPMod12[X + _dP[Y + 1]];
			short gi10 = _dPMod12[X + 1 + _dP[Y]];
			short gi11 = _dPMod12[X + 1 + _dP[Y + 1]];

			//Debug.Log($"[{x}|{y}] [{gi00}|{gi01}|{gi10}|{gi11}|]");

			x -= flooredX;
			y -= flooredY;

			double n00 = Dot(_grad3[gi00], x, y);
			double n10 = Dot(_grad3[gi10], x - 1, y);
			double n01 = Dot(_grad3[gi01], x, y - 1);
			double n11 = Dot(_grad3[gi11], x - 1, y - 1);

			double u = Fade(x);
			double v = Fade(y);

			double nx0 = Lerp(n00, n10, u);
			double nx1 = Lerp(n01, n11, u);

			double nxy = Lerp(nx0, nx1, v);
			return (nxy + 1) / 2;
		}

		public double Evaluate(double x, double y, double z)
		{
			short flooredX = Floor(x);
			short flooredY = Floor(y);
			short flooredZ = Floor(z);

			// Find unit grid cell containing point
			// Wrap the integer cells at 255 (smaller integer period can be introduced here)
			short X = (short)(flooredX & 255);
			short Y = (short)(flooredY & 255);
			short Z = (short)(flooredZ & 255);

			// Get relative xyz coordinates of point within that cell
			x -= flooredX;
			y -= flooredY;
			z -= flooredZ;

			// Calculate a set of eight hashed gradient indices
			short gi000 = _dPMod12[X + _dP[Y + _dP[Z]]];
			short gi001 = _dPMod12[X + _dP[Y + _dP[Z + 1]]];
			short gi010 = _dPMod12[X + _dP[Y + 1 + _dP[Z]]];
			short gi011 = _dPMod12[X + _dP[Y + 1 + _dP[Z + 1]]];
			short gi100 = _dPMod12[X + 1 + _dP[Y + _dP[Z]]];
			short gi101 = _dPMod12[X + 1 + _dP[Y + _dP[Z + 1]]];
			short gi110 = _dPMod12[X + 1 + _dP[Y + 1 + _dP[Z]]];
			short gi111 = _dPMod12[X + 1 + _dP[Y + 1 + _dP[Z + 1]]];

			// Calculate noise contributions from each of the eight corners
			double n000 = Dot(_grad3[gi000], x, y, z);
			double n100 = Dot(_grad3[gi100], x - 1, y, z);
			double n010 = Dot(_grad3[gi010], x, y - 1, z);
			double n110 = Dot(_grad3[gi110], x - 1, y - 1, z);
			double n001 = Dot(_grad3[gi001], x, y, z - 1);
			double n101 = Dot(_grad3[gi101], x - 1, y, z - 1);
			double n011 = Dot(_grad3[gi011], x, y - 1, z - 1);
			double n111 = Dot(_grad3[gi111], x - 1, y - 1, z - 1);

			// Compute the fade curve value for each of x, y, z
			double u = Fade(x);
			double v = Fade(y);
			double w = Fade(z);

			// Interpolate along x the contributions from each of the corners
			double nx00 = Lerp(n000, n100, u);
			double nx01 = Lerp(n001, n101, u);
			double nx10 = Lerp(n010, n110, u);
			double nx11 = Lerp(n011, n111, u);

			// Interpolate the four results along y
			double nxy0 = Lerp(nx00, nx10, v);
			double nxy1 = Lerp(nx01, nx11, v);

			// Interpolate the two last results along z
			double nxyz = Lerp(nxy0, nxy1, w);

			return (nxyz + 1) / 2;
		}


		private void InitPermutation(Seed seed)
		{
			short[] permutation = new short[_permutationCount];
			for (short i = 0; i < permutation.Length; i++)
				permutation[i] = i;

			permutation = permutation.OrderBy(x => seed.Random.Next()).ToArray();
			for (int i = 0; i < _permutationCount; i++)
			{
				_dP[_permutationCount + i] = _dP[i] = permutation[i];
				_dPMod12[_permutationCount + i] = _dPMod12[i] = (short) (permutation[i] % 12);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static double Fade(double t)
		{
			return t * t * t * (t * (t * 6 - 15) + 10);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static double Lerp(double min, double max, double t)
		{
			return min + t * (max - min);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static short Floor(double value)
		{
			short xi = (short)value;
			return (short)(value < xi ? xi - 1 : xi);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static double Dot(short[] gradient, double x, double y, double z)
		{
			return gradient[0] * x + gradient[1] * y + gradient[2] * z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static double Dot(short[] gradient, double x, double y)
		{
			return gradient[0] * x + gradient[1] * y;
		}
	}
}