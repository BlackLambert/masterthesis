using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SBaier.Master
{
    public class Vector3BinaryKDTreeFactory
    {
        private QuickSelector<Vector3> _quickSelector;

        public Vector3BinaryKDTreeFactory(QuickSelector<Vector3> quickSelector)
		{
            _quickSelector = quickSelector;
        }

        public KDTree<Vector3> Create(Vector3[] nodes)
		{
			RecursiveTreeBuilder builder = new RecursiveTreeBuilder(nodes, _quickSelector);
			builder.Build();
			return new Vector3BinaryKDTree(nodes, builder.Root, builder.NodeToChildren, builder.IndexPermutation, builder.Depth);
		}

		private class RecursiveTreeBuilder
		{
			public int Root { get; private set; }
			public int[][] NodeToChildren { get; private set; }
			public int[] IndexPermutation { get; private set; }
			public int Depth { get; private set; } = 0;
			private Vector3[] _permutations;
			private QuickSelector<Vector3> _quickSelector;

			public RecursiveTreeBuilder(Vector3[] nodes, QuickSelector<Vector3> quickSelector)
			{
				_permutations = nodes.ToArray();
				IndexPermutation = CreateIndexPermutations(nodes.Length);
				_quickSelector = quickSelector;
			}

			public void Build()
			{
				NodeToChildren = new int[_permutations.Length][];
				BuildTreeRecursive(0, new Vector2Int(0, _permutations.Length - 1));
			}

			private void BuildTreeRecursive(int depth, Vector2Int indexRange, int previousNodeIndex = -1, int recursionIndex = 0)
			{
				OrderNodesByCompareValue(depth, indexRange);
				int median = GetMedian(indexRange);
				if (depth == 0)
					Root = median;
				UpdatePreviousNodeChildren(previousNodeIndex, recursionIndex, median);
				BuildBranch(depth, indexRange, median);
			}

			private void BuildBranch(int depth, Vector2Int indexRange, int currentNodeIndex)
			{
				int count = indexRange.y - indexRange.x + 1;
				if (count == 1)
					BuildLeave(depth, currentNodeIndex);
				else if (count == 2)
					BuildOneChildNode(depth, indexRange, currentNodeIndex);
				else if (count > 2)
					BuildTwoChildNodes(depth, indexRange, currentNodeIndex);
			}

			private void BuildLeave(int depth, int nodeIndex)
			{
				NodeToChildren[nodeIndex] = new int[0];
				UpdateDepth(depth);
			}

			private void BuildOneChildNode(int depth, Vector2Int indexRange, int nodeIndex)
			{
				NodeToChildren[nodeIndex] = new int[] { -1 };
				Vector2Int newRange;
				if (indexRange.x == nodeIndex)
					newRange = SkipNodesBeforeMedian(indexRange, nodeIndex);
				else
					newRange = TakeNodesBeforeMedian(indexRange, nodeIndex);
				BuildTreeRecursive(depth + 1, newRange, nodeIndex, 0);
			}

			private void BuildTwoChildNodes(int depth, Vector2Int indexRange, int nodeIndex)
			{
				NodeToChildren[nodeIndex] = new int[] { -1, -1 };
				Vector2Int firstNewIndexRange = TakeNodesBeforeMedian(indexRange, nodeIndex);
				Vector2Int secondNewIndexRange = SkipNodesBeforeMedian(indexRange, nodeIndex);
				BuildTreeRecursive(depth + 1, firstNewIndexRange, nodeIndex, 0);
				BuildTreeRecursive(depth + 1, secondNewIndexRange, nodeIndex, 1);
			}

			private void UpdatePreviousNodeChildren(int previousNodeIndex, int recursionIndex, int currentNodeIndex)
			{
				if (previousNodeIndex >= 0)
					NodeToChildren[previousNodeIndex][recursionIndex] = currentNodeIndex;
			}

			private Vector2Int SkipNodesBeforeMedian(Vector2Int range, int median)
			{
				Vector2Int result = new Vector2Int(median + 1, range.y);
				return result;
			}

			private Vector2Int TakeNodesBeforeMedian(Vector2Int range, int median)
			{
				Vector2Int result = new Vector2Int(range.x, median - 1);
				return result;
			}

			private void UpdateDepth(int depth)
			{
				Depth = depth > Depth ? depth : Depth;
			}

			private int GetMedian(Vector2Int indexRange)
			{
				int range = indexRange.y - indexRange.x;
				return (range / 2) + indexRange.x;
			}

			private void OrderNodesByCompareValue(int depth, Vector2Int indexRange)
			{
				const int vectorDimension = 3;
				int vectorIndex = depth % vectorDimension;
				_quickSelector.QuickSelect(_permutations, IndexPermutation, indexRange, vectorIndex, GetMedian(indexRange));
			}

			private int[] CreateIndexPermutations(int count)
			{
				int[] result = new int[count];
				for (int i = 0; i < count; i++)
					result[i] = i;
				return result;
			}
		}
	}
}
