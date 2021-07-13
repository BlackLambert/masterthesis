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
		private Vector3Sorter _sorter;

		public Vector3BinaryKDTree(IList<Vector3> nodes, Vector3Sorter sorter)
		{
			ValidateNodes(nodes);
			_nodes = nodes;
			_sorter = sorter;
			BuildTree(nodes);
		}

		public int GetNearestTo(Vector3 sample)
		{
			int internIndex = GetNearestToRecursive(sample, _root);
			int result = _indexPermutation[internIndex];
			return result;
		}

		private int GetNearestToRecursive(Vector3 sample, int nodeIndex, int depth = 0)
		{
			int[] children = _nodeToChildren[nodeIndex];

			bool nodeIsALeave = children.Length == 0;
			if (nodeIsALeave)
				return nodeIndex;

			bool nodeHasOneChild = children.Length == 1;
			int nextChildIndex = GetNextChildIndex(sample, nodeIndex, depth);
			int nextNodeIndex = nodeHasOneChild ? children[0] : children[nextChildIndex];
			int childSmallest = GetNearestToRecursive(sample, nextNodeIndex, depth + 1);

			float nodeDistanceToSampleSqr = (_nodes[_indexPermutation[nodeIndex]] - sample).sqrMagnitude;
			float childSmallestDistanceToSampleSqr = (_nodes[_indexPermutation[childSmallest]] - sample).sqrMagnitude;
			int currentSmallest = nodeDistanceToSampleSqr < childSmallestDistanceToSampleSqr ? nodeIndex : childSmallest;
			float currentSmallestDistanceToSample = nodeDistanceToSampleSqr < childSmallestDistanceToSampleSqr ?
				Mathf.Sqrt(nodeDistanceToSampleSqr) :
				Mathf.Sqrt(childSmallestDistanceToSampleSqr);

			int otherChildIndex = (nextChildIndex + 1) % 2;
			float compareValueDistance = CalculateCompareValueDistanceOf(sample, _nodes[_indexPermutation[nodeIndex]], depth);

			bool currentIsSmallestOfThisBranch = currentSmallestDistanceToSample <= compareValueDistance || nodeHasOneChild;
			if (currentIsSmallestOfThisBranch)
				return currentSmallest;

			int otherChildSmallest = GetNearestToRecursive(sample, _nodeToChildren[nodeIndex][otherChildIndex], depth + 1);
			float otherChildSmallestDistanceToSample = (_nodes[_indexPermutation[otherChildSmallest]] - sample).magnitude;

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

		private void GetNearestToWithinRecursive(Vector3 sample, float maxDistance, List<int> result, int nodeIndex, int depth = 0)
		{
			int[] children = _nodeToChildren[nodeIndex];

			bool nodeIsALeave = children.Length == 0;
			float nodeDistanceToSample = (_nodes[_indexPermutation[nodeIndex]] - sample).magnitude;
			bool nodeIsWithinMaxDistance = nodeDistanceToSample <= maxDistance;
			if(nodeIsWithinMaxDistance)
				result.Add(_indexPermutation[nodeIndex]);
			if (nodeIsALeave)
				return;
			bool nodeHasOneChild = children.Length == 1;
			int nextChildIndex = GetNextChildIndex(sample, nodeIndex, depth);
			int nextNodeIndex = nodeHasOneChild ? children[0] : children[nextChildIndex];
			GetNearestToWithinRecursive(sample, maxDistance, result, nextNodeIndex, depth + 1);
			float compareValueDistance = CalculateCompareValueDistanceOf(sample, _nodes[_indexPermutation[nodeIndex]], depth);
			if (compareValueDistance > maxDistance || nodeHasOneChild)
				return;

			int otherChildIndex = (nextChildIndex + 1) % 2;
			GetNearestToWithinRecursive(sample, maxDistance, result, children[otherChildIndex], depth + 1);
		}

		private float CalculateCompareValueDistanceOf(Vector3 v0, Vector3 v1, int depth)
		{
			float nodeCompareValue = GetTreeTraverseCompareValue(depth, v1);
			float sampleCompareValue = GetTreeTraverseCompareValue(depth, v0);
			float compareValueDistance = Mathf.Abs(nodeCompareValue - sampleCompareValue);
			return compareValueDistance;
		}

		private int GetNextChildIndex(Vector3 sample, int nodeIndex, int depth)
		{
			float nodeCompareValue = GetTreeTraverseCompareValue(depth, _nodes[_indexPermutation[nodeIndex]]);
			float sampleCompareValue = GetTreeTraverseCompareValue(depth, sample);
			int nextChildIndex = sampleCompareValue < nodeCompareValue ? 0 : 1;
			return nextChildIndex;
		}

		private float GetTreeTraverseCompareValue(int depth, Vector3 value)
		{
			switch(depth % 3)
			{
				case 0:
					return value.x;
				case 1:
					return value.y;
				case 2:
					return value.z;
				default:
					throw new NotImplementedException();
			}
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
			RecursiveTreeBuilder builder = new RecursiveTreeBuilder(nodes, _sorter);
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
			private Vector3Sorter _sorter;

			public RecursiveTreeBuilder(IList<Vector3> nodes, Vector3Sorter sorter)
			{
				_permutations = nodes.ToList();
				IndexPermutation = CreateIndexPermutations(nodes.Count);
				_sorter = sorter;
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
				_sorter.Sort(_permutations, IndexPermutation, indexRange, vectorIndex);
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