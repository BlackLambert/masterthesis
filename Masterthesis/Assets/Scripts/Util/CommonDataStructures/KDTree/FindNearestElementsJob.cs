using System;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace SBaier.Master
{
	//[Unity.Burst.BurstCompile]
	public struct FindNearestElementsJob : IJob, IDisposable
    {
		[WriteOnly]
		private NativeList<int> _result;
		public NativeList<int> Result => _result;
		[ReadOnly]
		private Vector3 _sample;
        private int _root;
		[ReadOnly]
		private NativeArray<Vector3> _nodes;
		public NativeArray<Vector3> Nodes => _nodes;
		[ReadOnly]
		private NativeArray<int> _indexPermutation;
		public NativeArray<int> IndexPermutation => _indexPermutation;
		[ReadOnly]
		private NativeArray<int> _nodeChildren;
		public NativeArray<int> NodeChildren => _nodeChildren;
		private int _indexToExclude;
		private float _maxDistance;

		public FindNearestElementsJob(NativeList<int> result,
			Vector3 sample,
            int root,
            NativeArray<Vector3> nodes,
            NativeArray<int> indexPermutation,
            NativeArray<int> nodeChildren,
			int indexToExclude,
			float maxDistance)
		{
			_result = result;
			_sample = sample;
            _root = root;
            _nodes = nodes;
            _indexPermutation = indexPermutation;
            _nodeChildren = nodeChildren;
			_indexToExclude = indexToExclude;
			_maxDistance = maxDistance;
		}

		public void Dispose()
		{
			_result.Dispose();
		}

		public void Execute()
		{
			Vector3 sample = _sample;
			int indexToExclude = _indexToExclude;
			GetNearestToWithinRecursive(_root, sample, indexToExclude);
		}

		private void GetNearestToWithinRecursive(int nodeIndex, Vector3 sample, int indexToExclude, int compareValueIndex = 0)
		{
			if (ShallAddNode(nodeIndex, sample, indexToExclude))
				AddNodeTo(nodeIndex);
			if (!HasNoChild(nodeIndex))
				GetNearestInBranch(nodeIndex, sample, indexToExclude, compareValueIndex);
		}

		private bool ShallAddNode(int nodeIndex, Vector3 sample, int indexToExclude)
		{
			Vector3 node = GetNode(nodeIndex);
			float nodeDistanceToSample = node.FastSubstract(sample).magnitude;
			bool nodeIsWithinMaxDistance = nodeDistanceToSample <= _maxDistance;
			bool isNodeExcluded = IsNodeToExclude(nodeIndex, indexToExclude);
			return nodeIsWithinMaxDistance && !isNodeExcluded;
		}

		private void AddNodeTo(int internalNodeIndex)
		{
			int originalNodeIndex = GetOriginalNodeIndexOf(internalNodeIndex);
			_result.Add(originalNodeIndex);
		}

		private void GetNearestInBranch(int nodeIndex, Vector3 sample, int indexToExclude, int compareValueIndex)
		{
			bool nodeHasOneChild = HasOneChild(nodeIndex);

			if (nodeHasOneChild)
				GetNearestInChild(nodeIndex, sample, indexToExclude, compareValueIndex);
			else
				GetNearestInChildren(nodeIndex, sample, indexToExclude, compareValueIndex);
		}

		private void GetNearestInChild(int nodeIndex, Vector3 sample, int indexToExclude, int compareValueIndex)
		{
			int nextNodeIndex = _nodeChildren[nodeIndex * 2];
			int nextCompareValueIndex = GetNextCompareValueIndex(compareValueIndex);
			GetNearestToWithinRecursive(nextNodeIndex, sample, indexToExclude, nextCompareValueIndex);
		}

		private void GetNearestInChildren(int nodeIndex, Vector3 sample, int indexToExclude, int compareValueIndex)
		{
			Vector3 node = GetNode(nodeIndex);
			int nextChildIndex = GetNextChildIndexOf(node, sample, compareValueIndex);
			int nextNodeIndex = _nodeChildren[nodeIndex * 2 + nextChildIndex];
			int nextCompareValueIndex = GetNextCompareValueIndex(compareValueIndex);
			GetNearestToWithinRecursive(nextNodeIndex, sample, indexToExclude, nextCompareValueIndex);
			float compareValueDistance = CalculateCompareValueDistanceToSample(node, sample, compareValueIndex);
			bool noOtherNodesCanBeInDistance = compareValueDistance > _maxDistance;
			if (noOtherNodesCanBeInDistance)
				return;
			int otherChildIndex = GetOtherChildIndex(nextChildIndex);
			GetNearestToWithinRecursive(_nodeChildren[nodeIndex * 2 + otherChildIndex], sample, indexToExclude, nextCompareValueIndex);
		}


		private int GetOriginalNodeIndexOf(int internalIndex)
		{
			return _indexPermutation[internalIndex];
		}

		private bool HasOneChild(int nodeIndex)
		{
			int child0 = _nodeChildren[nodeIndex * 2];
			int child1 = _nodeChildren[nodeIndex * 2 + 1];
			return child0 >= 0 && child1 < 0 ||
				child1 >= 0 && child0 < 0;
		}

		private bool HasNoChild(int nodeIndex)
		{
			int child0 = _nodeChildren[nodeIndex * 2];
			int child1 = _nodeChildren[nodeIndex * 2 + 1];
			return child0 < 0 && child1 < 0;
		}

		private bool IsNodeToExclude(int nodeIndex, int nodeToExclude)
		{
			return nodeToExclude >= 0 && GetOriginalNodeIndexOf(nodeIndex) == nodeToExclude;
		}

		private int GetNextCompareValueIndex(int compareValueIndex)
		{
			return (compareValueIndex + 1) % 3;
		}

		private Vector3 GetNode(int internalIndex)
		{
			return _nodes[_indexPermutation[internalIndex]];
		}

		private int GetNextChildIndex(float sampleCompareValue, float nodeCompareValue)
		{
			int nextChildIndex = sampleCompareValue < nodeCompareValue ? 0 : 1;
			return nextChildIndex;
		}

		private int GetNextChildIndexOf(Vector3 node, Vector3 sample, int compareValueIndex)
		{
			float nodeCompareValue = GetCompareValue(node, compareValueIndex);
			float sampleCompareValue = GetCompareValue(sample, compareValueIndex);
			return GetNextChildIndex(sampleCompareValue, nodeCompareValue);
		}

		private float CalculateCompareValueDistanceToSample(Vector3 node, Vector3 sample, int compareValueIndex)
		{
			float nodeCompareValue = GetCompareValue(node, compareValueIndex);
			float sampleCompareValue = GetCompareValue(sample, compareValueIndex);
			return CalculateCompareValueDistanceOf(nodeCompareValue, sampleCompareValue);
		}

		private float GetCompareValue(Vector3 node, int compareValueIndex)
		{
			switch (compareValueIndex)
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

		private float CalculateCompareValueDistanceOf(float compareValue1, float compareValue2)
		{
			float compareValueDistance = Mathf.Abs(compareValue1 - compareValue2);
			return compareValueDistance;
		}

		private int GetOtherChildIndex(int nextChildIndex)
		{
			return (nextChildIndex + 1) % 2;
		}
	}
}