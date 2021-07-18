using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SBaier.Master
{
    public class Vector3BinaryKDTree
	{
		public int Depth { get; private set; }

		private int _root;
		private IList<Vector3> _nodes;
		private IList<int> _indexPermutation;
		private Dictionary<int, int[]> _nodeToChildren;
		private Vector3QuickSelector _quickSelector;

		public Vector3BinaryKDTree(IList<Vector3> nodes, Vector3QuickSelector quickSelector)
		{
			ValidateNodes(nodes);
			_nodes = nodes;
			_quickSelector = quickSelector;
			BuildTree(nodes);
		}

		public int GetNearestTo(Vector3 sample)
		{
			int internIndex = GetNearestToRecursive(sample, _root);
			int result = _indexPermutation[internIndex];
			return result;
		}

		private int GetNearestToRecursive(Vector3 sample, int nodeIndex, int compareValueIndex = 0)
		{
			int[] children = _nodeToChildren[nodeIndex];

			bool nodeIsALeave = children.Length == 0;
			if (nodeIsALeave)
				return nodeIndex;

			bool nodeHasOneChild = children.Length == 1;
			Vector3 node = _nodes[_indexPermutation[nodeIndex]];
			float nodeCompareValue = node[compareValueIndex];
			float sampleCompareValue = sample[compareValueIndex];
			int nextChildIndex = GetNextChildIndex(sampleCompareValue, nodeCompareValue);
			int nextNodeIndex = nodeHasOneChild ? children[0] : children[nextChildIndex];
			int nextCompareValueIndex = (compareValueIndex + 1) % 3;
			int childSmallest = GetNearestToRecursive(sample, nextNodeIndex, nextCompareValueIndex);
			Vector3 nodeChild1 = _nodes[_indexPermutation[childSmallest]];

			float nodeDistanceToSampleSqr = (node - sample).sqrMagnitude;
			float childSmallestDistanceToSampleSqr = (nodeChild1 - sample).sqrMagnitude;
			int currentSmallest = nodeDistanceToSampleSqr < childSmallestDistanceToSampleSqr ? nodeIndex : childSmallest;
			float currentSmallestDistanceToSample = nodeDistanceToSampleSqr < childSmallestDistanceToSampleSqr ?
				Mathf.Sqrt(nodeDistanceToSampleSqr) :
				Mathf.Sqrt(childSmallestDistanceToSampleSqr);

			int otherChildIndex = (nextChildIndex + 1) % 2;
			float compareValueDistance = CalculateCompareValueDistanceOf(sampleCompareValue, nodeCompareValue);

			bool currentIsSmallestOfThisBranch = currentSmallestDistanceToSample <= compareValueDistance || nodeHasOneChild;
			if (currentIsSmallestOfThisBranch)
				return currentSmallest;

			int otherChildSmallest = GetNearestToRecursive(sample, children[otherChildIndex], nextCompareValueIndex);
			Vector3 nodeChild2 = _nodes[_indexPermutation[otherChildSmallest]];
			float otherChildSmallestDistanceToSample = (nodeChild2 - sample).magnitude;

			if (otherChildSmallestDistanceToSample < currentSmallestDistanceToSample)
				return otherChildSmallest;
			return currentSmallest;
		}

		public IList<int> GetNearestToWithin(Vector3 sample, float maxDistance)
		{
			ValidateMaxDIstance(maxDistance);
			List<int> result = new List<int>();
			GetNearestToWithinRecursive(sample, maxDistance, result, _root);
			return result;
		}

		private void GetNearestToWithinRecursive(Vector3 sample, float maxDistance, List<int> result, int nodeIndex, int compareValueIndex = 0)
		{
			int actualNodeIndex = _indexPermutation[nodeIndex];
			Vector3 node = _nodes[actualNodeIndex];
			float nodeDistanceToSample = (node - sample).magnitude;
			bool nodeIsWithinMaxDistance = nodeDistanceToSample <= maxDistance;

			if (nodeIsWithinMaxDistance)
				result.Add(actualNodeIndex);

			int[] children = _nodeToChildren[nodeIndex];
			bool nodeIsALeave = children.Length == 0;

			if (nodeIsALeave)
				return;

			float nodeCompareValue = node[compareValueIndex];
			float sampleCompareValue = sample[compareValueIndex];
			int nextChildIndex = GetNextChildIndex(sampleCompareValue, nodeCompareValue);

			bool nodeHasOneChild = children.Length == 1;
			int nextNodeIndex = nodeHasOneChild ? children[0] : children[nextChildIndex];
			int nextCompareValueIndex = (compareValueIndex + 1) % 3;
			GetNearestToWithinRecursive(sample, maxDistance, result, nextNodeIndex, nextCompareValueIndex);
			float compareValueDistance = CalculateCompareValueDistanceOf(sampleCompareValue, nodeCompareValue);
			if (compareValueDistance > maxDistance || nodeHasOneChild)
				return;

			int otherChildIndex = (nextChildIndex + 1) % 2;
			GetNearestToWithinRecursive(sample, maxDistance, result, children[otherChildIndex], nextCompareValueIndex);
		}

		private float CalculateCompareValueDistanceOf(float compareValue1, float compareValue2)
		{
			float compareValueDistance = Mathf.Abs(compareValue1 - compareValue2);
			return compareValueDistance;
		}

		private int GetNextChildIndex(float sampleCompareValue, float nodeCompareValue)
		{
			int nextChildIndex = sampleCompareValue < nodeCompareValue ? 0 : 1;
			return nextChildIndex;
		}

		private void ValidateNodes(IList<Vector3> vectors)
		{
			if (vectors.Count == 0)
				throw new ArgumentException();
		}

		private static void ValidateMaxDIstance(float maxDistance)
		{
			if (maxDistance <= 0)
				throw new ArgumentOutOfRangeException();
		}


		private void BuildTree(IList<Vector3> nodes)
		{
			RecursiveTreeBuilder builder = new RecursiveTreeBuilder(nodes, _quickSelector);
			builder.Build();
			_nodeToChildren = builder.NodeToChildren;
			_root = builder.Root;
			_indexPermutation = builder.IndexPermutation;
			Depth = builder.Depth;
		}

		private class RecursiveTreeBuilder
		{
			public int Root { get; private set; }
			public Dictionary<int, int[]> NodeToChildren { get; private set; }
			public int[] IndexPermutation { get; private set; }
			public int Depth { get; private set; } = 0;
			private IList<Vector3> _permutations;
			private Vector3QuickSelector _quickSelector;

			public RecursiveTreeBuilder(IList<Vector3> nodes, Vector3QuickSelector quickSelector)
			{
				_permutations = nodes.ToList();
				IndexPermutation = CreateIndexPermutations(nodes.Count);
				_quickSelector = quickSelector;
			}

			public void Build()
			{
				NodeToChildren = new Dictionary<int, int[]>(_permutations.Count);
				BuildTreeRecursive(0, new Vector2Int(0, _permutations.Count - 1));
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
				UpdateDepth(depth);
				NodeToChildren.Add(nodeIndex, new int[0] { });
			}

			private void BuildOneChildNode(int depth, Vector2Int indexRange, int nodeIndex)
			{
				NodeToChildren.Add(nodeIndex, new int[1] { -1 });
				Vector2Int newRange;
				if(indexRange.x == nodeIndex)
					newRange = SkipNodesBeforeMedian(indexRange, nodeIndex);
				else
					newRange = TakeNodesBeforeMedian(indexRange, nodeIndex);
				BuildTreeRecursive(depth + 1, newRange, nodeIndex, 0);
			}

			private void BuildTwoChildNodes(int depth, Vector2Int indexRange, int nodeIndex)
			{
				NodeToChildren.Add(nodeIndex, new int[2] { -1, -1 });
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