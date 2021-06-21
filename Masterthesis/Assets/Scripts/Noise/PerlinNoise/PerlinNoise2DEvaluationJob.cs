using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace SBaier.Master
{
    public struct PerlinNoise2DEvaluationJob : IJobParallelFor
    {
		public NativeArray<double> _result;
		[ReadOnly]
		public NativeArray<Vector2> _points;
		[ReadOnly]
		public IndexableNativeArray<short> _dP;
		[ReadOnly]
		public IndexableNativeArray<short> _dPMod12;

		public void Execute(int index)
		{
			Vector3 point = _points[index];
			_result[index] = Evaluate(point.x, point.y);
		}

		public double Evaluate(double x, double y)
		{
			short[][] Grad3 = PerlinNoise.Grad3;

			short flooredX = PerlinNoise.Floor(x);
			short flooredY = PerlinNoise.Floor(y);

			short X = (short)(flooredX & 255);
			short Y = (short)(flooredY & 255);

			short gi00 = _dPMod12[X + _dP[Y]];
			short gi01 = _dPMod12[X + _dP[Y + 1]];
			short gi10 = _dPMod12[X + 1 + _dP[Y]];
			short gi11 = _dPMod12[X + 1 + _dP[Y + 1]];

			x -= flooredX;
			y -= flooredY;

			double n00 = PerlinNoise.Dot(Grad3[gi00], x, y);
			double n10 = PerlinNoise.Dot(Grad3[gi10], x - 1, y);
			double n01 = PerlinNoise.Dot(Grad3[gi01], x, y - 1);
			double n11 = PerlinNoise.Dot(Grad3[gi11], x - 1, y - 1);

			double u = PerlinNoise.Fade(x);
			double v = PerlinNoise.Fade(y);

			double nx0 = PerlinNoise.Lerp(n00, n10, u);
			double nx1 = PerlinNoise.Lerp(n01, n11, u);

			double nxy = PerlinNoise.Lerp(nx0, nx1, v);
			return (nxy + 1) / 2;
		}
	}
}