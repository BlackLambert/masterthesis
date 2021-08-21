using System;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace SBaier.Master
{
	[Unity.Burst.BurstCompile]
	public struct FindNearestElementJob : IJobParallelFor, IDisposable
    {
		[WriteOnly]
		private NativeArray<int> _result;
		public NativeArray<int> Result => _result;
		[ReadOnly]
		private NativeArray<Vector3> _samples;
        private int _root;
		[ReadOnly]
		private NativeArray<Vector3> _nodes;
		[ReadOnly]
		private NativeArray<int> _indexPermutation;
		[ReadOnly]
		private NativeArray<int> _nodeChildren;
		private NativeArray<int> _indicesToExclude;

        public FindNearestElementJob(NativeArray<int> result,
            NativeArray<Vector3> samples,
            int root,
            NativeArray<Vector3> nodes,
            NativeArray<int> indexPermutation,
            NativeArray<int> nodeChildren,
			NativeArray<int> indicesToExclude)
		{
			_result = result;
            _samples = samples;
            _root = root;
            _nodes = nodes;
            _indexPermutation = indexPermutation;
            _nodeChildren = nodeChildren;
			_indicesToExclude = indicesToExclude;
		}

		public void Dispose()
		{
            Result.Dispose();
            _samples.Dispose();
            _nodes.Dispose();
            _indexPermutation.Dispose();
            _nodeChildren.Dispose();
			_indicesToExclude.Dispose();
		}

		public void Execute(int index)
		{
			Vector3 sample = _samples[index];
			int indexToExclude = _indicesToExclude[index];
			_result[index] = GetOriginalNodeIndexOf(GetNearestToRecursive(_root, sample, indexToExclude));
		}

		private int GetOriginalNodeIndexOf(int internalIndex)
		{
			return _indexPermutation[internalIndex];
		}

		private int GetNearestToRecursive(int nodeIndex, Vector3 sample, int indexToExclude, int compareValueIndex = 0)
		{
			bool nodeIsALeave = HasNoChild(nodeIndex);
			if (nodeIsALeave)
				return GetNearestOfLeave(nodeIndex, indexToExclude);
			else
				return GetNearestOfBranch(nodeIndex, sample, indexToExclude, compareValueIndex);
		}

		private int GetNearestOfLeave(int nodeIndex, int indexToExclude)
		{
			bool nodeIsExcluded = IsNodeToExclude(nodeIndex, indexToExclude);
			return nodeIsExcluded ? -1 : nodeIndex;
		}

		private int GetNearestOfBranch(int nodeIndex, Vector3 sample, int indexToExclude, int compareValueIndex)
		{
			bool nodeHasOneChild = HasOneChild(nodeIndex);
			if (nodeHasOneChild)
				return GetNearestOfOneChild(nodeIndex, sample, indexToExclude, compareValueIndex);
			else
				return GetNearestOfTwoChildren(nodeIndex, sample, indexToExclude, compareValueIndex);
		}

		private int GetNearestOfOneChild(int nodeIndex, Vector3 sample, int indexToExclude, int compareValueIndex)
		{
			bool nodeIsExcluded = IsNodeToExclude(nodeIndex, indexToExclude);
			if (!nodeIsExcluded)
				return GetNearestOfChildAndNode(nodeIndex, sample, indexToExclude, compareValueIndex);
			int nextCompareValueIndex = GetNextCompareValueIndex(compareValueIndex);
			return GetNearestToRecursive(_nodeChildren[nodeIndex * 2], sample, nextCompareValueIndex);
		}

		private int GetNearestOfChildAndNode(int nodeIndex, Vector3 sample, int indexToExclude, int compareValueIndex)
		{
			int childIndex = _nodeChildren[nodeIndex * 2];
			int nextCompareValueIndex = GetNextCompareValueIndex(compareValueIndex);
			int childNearest = GetNearestToRecursive(childIndex, sample, indexToExclude, nextCompareValueIndex);

			if (childNearest < 0)
				return nodeIndex;

			Vector3 child = GetNode(childNearest);
			Vector3 currentNode = GetNode(nodeIndex);
			float nodeDistanceToSampleSqr = (currentNode.FastSubstract(sample)).sqrMagnitude;
			float childSmallestDistanceToSampleSqr = (child.FastSubstract(sample)).sqrMagnitude;
			bool nodeDistanceSmaller = nodeDistanceToSampleSqr < childSmallestDistanceToSampleSqr;
			return nodeDistanceSmaller ? nodeIndex : childNearest;
		}

		private int GetNearestOfTwoChildren(int nodeIndex, Vector3 sample, int indexToExclude, int compareValueIndex)
		{
			bool nodeIsExcluded = IsNodeToExclude(nodeIndex, indexToExclude);

			if (nodeIsExcluded)
				return GetNearestOfTwoChildrenParentExcluded(nodeIndex, sample, indexToExclude, compareValueIndex);
			else
				return GetNearestOfTwoChildrenAndParent(nodeIndex, sample, indexToExclude, compareValueIndex);
		}

		private int GetNearestOfTwoChildrenParentExcluded(int nodeIndex, Vector3 sample, int indexToExclude, int compareValueIndex)
		{
			int nextCompareValueIndex = GetNextCompareValueIndex(compareValueIndex);
			int child0Nearest = GetNearestToRecursive(_nodeChildren[nodeIndex * 2], sample, indexToExclude, nextCompareValueIndex);
			int child1Nearest = GetNearestToRecursive(_nodeChildren[nodeIndex * 2 + 1], sample, indexToExclude, nextCompareValueIndex);
			if (child0Nearest < 0)
				return child1Nearest;
			else if (child1Nearest < 0)
				return child0Nearest;
			else
				return GetSmaller(child0Nearest, child1Nearest, sample);
		}

		private int GetNearestOfTwoChildrenAndParent(int nodeIndex, Vector3 sample, int indexToExclude, int compareValueIndex)
		{
			Vector3 node = GetNode(nodeIndex);
			int nextChildIndex = GetNextChildIndexOf(node, sample, compareValueIndex);
			int nextNodeIndex = _nodeChildren[nodeIndex * 2 + nextChildIndex];
			int nextCompareValueIndex = GetNextCompareValueIndex(compareValueIndex);
			int child0Smallest = GetNearestToRecursive(nextNodeIndex, sample, indexToExclude, nextCompareValueIndex);
			int currentSmallest = nodeIndex;
			float nodeDistanceToSampleSqr = node.FastSubstract(sample).sqrMagnitude;
			float currentSmallestDistanceToSample = Mathf.Sqrt(nodeDistanceToSampleSqr);
			if (child0Smallest >= 0)
			{
				Vector3 nodeChild0 = GetNode(child0Smallest);
				float childSmallestDistanceToSampleSqr = nodeChild0.FastSubstract(sample).sqrMagnitude;
				bool nodeDistanceSmaller = nodeDistanceToSampleSqr < childSmallestDistanceToSampleSqr;
				currentSmallest = nodeDistanceSmaller ? nodeIndex : child0Smallest;
				currentSmallestDistanceToSample = nodeDistanceSmaller ?
					currentSmallestDistanceToSample :
					Mathf.Sqrt(childSmallestDistanceToSampleSqr);
			}

			float compareValueDistance = CalculateCompareValueDistanceToSample(node, sample, compareValueIndex);
			bool currentIsSmallestOfThisBranch = currentSmallestDistanceToSample <= compareValueDistance;
			if (currentIsSmallestOfThisBranch)
				return currentSmallest;

			int otherChildIndex = GetOtherChildIndex(nextChildIndex);
			int otherNodeIndex = _nodeChildren[nodeIndex * 2 + otherChildIndex];
			int child1Smallest = GetNearestToRecursive(otherNodeIndex, sample, indexToExclude, nextCompareValueIndex);
			if (child1Smallest < 0)
				return currentSmallest;
			Vector3 nodeChild1 = GetNode(child1Smallest);
			float child1SmallestDistanceToSample = nodeChild1.FastSubstract(sample).magnitude;
			currentSmallest = child1SmallestDistanceToSample < currentSmallestDistanceToSample ? child1Smallest : currentSmallest;
			return currentSmallest;
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

		private int GetSmaller(int index0, int index1, Vector3 sample)
		{
			float node0DistanceSqr = _nodes[_indexPermutation[index0]].FastSubstract(sample).sqrMagnitude;
			float node1DistanceSqr = _nodes[_indexPermutation[index1]].FastSubstract(sample).sqrMagnitude;
			return node0DistanceSqr < node1DistanceSqr ? index0 : index1;
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