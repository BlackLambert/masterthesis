using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
	/// <summary>
	/// Source: https://www.happycoders.eu/de/algorithmen/quicksort/
	/// </summary>
	public class QuickSorter<T, U> : SorterBase<T, U>, QuickSelector<T> where U : IComparable<U>
	{
		private Func<T, int, U> _compareValueSelect;

		public QuickSorter(
			Func<T, int, U> compareValueSelect
			) : base()
		{
			_compareValueSelect = compareValueSelect;
		}

		public void QuickSelect(T[] points, int[] indexPermutations, Vector2Int indexRange, int compareValueIndex, int selectedIndex)
		{
			ValidateSelectedIndex(points, selectedIndex);
			QuickSelectRecursive(points, indexPermutations, indexRange.x, indexRange.y, compareValueIndex, selectedIndex);
		}

		protected override void DoSort(T[] points, int[] indexPermutations, Vector2Int indexRange, int compareValueIndex)
		{
			Quicksort(points, indexPermutations, indexRange.x, indexRange.y, compareValueIndex);
		}

		private void Quicksort(T[] points, int[] indexPermutations, int left, int right, int compareValueIndex)
		{
			if (left >= right) return;

			int pivotPos = Partition(points, indexPermutations, left, right, compareValueIndex);
			Quicksort(points, indexPermutations, left, pivotPos - 1, compareValueIndex);
			Quicksort(points, indexPermutations, pivotPos + 1, right, compareValueIndex);
		}

		private int Partition(T[] points, int[] indexPermutations, int left, int right, int compareValueIndex)
		{
			T pivot = points[right];
			U pivotCompareValue = _compareValueSelect(pivot, compareValueIndex);

			int i = left;
			int j = right - 1;
			while (i < j)
			{
				// Find the first element >= pivot
				while (_compareValueSelect(points[i], compareValueIndex).CompareTo(pivotCompareValue) < 0)
					i++;

				// Find the last element < pivot
				while (j > left && _compareValueSelect(points[j], compareValueIndex).CompareTo(pivotCompareValue) >= 0)
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
			if (i == j && _compareValueSelect(points[i], compareValueIndex).CompareTo(pivotCompareValue) < 0)
				i++;

			// Move pivot element to its final position
			if (!points[i].Equals(pivot))
				Swap(i, right, points, indexPermutations);
			return i;
		}

		private void Swap(int i, int j, T[] points, int[] indexPermutations)
		{
			int iIndex = indexPermutations[i];
			indexPermutations[i] = indexPermutations[j];
			indexPermutations[j] = iIndex;

			T iPoint = points[i];
			points[i] = points[j];
			points[j] = iPoint;
		}

		private void QuickSelectRecursive(T[] points, int[] indexPermutations, int left, int right, int compareValueIndex, int selectedIndex)
		{
			if (left >= right) 
				return;

			int pivotPos = Partition(points, indexPermutations, left, right, compareValueIndex);
			if (pivotPos == selectedIndex)
				return;
			if(pivotPos > selectedIndex)
				QuickSelectRecursive(points, indexPermutations, left, pivotPos - 1, compareValueIndex, selectedIndex);
			else if (pivotPos < selectedIndex)
				QuickSelectRecursive(points, indexPermutations, pivotPos + 1, right, compareValueIndex, selectedIndex);
		}

		private void ValidateSelectedIndex(T[] points, int selectedIndex)
		{
			if (selectedIndex >= points.Length || selectedIndex < 0)
				throw new ArgumentOutOfRangeException();
		}
	}
}