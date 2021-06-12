using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace SBaier.Master
{
	/// <summary>
	/// Quelle: https://weber.itn.liu.se/~stegu/simplexnoise/SimplexNoise.java
	/// </summary>
	public class SimplexNoise : Noise3D, Seedbased
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

		private readonly double F2 = 0.5 * (Math.Sqrt(3.0) - 1.0);
		private readonly double G2 = (3.0 - Math.Sqrt(3.0)) / 6.0;
		private const double F3 = 1.0 / 3.0;
		private const double G3 = 1.0 / 6.0;

		private readonly short[] _dP = new short[_doublePermutationCount];
		private readonly short[] _dPMod12 = new short[_doublePermutationCount];

		public NoiseType NoiseType => NoiseType.Simplex;

		public Seed Seed { get; }

		public SimplexNoise(Seed seed)
		{
			Seed = seed;
			InitPermutation(seed);
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
				_dPMod12[_permutationCount + i] = _dPMod12[i] = (short)(permutation[i] % 12);
			}
		}

		public double Evaluate(double x, double y)
		{
			// Skew the input space to determine which simplex cell we're in
			// Hairy factor for 2D
			double s = (x + y) * F2; 
			int i = Floor(x + s);
			int j = Floor(y + s);
			double t = (i + j) * G2;

			// Unskew the cell origin back to (x,y) space
			double X0 = i - t; 
			double Y0 = j - t;

			// The x,y distances from the cell origin
			double x0 = x - X0; 
			double y0 = y - Y0;

			// For the 2D case, the simplex shape is an equilateral triangle.
			// Determine which simplex we are in.
			// Offsets for second (middle) corner of simplex in (i,j) coords
			int i1, j1;
			// lower triangle, XY order: (0,0)->(1,0)->(1,1)
			if (x0 > y0) 
			{ 
				i1 = 1; 
				j1 = 0; 
			}
			// upper triangle, YX order: (0,0)->(0,1)->(1,1)
			// A step of (1,0) in (i,j) means a step of (1-c,-c) in (x,y), and
			// a step of (0,1) in (i,j) means a step of (-c,1-c) in (x,y), where
			// c = (3-sqrt(3))/6
			else 
			{ 
				i1 = 0; 
				j1 = 1; 
			}

			// Offsets for middle corner in (x,y) unskewed coords
			double x1 = x0 - i1 + G2; 
			double y1 = y0 - j1 + G2;

			// Offsets for last corner in (x,y) unskewed coords
			double x2 = x0 - 1.0 + 2.0 * G2; 
			double y2 = y0 - 1.0 + 2.0 * G2;

			// Work out the hashed gradient indices of the three simplex corners
			int ii = i & 255;
			int jj = j & 255;
			int gi0 = _dPMod12[ii + _dP[jj]];
			int gi1 = _dPMod12[ii + i1 + _dP[jj + j1]];
			int gi2 = _dPMod12[ii + 1 + _dP[jj + 1]];

			// Noise contributions from the three corners
			double n0, n1, n2;

			// Calculate the contribution from the three corners
			double t0 = 0.5 - x0 * x0 - y0 * y0;
			if (t0 < 0) 
				n0 = 0.0;
			else
			{
				t0 *= t0;
				// (x,y) of grad3 used for 2D gradient
				n0 = t0 * t0 * Dot(_grad3[gi0], x0, y0);  
			}

			double t1 = 0.5 - x1 * x1 - y1 * y1;
			if (t1 < 0)
				n1 = 0.0;
			else
			{
				t1 *= t1;
				n1 = t1 * t1 * Dot(_grad3[gi1], x1, y1);
			}

			double t2 = 0.5 - x2 * x2 - y2 * y2;
			if (t2 < 0) 
				n2 = 0.0;
			else
			{
				t2 *= t2;
				n2 = t2 * t2 * Dot(_grad3[gi2], x2, y2);
			}

			// Add contributions from each corner to get the final noise value.
			// The result is scaled to return values in the interval [0,1].
			return (70.0 * (n0 + n1 + n2) + 1) / 2;
		}

		public double Evaluate(double x, double y, double z)
		{
			// Skew the input space to determine which simplex cell we're in
			// Very nice and simple skew factor for 3D
			double s = (x + y + z) * F3; 

			int i = Floor(x + s);
			int j = Floor(y + s);
			int k = Floor(z + s);
			
			double t = (i + j + k) * G3;

			// Unskew the cell origin back to (x,y,z) space
			double X0 = i - t; 
			double Y0 = j - t;
			double Z0 = k - t;

			// The x,y,z distances from the cell origin
			double x0 = x - X0; 
			double y0 = y - Y0;
			double z0 = z - Z0;

			// For the 3D case, the simplex shape is a slightly irregular tetrahedron.
			// Determine which simplex we are in.
			// Offsets for second corner of simplex in (i,j,k) coords
			int i1, j1, k1;
			// Offsets for third corner of simplex in (i,j,k) coords
			int i2, j2, k2; 
			if (x0 >= y0)
			{
				if (y0 >= z0)
				{
					// X Y Z order
					i1 = 1; 
					j1 = 0; 
					k1 = 0; 
					i2 = 1; 
					j2 = 1; 
					k2 = 0; 
				} 
				else if (x0 >= z0) 
				{
					// X Z Y order
					i1 = 1; 
					j1 = 0; 
					k1 = 0; 
					i2 = 1; 
					j2 = 0; 
					k2 = 1; 
				} 
				else 
				{
					// Z X Y order
					i1 = 0; 
					j1 = 0;
					k1 = 1; 
					i2 = 1; 
					j2 = 0; 
					k2 = 1; 
				} 
			}
			else
			{ 
				// x0<y0
				if (y0 < z0) 
				{
					// Z Y X order
					i1 = 0; 
					j1 = 0; 
					k1 = 1; 
					i2 = 0; 
					j2 = 1; 
					k2 = 1; 
				} 
				else if (x0 < z0) 
				{
					// Y Z X order
					i1 = 0; 
					j1 = 1; 
					k1 = 0; 
					i2 = 0; 
					j2 = 1; 
					k2 = 1; 
				} 
				else 
				{
					// Y X Z order
					i1 = 0; 
					j1 = 1; 
					k1 = 0; 
					i2 = 1; 
					j2 = 1; 
					k2 = 0; 
				} 
			}
			// A step of (1,0,0) in (i,j,k) means a step of (1-c,-c,-c) in (x,y,z),
			// a step of (0,1,0) in (i,j,k) means a step of (-c,1-c,-c) in (x,y,z), and
			// a step of (0,0,1) in (i,j,k) means a step of (-c,-c,1-c) in (x,y,z), where
			// c = 1/6.

			// Offsets for second corner in (x,y,z) coords
			double x1 = x0 - i1 + G3; 
			double y1 = y0 - j1 + G3;
			double z1 = z0 - k1 + G3;

			// Offsets for third corner in (x,y,z) coords
			double x2 = x0 - i2 + 2.0 * G3; 
			double y2 = y0 - j2 + 2.0 * G3;
			double z2 = z0 - k2 + 2.0 * G3;

			// Offsets for last corner in (x,y,z) coords
			double x3 = x0 - 1.0 + 3.0 * G3; 
			double y3 = y0 - 1.0 + 3.0 * G3;
			double z3 = z0 - 1.0 + 3.0 * G3;

			// Work out the hashed gradient indices of the four simplex corners
			int ii = i & 255;
			int jj = j & 255;
			int kk = k & 255;
			int gi0 = _dPMod12[ii + _dP[jj + _dP[kk]]];
			int gi1 = _dPMod12[ii + i1 + _dP[jj + j1 + _dP[kk + k1]]];
			int gi2 = _dPMod12[ii + i2 + _dP[jj + j2 + _dP[kk + k2]]];
			int gi3 = _dPMod12[ii + 1 + _dP[jj + 1 + _dP[kk + 1]]];

			// Noise contributions from the four corners
			double n0, n1, n2, n3;

			// Calculate the contribution from the four corners
			double t0 = 0.6 - x0 * x0 - y0 * y0 - z0 * z0;
			if (t0 < 0) 
				n0 = 0.0;
			else
			{
				t0 *= t0;
				n0 = t0 * t0 * Dot(_grad3[gi0], x0, y0, z0);
			}

			double t1 = 0.6 - x1 * x1 - y1 * y1 - z1 * z1;
			if (t1 < 0) 
				n1 = 0.0;
			else
			{
				t1 *= t1;
				n1 = t1 * t1 * Dot(_grad3[gi1], x1, y1, z1);
			}

			double t2 = 0.6 - x2 * x2 - y2 * y2 - z2 * z2;
			if (t2 < 0)
				n2 = 0.0;
			else
			{
				t2 *= t2;
				n2 = t2 * t2 * Dot(_grad3[gi2], x2, y2, z2);
			}

			double t3 = 0.6 - x3 * x3 - y3 * y3 - z3 * z3;
			if (t3 < 0) 
				n3 = 0.0;
			else
			{
				t3 *= t3;
				n3 = t3 * t3 * Dot(_grad3[gi3], x3, y3, z3);
			}

			// Add contributions from each corner to get the final noise value.
			// The result is scaled to stay just inside [0,1]
			return (32.0 * (n0 + n1 + n2 + n3) + 1) / 2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int Floor(double value)
		{
			int xi = (int)value;
			return value < xi ? xi - 1 : xi;
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