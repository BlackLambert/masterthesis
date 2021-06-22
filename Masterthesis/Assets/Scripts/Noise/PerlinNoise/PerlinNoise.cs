using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using System;
using System.Collections.Generic;

namespace SBaier.Master
{
	/// <summary>
	/// Deviation of Ken Perlins algorithm introduced in 2002.
	/// Optimized by the implementation of Stefan Gustavson and Peter Eastman 2005/2012.
	/// Quelle: https://weber.itn.liu.se/~stegu/simplexnoise
	/// </summary>
	public class PerlinNoise : Noise3D, Seedbased
	{
		private readonly static Vector3Int[] Grad3 = new Vector3Int[]
		{
			new Vector3Int (1,1,0),
			new Vector3Int(-1,1,0),
			new Vector3Int(1,-1,0),
			new Vector3Int(-1,-1,0),
			new Vector3Int(1,0,1),
			new Vector3Int(-1,0,1),
			new Vector3Int(1,0,-1),
			new Vector3Int(-1,0,-1),
			new Vector3Int(0,1,1),
			new Vector3Int(0,-1,1),
			new Vector3Int(0,1,-1),
			new Vector3Int(0,-1,-1)
		};

		private const short _permutationCount = 256;
		private const short _doublePermutationCount = _permutationCount * 2;
		private const int _innerloopBatchCount = 64;

		private short[] _dP;
		private short[] _dPMod12;

		public Seed Seed { get; }

		public NoiseType NoiseType => NoiseType.Perlin;

		public PerlinNoise(Seed seed)
		{
			Seed = seed;
			InitPermutation(seed);
		}

		private void InitPermutation(Seed seed)
		{
			short[] permutation = new short[_permutationCount];
			for (short i = 0; i < permutation.Length; i++)
				permutation[i] = i;

			_dP = new short[_doublePermutationCount];
			_dPMod12 = new short[_doublePermutationCount];

			permutation = permutation.OrderBy(x => seed.Random.Next()).ToArray();
			for (int i = 0; i < _permutationCount; i++)
			{
				_dP[_permutationCount + i] = _dP[i] = permutation[i];
				_dPMod12[_permutationCount + i] = _dPMod12[i] = (short)(permutation[i] % 12);
			}
		}

		public float[] Evaluate3D(Vector3[] points)
		{
			NativeArray<float> evaluatedPoints = new NativeArray<float>(points.Length, Allocator.TempJob);
			NativeArray<Vector3> p = new NativeArray<Vector3>(points, Allocator.TempJob);
			NativeArray<short> dP = new NativeArray<short>(_dP, Allocator.TempJob);
			NativeArray<short> dPMod12 = new NativeArray<short>(_dPMod12, Allocator.TempJob);
			NativeArray<Vector3Int> grad3 = CreateNativeGradients3D();

			PerlinNoise3DEvaluationJob job = new PerlinNoise3DEvaluationJob()
			{
				_dP = dP,
				_dPMod12 = dPMod12,
				_result = evaluatedPoints,
				_points = p,
				_grad3 = grad3
			};

			job.Schedule(points.Length, _innerloopBatchCount).Complete();
			float[] result = evaluatedPoints.ToArray();

			evaluatedPoints.Dispose();
			p.Dispose();
			dP.Dispose();
			dPMod12.Dispose();
			grad3.Dispose();

			return result;
		}

		public float[] Evaluate2D(Vector2[] points)
		{
			NativeArray<float> evaluatedPoints = new NativeArray<float>(points.Length, Allocator.TempJob);
			NativeArray<Vector2> p = new NativeArray<Vector2>(points, Allocator.TempJob);
			NativeArray<short> dP = new NativeArray<short>(_dP, Allocator.TempJob);
			NativeArray<short> dPMod12 = new NativeArray<short>(_dPMod12, Allocator.TempJob);
			NativeArray<Vector2Int> grad2 = CreateNativeGradients2D();

			PerlinNoise2DEvaluationJob job = new PerlinNoise2DEvaluationJob()
			{
				_dP = dP,
				_dPMod12 = dPMod12,
				_result = evaluatedPoints,
				_points = p,
				_grad2 = grad2
			};

			job.Schedule(points.Length, _innerloopBatchCount).Complete();
			float[] result = evaluatedPoints.ToArray();

			evaluatedPoints.Dispose();
			p.Dispose();
			dP.Dispose();
			dPMod12.Dispose();
			grad2.Dispose();

			return result;
		}

		public float Evaluate2D(Vector2 point)
		{
			Vector2[] points = new Vector2[] { point };
			return Evaluate2D(points)[0];
		}

		public float Evaluate3D(Vector3 point)
		{
			Vector3[] points = new Vector3[] { point };
			return Evaluate3D(points)[0];
		}

		private NativeArray<Vector3Int> CreateNativeGradients3D()
		{
			return new NativeArray<Vector3Int>(Grad3, Allocator.Persistent);
		}

		private NativeArray<Vector2Int> CreateNativeGradients2D()
		{
			NativeArray<Vector2Int> result = new NativeArray<Vector2Int>(Grad3.Length, Allocator.Persistent);
			for (int i = 0; i < result.Length; i++)
			{
				Vector3Int g = Grad3[i];
				result[i] = new Vector2Int(g[0], g[1]);
			}
			return result;
		}
	}
}