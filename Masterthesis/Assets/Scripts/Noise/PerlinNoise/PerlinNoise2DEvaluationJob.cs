using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace SBaier.Master
{
	[Unity.Burst.BurstCompile]
    public struct PerlinNoise2DEvaluationJob : IJobParallelFor, NoiseEvaluationJob
    {
		[WriteOnly]
		public NativeArray<float> _result;
		[ReadOnly]
		public NativeArray<Vector2> _points;
		[ReadOnly]
		public NativeArray<short> _dP;
		[ReadOnly]
		public NativeArray<short> _dPMod12;
		[ReadOnly]
		public NativeArray<Vector2Int> _grad2;
		private const int _innerloopBatchCount = 8;

		public NativeArray<float> Result => _result;

		public void Execute(int index)
		{
			Vector2 point = _points[index];
			_result[index] = Evaluate(point);
		}

		public float Evaluate(Vector2 point)
		{
			float x = point.x;
			float y = point.y;
			short flooredX = Floor(x);
			short flooredY = Floor(y);

			short X = (short)(flooredX & 255);
			short Y = (short)(flooredY & 255);

			short gi00 = _dPMod12[X + _dP[Y]];
			short gi01 = _dPMod12[X + _dP[Y + 1]];
			short gi10 = _dPMod12[X + 1 + _dP[Y]];
			short gi11 = _dPMod12[X + 1 + _dP[Y + 1]];

			x -= flooredX;
			y -= flooredY;

			double n00 = Dot(_grad2[gi00], x, y);
			double n10 = Dot(_grad2[gi10], x - 1, y);
			double n01 = Dot(_grad2[gi01], x, y - 1);
			double n11 = Dot(_grad2[gi11], x - 1, y - 1);

			double u = Fade(x);
			double v = Fade(y);

			double nx0 = Lerp(n00, n10, u);
			double nx1 = Lerp(n01, n11, u);

			double nxy = Lerp(nx0, nx1, v);
			return (float) (nxy + 1) / 2;
		}


		private static double Dot(Vector2 gradient, double x, double y)
		{
			return gradient[0] * x + gradient[1] * y;
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
			_grad2.Dispose();
		}
	}
}