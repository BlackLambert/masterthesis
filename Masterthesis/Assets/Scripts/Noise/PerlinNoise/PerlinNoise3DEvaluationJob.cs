using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace SBaier.Master
{
    public struct PerlinNoise3DEvaluationJob : IJobParallelFor
    {
		public NativeArray<double> _result;
		[ReadOnly]
		public NativeArray<Vector3> _points;
		[ReadOnly]
		public IndexableNativeArray<short> _dP;
		[ReadOnly]
		public IndexableNativeArray<short> _dPMod12;

		public void Execute(int index)
		{
			Vector3 point = _points[index];
			_result[index] = Evaluate(point.x, point.y, point.z);
		}

		public double Evaluate(double x, double y, double z)
		{
			short[][] Grad3 = PerlinNoise.Grad3;

			short flooredX = PerlinNoise.Floor(x);
			short flooredY = PerlinNoise.Floor(y);
			short flooredZ = PerlinNoise.Floor(z);

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
			double n000 = PerlinNoise.Dot(Grad3[gi000], x, y, z);
			double n100 = PerlinNoise.Dot(Grad3[gi100], x - 1, y, z);
			double n010 = PerlinNoise.Dot(Grad3[gi010], x, y - 1, z);
			double n110 = PerlinNoise.Dot(Grad3[gi110], x - 1, y - 1, z);
			double n001 = PerlinNoise.Dot(Grad3[gi001], x, y, z - 1);
			double n101 = PerlinNoise.Dot(Grad3[gi101], x - 1, y, z - 1);
			double n011 = PerlinNoise.Dot(Grad3[gi011], x, y - 1, z - 1);
			double n111 = PerlinNoise.Dot(Grad3[gi111], x - 1, y - 1, z - 1);

			// Compute the fade curve value for each of x, y, z
			double u = PerlinNoise.Fade(x);
			double v = PerlinNoise.Fade(y);
			double w = PerlinNoise.Fade(z);

			// Interpolate along x the contributions from each of the corners
			double nx00 = PerlinNoise.Lerp(n000, n100, u);
			double nx01 = PerlinNoise.Lerp(n001, n101, u);
			double nx10 = PerlinNoise.Lerp(n010, n110, u);
			double nx11 = PerlinNoise.Lerp(n011, n111, u);

			// Interpolate the four results along y
			double nxy0 = PerlinNoise.Lerp(nx00, nx10, v);
			double nxy1 = PerlinNoise.Lerp(nx01, nx11, v);

			// Interpolate the two last results along z
			double nxyz = PerlinNoise.Lerp(nxy0, nxy1, w);

			return (nxyz + 1) / 2;
		}
	}
}