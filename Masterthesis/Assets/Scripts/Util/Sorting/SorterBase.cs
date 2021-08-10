using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public abstract class SorterBase<T, U> : Sorter<T> where U : IComparable<U>
	{
		public SorterBase()
		{
		}

		public void Sort(T[] points, int compareValueIndex = 0)
		{
			ValidateCompareValueIndex(compareValueIndex);
			Sort(points, new Vector2Int(0, points.Length - 1), compareValueIndex);
		}

		public void Sort(T[] points, int[] indexPermutations, Vector2Int indexRange, int compareValueIndex = 0)
		{
			ValidateIndexRange(points, indexRange);
			DoSort(points, indexPermutations, indexRange, compareValueIndex);
		}

		public void Sort(T[] points, Vector2Int indexRange, int compareValueIndex = 0)
		{
			int[] permutations = new int[points.Length];
			Sort(points, permutations, indexRange, compareValueIndex);
		}

		public void Sort(T[] points, int[] indexPermutations, int compareValueIndex = 0)
		{
			Sort(points, indexPermutations, new Vector2Int(0, points.Length - 1), compareValueIndex);
		}

		protected abstract void DoSort(T[] points, int[] indexPermutations, Vector2Int indexRange, int compareValueIndex);


		protected static void ValidateIndexRange(T[] points, Vector2Int valueRange)
		{
			if (valueRange.x > valueRange.y || valueRange.x < 0 || valueRange.y >= points.Length)
				throw new ArgumentOutOfRangeException();
		}

		protected static void ValidateCompareValueIndex(int compareValueIndex)
		{
			if (compareValueIndex < 0 || compareValueIndex > 2)
				throw new ArgumentOutOfRangeException();
		}
	}
}