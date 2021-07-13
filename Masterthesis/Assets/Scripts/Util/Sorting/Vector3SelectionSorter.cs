using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
	public class Vector3SelectionSorter : Vector3SorterBase
	{
		protected override void DoSort(IList<Vector3> points, IList<int> indexPermutations, Vector2Int indexRange, int compareValueIndex)
		{
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
	}
}