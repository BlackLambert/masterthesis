using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace SBaier.Master
{
	[Unity.Burst.BurstCompile]
	public struct PerlinNoise3DEvaluationJob : IJobParallelFor, NoiseEvaluationJob
    {
		private const int _innerloopBatchCount = 8;
		public NativeArray<float> _result;
		[ReadOnly]
		public NativeArray<Vector3> _points;
		[ReadOnly]
		public NativeArray<short> _dP;
		[ReadOnly]
		public NativeArray<short> _dPMod12;
		[ReadOnly]
		public NativeArray<Vector3Int> _grad3;

		public NativeArray<float> Result => _result;

		public void Execute(int index)
		{
			Vector3 point = _points[index];
			_result[index] = Evaluate(point);
		}

		public float Evaluate(Vector3 point)
		{
			float x = point.x;
			float y = point.y;
			float z = point.z;

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

			return (float)(nxyz + 1) / 2;
		}
		private static double Fade(double t)
		{
			return t * t * t * (t * (t * 6 - 15) + 10);
		}

		private static double Lerp(double min, double max, double t)
		{
			return min + t * (max - min);
		}

		private static short Floor(double value)
		{
			short xi = (short)value;
			return (short)(value < xi ? xi - 1 : xi);
		}

		private static double Dot(Vector3 gradient, double x, double y, double z)
		{
			return gradient[0] * x + gradient[1] * y + gradient[2] * z;
		}

		public JobHandle Schedule()
		{
			return this.Schedule(_result.Length, _innerloopBatchCount);
		}

		public JobHandle Schedule(JobHandle dependency)
		{
			return this.Schedule(_result.Length, _innerloopBatchCount, dependency);
		}

		public void Dispose()
		{
			_dP.Dispose();
			_dPMod12.Dispose();
			_grad3.Dispose();
		}
	}
}