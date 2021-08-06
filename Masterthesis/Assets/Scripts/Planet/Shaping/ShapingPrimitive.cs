using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace SBaier.Master
{
	public abstract class ShapingPrimitive
	{
		public Vector3 Position { get; }
		public float BlendArea { get; }
		public abstract float MaxAreaOfEffect { get; }

		public ShapingPrimitive(Vector3 position, float blendArea)
		{
			ValidateBlendArea(blendArea);
			Position = position;
			BlendArea = blendArea;
		}

		private void ValidateBlendArea(float blendArea)
		{
			if (blendArea < 0)
				throw new ArgumentException();
		}

		public float Evaluate(Vector3 point)
		{
			if (IsOutsideAreaOfEffect(point))
				return 0;
			if (IsInsideKernel(point))
				return 1;
			return GetBlendedValue(point);
		}

		public float[] Evaluate(IList<Vector3> points)
		{
			float[] result = new float[points.Count];
			List<Vector3> evalPoints = new List<Vector3>();
			InitEvalPoints(points, result, evalPoints);
			SetResult(points, result);
			return result;
		}

		private void SetResult(IList<Vector3> points, float[] result)
		{
			int evalIndex = 0;
			for (int i = 0; i < result.Length; i++)
			{
				if (result[i] == 0)
					continue;
				Vector3 point = points[i];
				if (IsInsideKernel(point))
					result[i] = 1;
				else
					result[i] = GetBlendedValue(point);
				evalIndex++;
			}
		}

		private void InitEvalPoints(IList<Vector3> points, float[] evalResult, List<Vector3> evalPoints)
		{
			float count = points.Count;
			for (int i = 0; i < count; i++)
			{
				Vector3 point = points[i];
				if (IsOutsideAreaOfEffect(point))
					evalResult[i] = 0;
				else
				{
					evalResult[i] = -1;
					evalPoints.Add(point);
				}
			}
		}

		protected abstract float GetBlendedValue(Vector3 point);
		protected abstract bool IsInsideKernel(Vector3 point);
		public abstract bool IsOutsideAreaOfEffect(Vector3 point);
		protected abstract NativeArray<float> Evaluate(NativeArray<Vector3> points);
	}
}