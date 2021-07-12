using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
	public class Vector3MergeSorter : Vector3SorterBase, Vector3Sorter
	{
		public void Sort(IList<Vector3> points, int compareValueIndex = 0)
		{
			ValidateCompareValueIndex(compareValueIndex);
			Sort(points, new Vector2Int(0, points.Count - 1), compareValueIndex);
		}

		public void Sort(IList<Vector3> points, Vector2Int indexRange, int compareValueIndex = 0)
		{
			ValidateIndexRange(points, indexRange);
		}

		public void Sort(IList<Vector3> points, IList<int> indexPermutations, Vector2Int indexRange, int compareValueIndex = 0)
		{
			throw new System.NotImplementedException();
		}

		public void Sort(IList<Vector3> points, IList<int> indexPermutations, int compareValueIndex = 0)
		{
			throw new System.NotImplementedException();
		}
	}
}