using System;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace SBaier.Master
{
    public struct SimplexNoise2DEvaluationJob : IJobParallelFor
    {
		[WriteOnly]
		public NativeArray<double> _result;
		[ReadOnly]
		public NativeArray<Vector2> _points;
		[ReadOnly]
		public IndexableNativeArray<short> _dP;
		[ReadOnly]
		public IndexableNativeArray<short> _dPMod;

		public SimplexNoise2DEvaluationJob(NativeArray<double> result,
			NativeArray<Vector2> points,
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
			Vector2 p = _points[index];
			_result[index] = Evaluate(p.x, p.y);
		}
		public double Evaluate(double x, double y)
		{
			double G2 = SimplexNoise.G2;
			double F2 = SimplexNoise.F2;
			short[][] Grad3 = SimplexNoise.Grad3;

			// Skew the input space to determine which simplex cell we're in
			// Hairy factor for 2D
			double s = (x + y) * F2;
			int i = SimplexNoise.Floor(x + s);
			int j = SimplexNoise.Floor(y + s);
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
			int gi0 = _dPMod[ii + _dP[jj]];
			int gi1 = _dPMod[ii + i1 + _dP[jj + j1]];
			int gi2 = _dPMod[ii + 1 + _dP[jj + 1]];

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
				n0 = t0 * t0 * SimplexNoise.Dot(Grad3[gi0], x0, y0);
			}

			double t1 = 0.5 - x1 * x1 - y1 * y1;
			if (t1 < 0)
				n1 = 0.0;
			else
			{
				t1 *= t1;
				n1 = t1 * t1 * SimplexNoise.Dot(Grad3[gi1], x1, y1);
			}

			double t2 = 0.5 - x2 * x2 - y2 * y2;
			if (t2 < 0)
				n2 = 0.0;
			else
			{
				t2 *= t2;
				n2 = t2 * t2 * SimplexNoise.Dot(Grad3[gi2], x2, y2);
			}

			// Add contributions from each corner to get the final noise value.
			// The result is scaled to return values in the interval [0,1].
			return (70.0 * (n0 + n1 + n2) + 1) / 2;
		}
	}
}