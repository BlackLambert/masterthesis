using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public class PartitionTest : MonoBehaviour
    {
		[SerializeField]
		private int _nodesCount;
		Vector3[] _nodes;
		int[] _indexPermut;

        // Start is called before the first frame update
        void Start()
        {
			_nodes = CreateNodes();
			_indexPermut = new int[_nodes.Length];

			int i = Partition(_nodes, _indexPermut, 0, _nodes.Length - 1, 0);
        }

		private Vector3[] CreateNodes()
		{
			Vector3[] result = new Vector3[_nodesCount];
			for (int i = 0; i < _nodesCount; i++)
			{
				float x = UnityEngine.Random.value;
				float y = UnityEngine.Random.value;
				float z = UnityEngine.Random.value;
				result[i] = new Vector3(x, y, z);
			}
			return result;
		}

		private int Partition(Vector3[] points, int[] indexPermutations, int left, int right, int compareValueIndex)
		{
			Vector3 pivot = points[right];
			float pivotCompareValue = _compareValueSelect(pivot, compareValueIndex);

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

		private float _compareValueSelect(Vector3 pivot, int compareValueIndex)
		{
			switch(compareValueIndex)
			{
				case 0:
					return pivot.x;
				case 1:
					return pivot.y;
				case 2:
					return pivot.z;
			}
			return pivot[compareValueIndex];
		}

		private void Swap(int i, int j, Vector3[] points, int[] indexPermutations)
		{
			int iIndex = indexPermutations[i];
			indexPermutations[i] = indexPermutations[j];
			indexPermutations[j] = iIndex;

			Vector3 iPoint = points[i];
			points[i] = points[j];
			points[j] = iPoint;
		}
	}
}