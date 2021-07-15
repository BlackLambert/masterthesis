using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace SBaier.Master
{
	public class Vector3ParallelQuickSorter : Vector3SorterBase
	{
		protected const int _maxIterations = 100;

		protected override void DoSort(IList<Vector3> points, IList<int> indexPermutations, Vector2Int indexRange, int compareValueIndex)
		{
			NativeArray<Vector3> nativePoints = new NativeArray<Vector3>(points.ToArray(), Allocator.TempJob);
			NativeArray<int> nativeIndexPermutations = new NativeArray<int>(indexPermutations.ToArray(), Allocator.TempJob);
			Quicksort(nativePoints, nativeIndexPermutations, indexRange.x, indexRange.y, compareValueIndex);
			CopyTo<Vector3>(nativePoints, points);
			CopyTo<int>(nativeIndexPermutations, indexPermutations);
			nativePoints.Dispose();
			nativeIndexPermutations.Dispose();
		}

		private void Quicksort(NativeArray<Vector3> points, NativeArray<int> indexPermutations, int left, int right, int compareValueIndex)
		{
			NativeArray<int> pivots = CreatePivots(points.Length, left, right);
			NativeArray<int> formerPivots = new NativeArray<int>(points.Length, Allocator.TempJob);
			NativeArray<bool> done = CreateDoneArray(points.Length, left, right);

			int saftynet = 0;
			while (!IsDone(ref done) && saftynet < _maxIterations)
			{
				saftynet++;
				pivots.CopyTo(formerPivots);
				PartitionJob job = new PartitionJob
				(
					points,
					indexPermutations,
					compareValueIndex,
					pivots,
					formerPivots,
					done,
					left,
					right
				);

				job.Schedule(pivots.Length, 8).Complete();
			}

			pivots.Dispose();
			formerPivots.Dispose();
			done.Dispose();
		}

		private NativeArray<bool> CreateDoneArray(int length, int left, int right)
		{
			NativeArray<bool> result = new NativeArray<bool>(length, Allocator.TempJob);
			for (int i = 0; i < length; i++)
				result[i] = i < left || i > right;
			return result;
		}

		private NativeArray<int> CreatePivots(int length, int left, int right)
		{
			NativeArray<int> result = new NativeArray<int>(length, Allocator.TempJob);
			for (int i = 0; i < length; i++)
				result[i] = (i < left || i > right) ? i : -1;
			return result;
		}

		private bool IsDone(ref NativeArray<bool> done)
		{
			for (int i = 0; i < done.Length; i++)
			{
				if (done[i])
					continue;
				return false;
			}
			return true;
		}


		private void CopyTo<T>(NativeArray<T> nativePoints, IList<T> points) where T : struct
		{
			for (int i = 0; i < nativePoints.Length; i++)
				points[i] = nativePoints[i];
		}

		[BurstCompile]
		private struct PartitionJob : IJobParallelFor
		{
			public PartitionJob(
				NativeArray<Vector3> points,
				NativeArray<int> indexPermuations, 
				int compareValueIndex,
				NativeArray<int> pivots,
				NativeArray<int> formerPivots,
				NativeArray<bool> done,
				int leftBorder,
				int rightBorder)
			{
				_points = points;
				_indexPermuations = indexPermuations;
				_compareValueIndex = compareValueIndex;
				_pivots = pivots;
				_formerPivots = formerPivots;
				_done = done;
				_rightBorder = rightBorder;
				_leftBorder = leftBorder;
			}

			[NativeDisableParallelForRestriction]
			public NativeArray<Vector3> _points;
			[NativeDisableParallelForRestriction]
			public NativeArray<int> _indexPermuations;
			public int _compareValueIndex;
			[NativeDisableParallelForRestriction]
			[WriteOnly]
			public NativeArray<int> _pivots;
			[NativeDisableParallelForRestriction]
			[ReadOnly]
			public NativeArray<int> _formerPivots;
			[NativeDisableParallelForRestriction]
			public NativeArray<bool> _done;
			public int _rightBorder;
			public int _leftBorder;


			public void Execute(int index)
			{
				if (_done[index])
					return;
				if (index != _formerPivots.Length - 1 && _formerPivots[index + 1] < 0)
					return;

				int right = index;
				int left = right;
				while (left != 0 && _formerPivots[left-1] < 0)
					left--;

				if (right <= left)
				{
					_pivots[index] = index;
					_done[index] = true;
					return;
				}
				
				Vector3 pivot = _points[right];

				int i = left;
				int j = right - 1;
				while (i < j)
				{
					// Find the first element >= pivot
					while (_points[i][_compareValueIndex] < pivot[_compareValueIndex])
						i++;

					// Find the last element < pivot
					while (j > left && _points[j][_compareValueIndex] >= pivot[_compareValueIndex])
						j--;

					// If the greater element is left of the lesser element, switch them
					if (i < j)
					{
						Swap(i, j);

						i++;
						j--;
					}
				}
				// i == j means we haven't checked this index yet.
				// Move i right if necessary so that i marks the start of the right array.
				if (i == j && _points[i][_compareValueIndex] < pivot[_compareValueIndex])
					i++;

				// Move pivot element to its final position
				if (_points[i] != pivot)
					Swap(i, right);

				_pivots[i] = i;
				_done[i] = true;
			}

			private void Swap(int i, int j)
			{
				int iIndex = _indexPermuations[i];
				Vector3 iPoint = _points[i];
				_indexPermuations[i] = _indexPermuations[j];
				_points[i] = _points[j];
				_indexPermuations[j] = iIndex;
				_points[j] = iPoint;
			}
		}
	}
}