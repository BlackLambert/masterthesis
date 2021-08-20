using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SBaier.Master
{
    public class Vector3BinaryKDTree : KDTree<Vector3>
	{
		public int Depth { get; private set; }

		public int Count => _nodes.Length;

		private int _root;
		private Vector3[] _nodes;
		private int[] _indexPermutation;
		private int[][] _nodeToChildren;
		private FindNearestElementComputer _findNearestElementComputer;
		private FindNearestElementsComputer _findNearestElementsComputer;

		public Vector3BinaryKDTree(Vector3[] nodes, int root, int[][] nodeToChildren, int[] indexPermutation, int depth)
		{
			ValidateNodes(nodes);
			_nodes = nodes;
			_root = root;
			_nodeToChildren = nodeToChildren;
			_indexPermutation = indexPermutation;
			Depth = depth;
			CreateComputers();
		}

		private void CreateComputers()
		{
			_findNearestElementComputer = new FindNearestElementComputer
			(
				_root,
				_nodes,
				_indexPermutation,
				_nodeToChildren
			);
			_findNearestElementsComputer = new FindNearestElementsComputer
			(
				_root,
				_nodes,
				_indexPermutation,
				_nodeToChildren
			);
		}

		public int GetNearestTo(Vector3 sample)
		{
			_findNearestElementComputer.Init(sample, -1);
			return _findNearestElementComputer.Compute();
		}

		public int GetNearestTo(int sampleIndex)
		{
			_findNearestElementComputer.Init(_nodes[sampleIndex], sampleIndex);
			return _findNearestElementComputer.Compute();
		}

		public int[] GetNearestToWithin(Vector3 sample, float maxDistance)
		{
			ValidateMaxDistance(maxDistance);
			_findNearestElementsComputer.Init(sample, -1, maxDistance);
			return _findNearestElementsComputer.Compute();
		}

		public int[] GetNearestToWithin(int sampleIndex, float maxDistance)
		{
			ValidateMaxDistance(maxDistance);
			_findNearestElementsComputer.Init(_nodes[sampleIndex], sampleIndex, maxDistance);
			return _findNearestElementsComputer.Compute();
		}

		private void ValidateNodes(Vector3[] vectors)
		{
			if (vectors.Length == 0)
				throw new ArgumentException();
		}

		private static void ValidateMaxDistance(float maxDistance)
		{
			if (maxDistance <= 0)
				throw new ArgumentOutOfRangeException();
		}

		private class FindNearestElementComputer : Computer
		{
			private Vector3 _sample;
			private int _root;

			public FindNearestElementComputer(
				int root,
				Vector3[] nodes,
				int[] indexPermutation,
				int[][] nodeToChildren) : 
				base(nodes, indexPermutation, nodeToChildren)
			{
				_root = root;
			}

			public new void Init(
				Vector3 sample,
				int indexToExclude)
			{
				base.Init(sample, indexToExclude);
				_sample = sample;
			}

			public int Compute()
			{
				return GetOriginalNodeIndexOf(GetNearestToRecursive(_root));
			}

			private int GetNearestToRecursive(int nodeIndex, int compareValueIndex = 0)
			{
				int[] children = GetChildrenOf(nodeIndex);
				bool nodeIsALeave = HasNoChild(children);
				if (nodeIsALeave)
					return GetNearestOfLeave(nodeIndex);
				else
					return GetNearestOfBranch(nodeIndex, compareValueIndex);
			}

			private int GetNearestOfLeave(int nodeIndex)
			{
				bool nodeIsExcluded = IsNodeToExclude(nodeIndex);
				return nodeIsExcluded ? -1 : nodeIndex;
			}

			private int GetNearestOfBranch(int nodeIndex, int compareValueIndex = 0)
			{
				int[] children = GetChildrenOf(nodeIndex);
				bool nodeHasOneChild = HasOneChild(children);
				if (nodeHasOneChild)
					return GetNearestOfOneChild(nodeIndex, compareValueIndex);
				else
					return GetNearestOfTwoChildren(nodeIndex, compareValueIndex);
				
			}

			private int GetNearestOfOneChild(int nodeIndex, int compareValueIndex)
			{
				bool nodeIsExcluded = IsNodeToExclude(nodeIndex);

				if (!nodeIsExcluded)
					return GetNearestOfChildAndNode(nodeIndex, compareValueIndex);

				int[] children = GetChildrenOf(nodeIndex);
				int nextCompareValueIndex = GetNextCompareValueIndex(compareValueIndex);
				return GetNearestToRecursive(children[0], nextCompareValueIndex);
			}

			private int GetNearestOfChildAndNode(int nodeIndex, int compareValueIndex)
			{
				int[] children = GetChildrenOf(nodeIndex);
				int childIndex = children[0];
				int nextCompareValueIndex = GetNextCompareValueIndex(compareValueIndex);
				int childNearest = GetNearestToRecursive(childIndex, nextCompareValueIndex);

				if (childNearest < 0)
					return nodeIndex;

				Vector3 child = GetNode(childNearest);
				Vector3 currentNode = GetNode(nodeIndex);
				float nodeDistanceToSampleSqr = (currentNode - _sample).sqrMagnitude;
				float childSmallestDistanceToSampleSqr = (child - _sample).sqrMagnitude;
				bool nodeDistanceSmaller = nodeDistanceToSampleSqr < childSmallestDistanceToSampleSqr;
				return nodeDistanceSmaller ? nodeIndex : childNearest;
			}

			private int GetNearestOfTwoChildren(int nodeIndex, int compareValueIndex)
			{
				bool nodeIsExcluded = IsNodeToExclude(nodeIndex);

				if (nodeIsExcluded)
					return GetNearestOfTwoChildrenParentExcluded(nodeIndex, compareValueIndex);
				else
					return GetNearestOfTwoChildrenAndParent(nodeIndex, compareValueIndex);
			}

			private int GetNearestOfTwoChildrenParentExcluded(int nodeIndex, int compareValueIndex)
			{
				int[] children = GetChildrenOf(nodeIndex);
				int nextCompareValueIndex = GetNextCompareValueIndex(compareValueIndex);
				int child0Nearest = GetNearestToRecursive(children[0], nextCompareValueIndex);
				int child1Nearest = GetNearestToRecursive(children[1], nextCompareValueIndex);
				if (child0Nearest < 0)
					return child1Nearest;
				else if (child1Nearest < 0)
					return child0Nearest;
				else
					return GetSmaller(child0Nearest, child1Nearest);
			}

			private int GetNearestOfTwoChildrenAndParent(int nodeIndex, int compareValueIndex)
			{
				Vector3 node = GetNode(nodeIndex);
				int nextChildIndex = GetNextChildIndexOf(node, compareValueIndex);
				int otherChildIndex = GetOtherChildIndex(nextChildIndex);
				int[] children = GetChildrenOf(nodeIndex);
				int nextNodeIndex = children[nextChildIndex];
				int nextCompareValueIndex = GetNextCompareValueIndex(compareValueIndex);
				int child0Smallest = GetNearestToRecursive(nextNodeIndex, nextCompareValueIndex);
				int currentSmallest = nodeIndex;
				float nodeDistanceToSampleSqr = node.FastSubstract(_sample).sqrMagnitude;
				float currentSmallestDistanceToSample = Mathf.Sqrt(nodeDistanceToSampleSqr);
				if (child0Smallest >= 0)
				{
					Vector3 nodeChild0 = GetNode(child0Smallest);
					float childSmallestDistanceToSampleSqr = nodeChild0.FastSubstract(_sample).sqrMagnitude;
					bool nodeDistanceSmaller = nodeDistanceToSampleSqr < childSmallestDistanceToSampleSqr;
					currentSmallest = nodeDistanceSmaller ? nodeIndex : child0Smallest;
					currentSmallestDistanceToSample = nodeDistanceSmaller ?
						currentSmallestDistanceToSample :
						Mathf.Sqrt(childSmallestDistanceToSampleSqr);
				}

				float compareValueDistance = CalculateCompareValueDistanceToSample(node, compareValueIndex);
				bool currentIsSmallestOfThisBranch = currentSmallestDistanceToSample <= compareValueDistance;
				if (currentIsSmallestOfThisBranch)
					return currentSmallest;

				int child1Smallest = GetNearestToRecursive(children[otherChildIndex], nextCompareValueIndex);
				if (child1Smallest < 0)
					return currentSmallest;
				Vector3 nodeChild1 = GetNode(child1Smallest);
				float child1SmallestDistanceToSample = nodeChild1.FastSubstract(_sample).magnitude;
				currentSmallest = child1SmallestDistanceToSample < currentSmallestDistanceToSample ? child1Smallest : currentSmallest;
				return currentSmallest;
			}

		}
		private class FindNearestElementsComputer : Computer
		{
			private Vector3 _sample;
			private int _root;
			private float _maxDistance;
			private int _resultCount;
			private int[] _resultBuffer;

			public FindNearestElementsComputer(
				int root,
				Vector3[] nodes,
				int[] indexPermutation,
				int[][] nodeToChildren) :
				base(nodes, indexPermutation, nodeToChildren)
			{
				_root = root;
				_resultBuffer = new int[nodes.Length];
			}

			public void Init( 
				Vector3 sample,
				int indexToExclude,
				float maxDistance)
			{
				base.Init(sample, indexToExclude);
				_sample = sample;
				_maxDistance = maxDistance;
				_resultCount = 0;
			}

			public int[] Compute()
			{
				GetNearestToWithinRecursive(_root);
				return ExtractResult();
			}

			private int[] ExtractResult()
			{
				int[] result = new int[_resultCount];
				for (int i = 0; i < _resultCount; i++)
					result[i] = _resultBuffer[i];
				return result;
			}

			private void GetNearestToWithinRecursive(int nodeIndex, int compareValueIndex = 0)
			{
				if (ShallAddNode(nodeIndex))
					AddNodeTo(nodeIndex);
				if (!IsNodeALeave(nodeIndex))
					GetNearestInBranch(compareValueIndex, nodeIndex);
			}

			private bool ShallAddNode(int nodeIndex)
			{
				Vector3 node = GetNode(nodeIndex);
				float nodeDistanceToSample = node.FastSubstract(_sample).magnitude;
				bool nodeIsWithinMaxDistance = nodeDistanceToSample <= _maxDistance;
				bool isNodeExcluded = IsNodeToExclude(nodeIndex);
				return nodeIsWithinMaxDistance && !isNodeExcluded;
			}

			private void AddNodeTo(int internalNodeIndex)
			{
				int originalNodeIndex = GetOriginalNodeIndexOf(internalNodeIndex);
				_resultBuffer[_resultCount] = originalNodeIndex;
				_resultCount++;
			}

			private void GetNearestInBranch(int compareValueIndex, int nodeIndex)
			{
				int[] children = GetChildrenOf(nodeIndex);
				bool nodeHasOneChild = HasOneChild(children);

				if (nodeHasOneChild)
					GetNearestInChild(compareValueIndex, nodeIndex);
				else
					GetNearestInChildren(compareValueIndex, nodeIndex);
			}

			private void GetNearestInChild(int compareValueIndex, int nodeIndex)
			{
				int[] children = GetChildrenOf(nodeIndex);
				int nextNodeIndex = children[0];
				int nextCompareValueIndex = GetNextCompareValueIndex(compareValueIndex);
				GetNearestToWithinRecursive(nextNodeIndex, nextCompareValueIndex);
			}

			private void GetNearestInChildren(int compareValueIndex, int nodeIndex)
			{
				int[] children = GetChildrenOf(nodeIndex);
				Vector3 node = GetNode(nodeIndex);
				int nextChildIndex = GetNextChildIndexOf(node, compareValueIndex); 
				int nextNodeIndex = children[nextChildIndex];
				int nextCompareValueIndex = GetNextCompareValueIndex(compareValueIndex);
				GetNearestToWithinRecursive(nextNodeIndex, nextCompareValueIndex);
				float compareValueDistance = CalculateCompareValueDistanceToSample(node, compareValueIndex);
				bool noOtherNodesCanBeInDistance = compareValueDistance > _maxDistance;
				if (noOtherNodesCanBeInDistance)
					return;
				int otherChildIndex = GetOtherChildIndex(nextChildIndex);
				GetNearestToWithinRecursive(children[otherChildIndex], nextCompareValueIndex);
			}
		}

		private abstract class Computer
		{
			private Vector3[] _nodes;
			private int[] _indexPermutation;
			private Vector3 _sample;
			private int _indexToExclude;
			private int[][] _nodeToChildren;

			private bool _hasIndexToExclude = false;

			public Computer(Vector3[] nodes,
				int[] indexPermutation,
				int[][] nodeToChildren)
			{
				_nodes = nodes; 
				_indexPermutation = indexPermutation;
				_nodeToChildren = nodeToChildren;
			}

			public void Init(
				Vector3 sample,
				int indexToExclude)
			{
				_sample = sample;
				_indexToExclude = indexToExclude;
				_hasIndexToExclude = _indexToExclude >= 0;
			}

			protected int GetNextChildIndex(float sampleCompareValue, float nodeCompareValue)
			{
				int nextChildIndex = sampleCompareValue < nodeCompareValue ? 0 : 1;
				return nextChildIndex;
			}

			protected int GetNextChildIndexOf(Vector3 node, int compareValueIndex)
			{
				float sampleCompareValue = _sample[compareValueIndex];
				float nodeCompareValue = node[compareValueIndex];
				return GetNextChildIndex(sampleCompareValue, nodeCompareValue);
			}

			protected Vector3 GetNode(int internalIndex)
			{
				return _nodes[_indexPermutation[internalIndex]];
			}

			protected int GetOriginalNodeIndexOf(int internalIndex)
			{
				return _indexPermutation[internalIndex];
			}

			protected int GetNextCompareValueIndex(int compareValueIndex)
			{
				return (compareValueIndex + 1) % 3;
			}

			protected int GetSmaller(int index0, int index1)
			{
				float node0DistanceSqr = _nodes[_indexPermutation[index0]].FastSubstract(_sample).sqrMagnitude;
				float node1DistanceSqr = _nodes[_indexPermutation[index1]].FastSubstract(_sample).sqrMagnitude;
				return node0DistanceSqr < node1DistanceSqr ? index0 : index1;
			}

			protected bool IsNodeToExclude(int nodeIndex)
			{
				return _hasIndexToExclude && GetOriginalNodeIndexOf(nodeIndex) == _indexToExclude;
			}

			protected int[] GetChildrenOf(int nodeIndex)
			{
				return _nodeToChildren[nodeIndex];
			}

			protected bool IsNodeALeave(int nodeIndex)
			{
				int[] children = GetChildrenOf(nodeIndex);
				bool nodeIsALeave = HasNoChild(children);
				return nodeIsALeave;
			}

			protected float CalculateCompareValueDistanceToSample(Vector3 node, int compareValueIndex)
			{
				float nodeCompareValue = GetCompareValue(node, compareValueIndex);
				float sampleCompareValue = GetCompareValue(_sample, compareValueIndex);
				return CalculateCompareValueDistanceOf(nodeCompareValue, sampleCompareValue);
			}

			private float GetCompareValue(Vector3 node, int compareValueIndex)
			{
				switch(compareValueIndex)
				{
					case 0:
						return node.x;
					case 1:
						return node.y;
					case 2:
						return node.z;
				}
				return node[compareValueIndex];
			}

			protected float CalculateCompareValueDistanceOf(float compareValue1, float compareValue2)
			{
				float compareValueDistance = Mathf.Abs(compareValue1 - compareValue2);
				return compareValueDistance;
			}

			protected int GetOtherChildIndex(int nextChildIndex)
			{
				return (nextChildIndex + 1) % 2;
			}

			protected bool HasOneChild(int[] children)
			{
				return children.Length == 1;
			}

			protected bool HasNoChild(int[] children)
			{
				return children.Length == 0;
			}
		}

		
	}
}