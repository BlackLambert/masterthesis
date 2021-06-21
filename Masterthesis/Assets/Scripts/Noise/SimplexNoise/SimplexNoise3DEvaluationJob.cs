using System;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace SBaier.Master
{
    public struct SimplexNoise3DEvaluationJob : IJobParallelFor
    {
		[WriteOnly]
		public NativeArray<double> _result;
		[ReadOnly]
		public NativeArray<Vector3> _points;
		[ReadOnly]
		public IndexableNativeArray<short> _dP;
		[ReadOnly]
		public IndexableNativeArray<short> _dPMod;

		public SimplexNoise3DEvaluationJob(NativeArray<double> result,
			NativeArray<Vector3> points,
			IndexableNativeArray<short> dP,
			IndexableNativeArray<short> dPMod)
		{
			_result = result;
			_points = points;
			_dP = dP;
			_dPMod = dPMod;
		}

		public void Execute(int index)
		{
			Vector3 p = _points[index];
			_result[index] = Evaluate(p.x, p.y, p.z);
		}

		public double Evaluate(double x, double y, double z)
		{
			double G3 = SimplexNoise.G3;
			double F3 = SimplexNoise.F3;
			short[][] Grad3 = SimplexNoise.Grad3;

			// Skew the input space to determine which simplex cell we're in
			// Very nice and simple skew factor for 3D
			double s = (x + y + z) * F3;

			int i = SimplexNoise.Floor(x + s);
			int j = SimplexNoise.Floor(y + s);
			int k = SimplexNoise.Floor(z + s);

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

			int gi0 = _dPMod[ii + _dP[jj + _dP[kk]]];
			int gi1 = _dPMod[ii + i1 + _dP[jj + j1 + _dP[kk + k1]]];
			int gi2 = _dPMod[ii + i2 + _dP[jj + j2 + _dP[kk + k2]]];
			int gi3 = _dPMod[ii + 1 + _dP[jj + 1 + _dP[kk + 1]]];

			// Noise contributions from the four corners
			double n0, n1, n2, n3;

			// Calculate the contribution from the four corners
			double t0 = 0.6 - x0 * x0 - y0 * y0 - z0 * z0;
			if (t0 < 0)
				n0 = 0.0;
			else
			{
				t0 *= t0;
				n0 = t0 * t0 * SimplexNoise.Dot(Grad3[gi0], x0, y0, z0);
			}

			double t1 = 0.6 - x1 * x1 - y1 * y1 - z1 * z1;
			if (t1 < 0)
				n1 = 0.0;
			else
			{
				t1 *= t1;
				n1 = t1 * t1 * SimplexNoise.Dot(Grad3[gi1], x1, y1, z1);
			}

			double t2 = 0.6 - x2 * x2 - y2 * y2 - z2 * z2;
			if (t2 < 0)
				n2 = 0.0;
			else
			{
				t2 *= t2;
				n2 = t2 * t2 * SimplexNoise.Dot(Grad3[gi2], x2, y2, z2);
			}

			double t3 = 0.6 - x3 * x3 - y3 * y3 - z3 * z3;
			if (t3 < 0)
				n3 = 0.0;
			else
			{
				t3 *= t3;
				n3 = t3 * t3 * SimplexNoise.Dot(Grad3[gi3], x3, y3, z3);
			}

			// Add contributions from each corner to get the final noise value.
			// The result is scaled to stay just inside [0,1]
			return (32.0 * (n0 + n1 + n2 + n3) + 1) / 2;
		}
	}
}