using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
	public class Vector3SelectionSorter : Vector3SorterBase, Vector3Sorter
	{
		public void Sort(IList<Vector3> points, int compareValueIndex = 0)
		{
			ValidateCompareValueIndex(compareValueIndex);
			Sort(points, new Vector2Int(0, points.Count - 1), compareValueIndex);
		}

		public void Sort(IList<Vector3> points, Vector2Int indexRange, int compareValueIndex = 0)
		{
			IList<int> permutations = new int[points.Count];
			Sort(points, permutations, indexRange, compareValueIndex);
		}

		public void Sort(IList<Vector3> points, IList<int> indexPermutations, Vector2Int indexRange, int compareValueIndex = 0)
		{
			ValidateIndexRange(points, indexRange);
			for (int i = indexRange.x; i <= indexRange.y; i++)
			{
				int smallest = i;
				for (int j = i + 1; j <= indexRange.y; j++)
					if (points[j][compareValueIndex] < points[smallest][compareValueIndex])
						smallest = j;
				if (smallest == i)
					continue;

				Vector3 iVector = points[i];
				points[i] = points[smallest];
				points[smallest] = iVector;

				int iIndex = indexPermutations[i];
				indexPermutations[i] = indexPermutations[smallest];
				indexPermutations[smallest] = iIndex;
			}
		}

		public void Sort(IList<Vector3> points, IList<int> indexPermutations, int compareValueIndex = 0)
		{
			Sort(points, indexPermutations, new Vector2Int(0, points.Count - 1), compareValueIndex);
		}
	}
}