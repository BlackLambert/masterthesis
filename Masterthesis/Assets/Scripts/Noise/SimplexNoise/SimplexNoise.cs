using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace SBaier.Master
{
	/// <summary>
	/// Quelle: https://weber.itn.liu.se/~stegu/simplexnoise/SimplexNoise.java
	/// </summary>
	public class SimplexNoise : NoiseBase, Noise3D, Seedbased
	{
		private const short _permutationCount = 256;
		private const short _doublePermutationCount = _permutationCount * 2;

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

		private IList<short> _dP = new short[_doublePermutationCount];
		private IList<short> _dPMod = new short[_doublePermutationCount];

		public NoiseType NoiseType => NoiseType.Simplex;

		public Seed Seed { get; }

		public SimplexNoise(Seed seed)
		{
			Seed = seed;
			InitPermutation(seed);
		}

		public NativeArray<float> Evaluate3D(NativeArray<Vector3> points)
		{
			ApplyFrequencyFactor(points);
			NoiseEvaluationJob job = CreateEvaluate3DJob(points);
			return FinishJob(job);
		}

		public NativeArray<float> Evaluate2D(NativeArray<Vector2> points)
		{
			ApplyFrequencyFactor(points);
			NoiseEvaluationJob job = CreateEvaluate2DJob(points);
			return FinishJob(job);
		}

		public float Evaluate3D(Vector3 point)
		{
			NativeArray<Vector3> points = new NativeArray<Vector3>(1, Allocator.TempJob);
			points[0] = ApplyFrequencyFactor3D(point);
			NoiseEvaluationJob job = CreateEvaluate3DJob(points);
			float result = FinishJob(job)[0];
			points.Dispose();
			job.Result.Dispose();
			return result;
		}
		public float Evaluate2D(Vector2 point)
		{
			NativeArray<Vector2> points = new NativeArray<Vector2>(1, Allocator.TempJob);
			points[0] = ApplyFrequencyFactor2D(point);
			NoiseEvaluationJob job = CreateEvaluate2DJob(points);
			float result = FinishJob(job)[0];
			points.Dispose();
			job.Result.Dispose();
			return result;
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

		private void InitPermutation(Seed seed)
		{
			short[] permutation = new short[_permutationCount];
			for (short i = 0; i < permutation.Length; i++)
				permutation[i] = i;

			permutation = permutation.OrderBy(x => seed.Random.Next()).ToArray();

			for (int i = 0; i < _permutationCount; i++)
			{
				_dP[_permutationCount + i] = _dP[i] = permutation[i];
				_dPMod[_permutationCount + i] = _dPMod[i] = (short)(permutation[i] % Grad3.Length);
			}
		}

		public NoiseEvaluationJob CreateEvaluate3DJob(NativeArray<Vector3> points)
		{
			NativeArray<float> evaluatedPoints = new NativeArray<float>(points.Length, Allocator.TempJob);
			NativeArray<Vector3Int> gradients = CreateNativeGradients3D();
			NativeArray<short> dP = new NativeArray<short>(_dP.ToArray(), Allocator.TempJob);
			NativeArray<short> dPMod = new NativeArray<short>(_dPMod.ToArray(), Allocator.TempJob);

			SimplexNoise3DEvaluationJob job = new SimplexNoise3DEvaluationJob(evaluatedPoints, points, dP, dPMod, gradients, Weight);
			return job;
		}

		public NoiseEvaluationJob CreateEvaluate2DJob(NativeArray<Vector2> points)
		{
			NativeArray<float> evaluatedPoints = new NativeArray<float>(points.Length, Allocator.TempJob);
			NativeArray<short> dP = new NativeArray<short>(_dP.ToArray(), Allocator.TempJob);
			NativeArray<short> dPMod = new NativeArray<short>(_dPMod.ToArray(), Allocator.TempJob);
			NativeArray<Vector2Int> gradients = CreateNativeGradients2D();

			SimplexNoise2DEvaluationJob job = new SimplexNoise2DEvaluationJob(evaluatedPoints, points, dP, dPMod, gradients, Weight);
			return job;
		}

		private NativeArray<float> FinishJob(NoiseEvaluationJob job)
		{
			job.Schedule().Complete();
			NativeArray<float> result = job.Result;
			job.Dispose();
			return result;
		}
	}
}