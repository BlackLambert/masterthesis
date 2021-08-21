using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace SBaier.Master
{
    public class Vector3BinaryKDTree : KDTree<Vector3>
	{

		private const int _jobBatchSize = 16;

		public int Depth { get; private set; }

		public int Count => _nodes.Length;
		private int _root;
		private Vector3[] _nodes;
		private int[] _indexPermutation;
		private int[] _nodeChildren;
		private FindNearestElementsComputer _findNearestElementsComputer;

		public Vector3BinaryKDTree(Vector3[] nodes, int root, int[] nodeToChildren, int[] indexPermutation, int depth)
		{
			ValidateNodes(nodes);
			_nodes = nodes;
			_root = root;
			_nodeChildren = nodeToChildren;
			_indexPermutation = indexPermutation;
			Depth = depth;
			CreateComputers();
		}

		private void CreateComputers()
		{
			_findNearestElementsComputer = new FindNearestElementsComputer
			(
				_root,
				_nodes,
				_indexPermutation,
				_nodeChildren
			);
		}

		public int GetNearestTo(Vector3 sample)
		{
			return GetNearestTo(new Vector3[] { sample })[0];
		}

		public int GetNearestTo(int sampleIndex)
		{
			return GetNearestTo(new int[] { sampleIndex })[0];
		}

		public int[] GetNearestTo(Vector3[] samples)
		{
			FindNearestElementJob job = CreateFindNearestJob(samples, CreateEmptyIndicesToExclude(samples.Length));
			int[] r2 = FinishJob(job);
			return r2;
		}

		public int[] GetNearestTo(int[] sampleIndices)
		{
			FindNearestElementJob job = CreateFindNearestJob(FindSamples(sampleIndices), sampleIndices);
			return FinishJob(job);
		}

		private Vector3[] FindSamples(int[] sampleIndices)
		{
			Vector3[] result = new Vector3[sampleIndices.Length];
			for (int i = 0; i < result.Length; i++)
				result[i] = _nodes[sampleIndices[i]];
			return result;
		}

		private int[] CreateEmptyIndicesToExclude(int length)
		{
			int[] result = new int[length];
			for (int i = 0; i < result.Length; i++)
				result[i] = -1;
			return result;
		}

		private int[] FinishJob(FindNearestElementJob job)
		{
			job.Schedule(job.Result.Length, _jobBatchSize).Complete();
			int[] result = job.Result.ToArray();
			job.Dispose();
			return result;
		}

		private FindNearestElementJob CreateFindNearestJob(Vector3[] samples, int[] indicesToExclude)
		{
			NativeArray<int> result = new NativeArray<int>(samples.Length, Allocator.TempJob);
			NativeArray<Vector3> nodes = new NativeArray<Vector3>(_nodes, Allocator.TempJob);
			NativeArray<Vector3> samplesNative = new NativeArray<Vector3>(samples, Allocator.TempJob);
			NativeArray<int> indexPermutations = new NativeArray<int>(_indexPermutation, Allocator.TempJob);
			NativeArray<int> nodeChildren = new NativeArray<int>(_nodeChildren, Allocator.TempJob);
			NativeArray<int> indicesToExcludeNative = new NativeArray<int>(indicesToExclude, Allocator.TempJob);
			return new FindNearestElementJob
			(
				result: result,
				samples: samplesNative,
				root: _root,
				nodes: nodes,
				indexPermutation: indexPermutations,
				nodeChildren: nodeChildren,
				indicesToExclude: indicesToExcludeNative
			);
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

		public int[][] GetNearestToWithin(Vector3[] samples, float maxDistance)
		{
			ValidateMaxDistance(maxDistance);
			int[][] result = new int[samples.Length][];
			for (int i = 0; i < samples.Length; i++)
				result[i] = GetNearestToWithin(samples[i], maxDistance);
			return result;
			//FindNearestElementsJob[] jobs = CreateFindNearestElementsJobs(samples, CreateEmptyIndicesToExclude(samples.Length), maxDistance);
			//JobHandle[] handles = Start(jobs);
			//return FinishJob(jobs, handles);
		}

		public int[][] GetNearestToWithin(int[] sampleIndices, float maxDistance)
		{
			ValidateMaxDistance(maxDistance);
			int[][] result = new int[sampleIndices.Length][];
			for (int i = 0; i < sampleIndices.Length; i++)
				result[i] = GetNearestToWithin(sampleIndices[i], maxDistance);
			return result;
			//FindNearestElementsJob[] jobs = CreateFindNearestElementsJobs(FindSamples(sampleIndices), sampleIndices, maxDistance);
			//JobHandle[] handles = Start(jobs);
			//return FinishJob(jobs, handles);
		}

		private JobHandle[] Start(FindNearestElementsJob[] jobs)
		{
			JobHandle[] result = new JobHandle[jobs.Length];
			for (int i = 0; i < jobs.Length; i++)
				result[i] = jobs[i].Schedule();
			return result;
		}

		private int[][] FinishJob(FindNearestElementsJob[] jobs, JobHandle[] handles)
		{
			int[][] result = new int[jobs.Length][];
			for (int i = 0; i < jobs.Length; i++)
			{
				FindNearestElementsJob job = jobs[i];
				JobHandle handle = handles[i];
				handle.Complete();
				result[i] = job.Result.ToArray();
				job.Dispose();
			}
			if (jobs.Length > 0)
			{
				jobs[0].Nodes.Dispose();
				jobs[0].IndexPermutation.Dispose();
				jobs[0].NodeChildren.Dispose();
			}
			return result;
		}

		private FindNearestElementsJob[] CreateFindNearestElementsJobs(Vector3[] samples, int[] indicesToExclude, float maxDistance)
		{
			FindNearestElementsJob[] jobs = new FindNearestElementsJob[samples.Length];
			NativeArray<Vector3> nodes = new NativeArray<Vector3>(_nodes, Allocator.TempJob);
			NativeArray<int> indexPermutations = new NativeArray<int>(_indexPermutation, Allocator.TempJob);
			NativeArray<int> nodeChildren = new NativeArray<int>(_nodeChildren, Allocator.TempJob);
			for (int i = 0; i < samples.Length; i++)
				jobs[i] = CreateFindNearestElementsJobs(samples[i], indicesToExclude[i], nodes, indexPermutations, nodeChildren, maxDistance);
			return jobs;
		}

		private FindNearestElementsJob CreateFindNearestElementsJobs(Vector3 sample, 
			int indexToExclude, 
			NativeArray<Vector3> nodes, 
			NativeArray<int> indexPermutations, 
			NativeArray<int> nodeChildren, 
			float maxDistance)
		{
			NativeList<int> result = new NativeList<int>(1, Allocator.TempJob);
			return new FindNearestElementsJob
			(
				result: result,
				sample: sample,
				root: _root,
				nodes: nodes,
				indexPermutation: indexPermutations,
				nodeChildren: nodeChildren,
				indexToExclude: indexToExclude,
				maxDistance: maxDistance
			);
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
				int[] nodeToChildren) :
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
				bool nodeHasOneChild = HasOneChild(nodeIndex);

				if (nodeHasOneChild)
					GetNearestInChild(compareValueIndex, nodeIndex);
				else
					GetNearestInChildren(compareValueIndex, nodeIndex);
			}

			private void GetNearestInChild(int compareValueIndex, int nodeIndex)
			{
				int nextNodeIndex = _nodeToChildren[nodeIndex * 2];
				int nextCompareValueIndex = GetNextCompareValueIndex(compareValueIndex);
				GetNearestToWithinRecursive(nextNodeIndex, nextCompareValueIndex);
			}

			private void GetNearestInChildren(int compareValueIndex, int nodeIndex)
			{
				Vector3 node = GetNode(nodeIndex);
				int nextChildIndex = GetNextChildIndexOf(node, compareValueIndex); 
				int nextNodeIndex = _nodeToChildren[nodeIndex * 2 + nextChildIndex];
				int nextCompareValueIndex = GetNextCompareValueIndex(compareValueIndex);
				GetNearestToWithinRecursive(nextNodeIndex, nextCompareValueIndex);
				float compareValueDistance = CalculateCompareValueDistanceToSample(node, compareValueIndex);
				bool noOtherNodesCanBeInDistance = compareValueDistance > _maxDistance;
				if (noOtherNodesCanBeInDistance)
					return;
				int otherChildIndex = GetOtherChildIndex(nextChildIndex);
				GetNearestToWithinRecursive(_nodeToChildren[nodeIndex * 2 + otherChildIndex], nextCompareValueIndex);
			}
		}

		private abstract class Computer
		{
			private Vector3[] _nodes;
			private int[] _indexPermutation;
			private Vector3 _sample;
			private int _indexToExclude;
			protected int[] _nodeToChildren;

			private bool _hasIndexToExclude = false;

			public Computer(Vector3[] nodes,
				int[] indexPermutation,
				int[] nodeToChildren)
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

			protected bool IsNodeALeave(int nodeIndex)
			{
				return HasNoChild(nodeIndex);
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

			protected bool HasOneChild(int nodeIndex)
			{
				int child0 = _nodeToChildren[nodeIndex * 2];
				int child1 = _nodeToChildren[nodeIndex * 2 + 1];
				return child0 >= 0 && child1 < 0 ||
					child1 >= 0 && child0 < 0;
			}

			protected bool HasNoChild(int nodeIndex)
			{
				int child0 = _nodeToChildren[nodeIndex * 2];
				int child1 = _nodeToChildren[nodeIndex * 2 + 1];
				return child0 < 0 && child1 < 0;
			}
		}

		
	}
}