using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SBaier.Master
{
    public class Vector3BinaryKDTree
	{
		public IList<Vector3> Nodes { get; private set; }
		public int Depth { get; private set; }

		private IList<int> _indexPermutation;
		private Dictionary<int, int[]> _nodeToChildren;
		private Vector3Sorter _sorter;

		public Vector3BinaryKDTree(IList<Vector3> nodes, Vector3Sorter sorter)
		{
			_sorter = sorter;
			ValidateNodes(nodes);
			BuildTree(nodes);
		}

		public int GetNearestTo(Vector3 sample)
		{
			int result = _indexPermutation[GetNearestToRecursive(sample)];
			return result;
		}

		private int GetNearestToRecursive(Vector3 sample, int nodeIndex = 0, int depth = 0)
		{
			int[] children = _nodeToChildren[nodeIndex];

			bool nodeIsALeave = children.Length == 0;
			if (nodeIsALeave)
				return nodeIndex;

			bool nodeHasOneChild = children.Length == 1;
			int nextChildIndex = GetNextChildIndex(sample, nodeIndex, depth);
			int nextNodeIndex = nodeHasOneChild ? children[0] : children[nextChildIndex];
			int childSmallest = GetNearestToRecursive(sample, nextNodeIndex, depth + 1);

			float nodeDistanceToSampleSqr = (Nodes[nodeIndex] - sample).sqrMagnitude;
			float childSmallestDistanceToSampleSqr = (Nodes[childSmallest] - sample).sqrMagnitude;
			int currentSmallest = nodeDistanceToSampleSqr < childSmallestDistanceToSampleSqr ? nodeIndex : childSmallest;
			float currentSmallestDistanceToSample = nodeDistanceToSampleSqr < childSmallestDistanceToSampleSqr ?
				Mathf.Sqrt(nodeDistanceToSampleSqr) :
				Mathf.Sqrt(childSmallestDistanceToSampleSqr);

			int otherChildIndex = (nextChildIndex + 1) % 2;
			float compareValueDistance = CalculateCompareValueDistanceOf(sample, Nodes[nodeIndex], depth);

			bool currentIsSmallestOfThisBranch = currentSmallestDistanceToSample <= compareValueDistance || nodeHasOneChild;
			if (currentIsSmallestOfThisBranch)
				return currentSmallest;

			int otherChildSmallest = GetNearestToRecursive(sample, _nodeToChildren[nodeIndex][otherChildIndex], depth + 1);
			float otherChildSmallestDistanceToSample = (Nodes[otherChildSmallest] - sample).magnitude;

			if (otherChildSmallestDistanceToSample < currentSmallestDistanceToSample)
				return otherChildSmallest;
			return currentSmallest;
		}

		public IList<int> GetNearestToWithin(Vector3 sample, float maxDistance)
		{
			ValidateMaxDIstance(maxDistance);
			List<int> result = new List<int>();
			GetNearestToWithinRecursive(sample, maxDistance, ref result);
			return result;
		}

		private void GetNearestToWithinRecursive(Vector3 sample, float maxDistance, ref List<int> result, int nodeIndex = 0, int depth = 0)
		{
			int[] children = _nodeToChildren[nodeIndex];

			bool nodeIsALeave = children.Length == 0;
			float nodeDistanceToSample = (Nodes[nodeIndex] - sample).magnitude;
			bool nodeIsWithinMaxDistance = nodeDistanceToSample <= maxDistance;
			if(nodeIsWithinMaxDistance)
				result.Add(_indexPermutation[nodeIndex]);
			if (nodeIsALeave)
				return;
			bool nodeHasOneChild = children.Length == 1;
			int nextChildIndex = GetNextChildIndex(sample, nodeIndex, depth);
			int nextNodeIndex = nodeHasOneChild ? children[0] : children[nextChildIndex];
			GetNearestToWithinRecursive(sample, maxDistance, ref result, nextNodeIndex, depth + 1);
			float compareValueDistance = CalculateCompareValueDistanceOf(sample, Nodes[nodeIndex], depth);
			if (compareValueDistance > maxDistance || nodeHasOneChild)
				return;

			int otherChildIndex = (nextChildIndex + 1) % 2;
			GetNearestToWithinRecursive(sample, maxDistance, ref result, children[otherChildIndex], depth + 1);
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
			float nodeCompareValue = GetTreeTraverseCompareValue(depth, Nodes[nodeIndex]);
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
			Nodes = builder.Nodes;
			_indexPermutation = builder.IndexPermutation;
			Depth = builder.Depth;
		}

		private class RecursiveTreeBuilder
		{
			public List<Vector3> Nodes { get; private set; }
			public Dictionary<int, int[]> NodeToChildren { get; private set; }
			public List<int> IndexPermutation { get; private set; }
			public int Depth { get; private set; } = 0;
			private IList<Vector3> _input;
			private Vector3Sorter _sorter;

			private Vector3[] _permutations;
			private int[] _indexPermutations;

			public RecursiveTreeBuilder(IList<Vector3> nodes, Vector3Sorter sorter)
			{
				_input = nodes;
				_permutations = nodes.ToArray();
				_indexPermutations = CreateIndexPermutations(nodes.Count);
				_sorter = sorter;
			}

			public void Build()
			{
				Nodes = new List<Vector3>(_input.Count);
				_sorter.Sort(_permutations, _indexPermutations, new Vector2Int(0, _permutations.Length-1), 0);
				NodeToChildren = new Dictionary<int, int[]>(_input.Count);
				IndexPermutation = new List<int>(_input.Count);
				BuildTreeRecursive(0, _input);
			}

			private void BuildTreeRecursive(int depth, IList<Vector3> nodes, int previousNodeIndex = -1, int recursionIndex = 0)
			{
				int currentNodeIndex = Nodes.Count;
				IList<Vector3> orderedNodes = OrderNodesByCompareValue(depth, nodes);
				int median = GetMedian(orderedNodes);
				AddNodeToTree(orderedNodes[median]);
				UpdatePreviousNodeChildren(previousNodeIndex, recursionIndex, currentNodeIndex);
				BuildBranch(depth, orderedNodes, currentNodeIndex, median);
			}

			private void BuildBranch(int depth, IList<Vector3> nodes, int currentNodeIndex, int median)
			{
				if (nodes.Count == 1)
					BuildLeave(depth, currentNodeIndex);
				else if (nodes.Count == 2)
					BuildOneChildNode(depth, nodes, median, currentNodeIndex);
				else if (nodes.Count > 2)
					BuildTwoChildNodes(depth, nodes, median, currentNodeIndex);
			}

			private void UpdatePreviousNodeChildren(int previousNodeIndex, int recursionIndex, int currentNodeIndex)
			{
				if (previousNodeIndex >= 0)
					NodeToChildren[previousNodeIndex][recursionIndex] = currentNodeIndex;
			}

			private void AddNodeToTree(Vector3 node)
			{
				Nodes.Add(node);
				UpdateIndexPermutations(node);
			}

			private void UpdateIndexPermutations(Vector3 medianNode)
			{
				int inputIndex = _input.IndexOf(medianNode);
				IndexPermutation.Add(inputIndex);
			}

			private void BuildTwoChildNodes(int depth, IList<Vector3> nodes, int median, int nodeIndex)
			{
				NodeToChildren.Add(nodeIndex, new int[2] { -1, -1 });
				List<Vector3> firstNewNodes = TakeNodesBeforeMedian(nodes, median);
				List<Vector3> secondNewNodes = SkipNodesBeforeMedian(nodes, median);
				BuildTreeRecursive(depth + 1, firstNewNodes, nodeIndex, 0);
				BuildTreeRecursive(depth + 1, secondNewNodes, nodeIndex, 1);
			}

			private void BuildOneChildNode(int depth, IList<Vector3> nodes, int median, int nodeIndex)
			{
				NodeToChildren.Add(nodeIndex, new int[1] { -1 });
				List<Vector3> newNodes = TakeNodesBeforeMedian(nodes, median);
				BuildTreeRecursive(depth + 1, newNodes, nodeIndex, 0);
			}

			private void BuildLeave(int depth, int nodeIndex)
			{
				UpdateDepth(depth);
				NodeToChildren.Add(nodeIndex, new int[0] { });
			}

			private static List<Vector3> SkipNodesBeforeMedian(IList<Vector3> nodes, int median)
			{
				return nodes.Skip(median + 1).ToList();
			}

			private static List<Vector3> TakeNodesBeforeMedian(IList<Vector3> nodes, int median)
			{
				return nodes.Take(median).ToList();
			}

			private void UpdateDepth(int depth)
			{
				Depth = depth > Depth ? depth : Depth;
			}

			private int GetMedian(IList<Vector3> nodes)
			{;
				return nodes.Count / 2;
			}

			private IList<Vector3> OrderNodesByCompareValue(int depth, IList<Vector3> nodes)
			{
				const int vectorDimension = 3;
				int vectorIndex = depth % vectorDimension;
				return nodes.OrderBy(v => v[vectorIndex]).ToList();
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