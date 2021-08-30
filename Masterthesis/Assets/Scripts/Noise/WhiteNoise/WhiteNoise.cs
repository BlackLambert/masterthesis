using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace SBaier.Master
{
	public class WhiteNoise : NoiseBase, Noise3D
	{
		public NoiseType NoiseType => NoiseType.White;
		private Seed _seed;

		public WhiteNoise(Seed seed)
		{
			_seed = seed;
		}

		public float Evaluate2D(Vector2 point)
		{
			return (float) _seed.Random.NextDouble();
		}

		public NativeArray<float> Evaluate2D(NativeArray<Vector2> points)
		{
			NativeArray<float> result = new NativeArray<float>(points.Length, Allocator.TempJob);
			for (int i = 0; i < points.Length; i++)
				result[i] = Evaluate2D(points[i]);
			return result;
		}

		public float Evaluate3D(Vector3 point)
		{
			return (float)_seed.Random.NextDouble();
		}

		public NativeArray<float> Evaluate3D(NativeArray<Vector3> points)
		{
			NativeArray<float> result = new NativeArray<float>(points.Length, Allocator.TempJob);
			for (int i = 0; i < points.Length; i++)
				result[i] = Evaluate3D(points[i]);
			return result;
		}
	}
}