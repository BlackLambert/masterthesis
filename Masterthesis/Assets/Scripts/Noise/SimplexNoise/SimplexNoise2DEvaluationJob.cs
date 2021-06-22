using System;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace SBaier.Master
{
	[Unity.Burst.BurstCompile]
	public struct SimplexNoise2DEvaluationJob : IJobParallelFor
    {
		[WriteOnly]
		public NativeArray<float> _result;
		[ReadOnly]
		public NativeArray<Vector2> _points;
		[ReadOnly]
		public NativeArray<short> _dP;
		[ReadOnly]
		public NativeArray<short> _dPMod;
		[ReadOnly]
		public NativeArray<Vector2Int> _grad2;

		private static readonly double F2 = 0.5 * (Math.Sqrt(3.0) - 1.0);
		private static readonly double G2 = (3.0 - Math.Sqrt(3.0)) / 6.0;

		public SimplexNoise2DEvaluationJob(NativeArray<float> result,
			NativeArray<Vector2> points,
			NativeArray<short> dP,
			NativeArray<short> dPMod,
			NativeArray<Vector2Int> grad2)
		{
			_result = result;
			_points = points;
			_dP = dP;
			_dPMod = dPMod;
			_grad2 = grad2;
		}

		public void Execute(int index)
		{
			Vector2 p = _points[index];
			_result[index] = Evaluate(p.x, p.y);
		}

		public float Evaluate(double x, double y)
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
				n0 = t0 * t0 * Dot(_grad2[gi0], x0, y0);
			}

			double t1 = 0.5 - x1 * x1 - y1 * y1;
			if (t1 < 0)
				n1 = 0.0;
			else
			{
				t1 *= t1;
				n1 = t1 * t1 * Dot(_grad2[gi1], x1, y1);
			}

			double t2 = 0.5 - x2 * x2 - y2 * y2;
			if (t2 < 0)
				n2 = 0.0;
			else
			{
				t2 *= t2;
				n2 = t2 * t2 * Dot(_grad2[gi2], x2, y2);
			}

			// Add contributions from each corner to get the final noise value.
			// The result is scaled to return values in the interval [0,1].
			return (float)(70.0 * (n0 + n1 + n2) + 1) / 2;
		}

		private static double Dot(Vector2Int gradient, double x, double y)
		{
			return gradient[0] * x + gradient[1] * y;
		}
		private static int Floor(double value)
		{
			int xi = (int)value;
			return value < xi ? xi - 1 : xi;
		}
	}
}