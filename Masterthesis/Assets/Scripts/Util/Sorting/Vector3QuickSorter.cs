using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
	/// <summary>
	/// Source: https://www.happycoders.eu/de/algorithmen/quicksort/
	/// </summary>
	public class Vector3QuickSorter : Vector3SorterBase, Vector3Sorter
	{
		protected override void DoSort(IList<Vector3> points, IList<int> indexPermutations, Vector2Int indexRange, int compareValueIndex)
		{
			Quicksort(points, indexPermutations, indexRange.x, indexRange.y, compareValueIndex);
		}

		private void Quicksort(IList<Vector3> points, IList<int> indexPermutations, int left, int right, int compareValueIndex)
		{
			if (left >= right) return;

			int pivotPos = Partition(points, indexPermutations, left, right, compareValueIndex);
			Quicksort(points, indexPermutations, left, pivotPos - 1, compareValueIndex);
			Quicksort(points, indexPermutations, pivotPos + 1, right, compareValueIndex);
		}

		private int Partition(IList<Vector3> points, IList<int> indexPermutations, int left, int right, int compareValueIndex)
		{
			Vector3 pivot = points[right];

			int i = left;
			int j = right - 1;
			while (i < j)
			{
				// Find the first element >= pivot
				while (points[i][compareValueIndex] < pivot[compareValueIndex])
					i++;

				// Find the last element < pivot
				while (j > left && points[j][compareValueIndex] >= pivot[compareValueIndex])
					j--;

				// If the greater element is left of the lesser element, switch them
				if (i < j)
				{
					Swap(i, j, points, indexPermutations);

					i++;
					j--;
				}
			}
			// i == j means we haven't checked this index yet.
			// Move i right if necessary so that i marks the start of the right array.
			if (i == j && points[i][compareValueIndex] < pivot[compareValueIndex])
				i++;

			// Move pivot element to its final position
			if (points[i] != pivot)
				Swap(i, right, points, indexPermutations);
			return i;
		}

		private void Swap(int i, int j, IList<Vector3> points, IList<int> indexPermutations)
		{
			int iIndex = indexPermutations[i];
			Vector3 iPoint = points[i];
			indexPermutations[i] = indexPermutations[j];
			points[i] = points[j];
			indexPermutations[j] = iIndex;
			points[j] = iPoint;
		}
	}
}