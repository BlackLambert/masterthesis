using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
	public class SelectionSorter<T, U> : SorterBase<T, U> where U : IComparable<U>
	{
		private Func<T, int, U> _compareValueSelect;

		public SelectionSorter(Func<T, int, U> compareValueSelect) : base()
		{
			_compareValueSelect = compareValueSelect;
		}

		protected override void DoSort(T[] points, int[] indexPermutations, Vector2Int indexRange, int compareValueIndex)
		{
			for (int i = indexRange.x; i <= indexRange.y; i++)
			{
				int smallest = i;
				for (int j = i + 1; j <= indexRange.y; j++)
				{
					U pointCompareValue = _compareValueSelect(points[j], compareValueIndex);
					U smallestCompareValue = _compareValueSelect(points[smallest], compareValueIndex);
					if (pointCompareValue.CompareTo(smallestCompareValue) < 0)
						smallest = j;
				}
				if (smallest == i)
					continue;

				T iElement = points[i];
				points[i] = points[smallest];
				points[smallest] = iElement;

				int iIndex = indexPermutations[i];
				indexPermutations[i] = indexPermutations[smallest];
				indexPermutations[smallest] = iIndex;
			}
		}
	}
}