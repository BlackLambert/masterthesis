using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SBaier.Master
{
	/// <summary>
	/// Based on Yuksel C.: Sample Elimination for Generating Poisson Disk Sample Sets. 
	/// 2015. Computer Graphics Forum (Proceedings of EUROGRAPHICS 2015), 34, 2, 2015
	/// </summary>
	public class SampleEliminationSphereGenerator : MeshGenerator
	{
		private const float _minWeightPow = 1.5f;
		private const float _minWeightFactor = 0.65f;
		private const int _alpha = 8;
		private QuickSelector<Vector3> _quickSelector;

		public int TargetSampleCount { get; }
		public float BaseSamplesFactor { get; }
		public Seed Seed { get; }

		public SampleEliminationSphereGenerator(int targetSamples, 
			float baseSamplesFactor,
			Seed seed,
			QuickSelector<Vector3> quickSelector)
		{
			ValidateTargetSampes(targetSamples);
			ValidateBaseSamplesFactor(baseSamplesFactor);

			TargetSampleCount = targetSamples;
			BaseSamplesFactor = baseSamplesFactor;
			Seed = seed;
			_quickSelector = quickSelector;
		}

		private void ValidateTargetSampes(int targetSamples)
		{
			if (targetSamples <= 3)
				throw new ArgumentOutOfRangeException();
		}

		private void ValidateBaseSamplesFactor(float baseSamplesFactor)
		{
			if (baseSamplesFactor <= 1.0f)
				throw new ArgumentOutOfRangeException();
		}

		public void GenerateMeshFor(Mesh mesh)
		{
			GenerateMeshFor(mesh, 1);
		}

		public void GenerateMeshFor(Mesh mesh, float size)
		{
			ValidateSize(size);
			CreateVericesFor(mesh, size);
			Seed.Reset();
		}

		private void CreateVericesFor(Mesh mesh, float size)
		{
			Vector3[] vertices = CreateBaseSamples(size);
			Vector3BinaryKDTree tree = new Vector3BinaryKDTree(vertices, _quickSelector);
			List<Vector3> verticesList = new List<Vector3>();
			float weightRadius = CalculateWeightRadius(size);
			float weightRadiusMin = CalculateWeightRadiusMin(weightRadius);
			List<float> weights = WeightVertices(vertices, tree, weightRadius, weightRadiusMin);
			Heap<float> heap = new BinaryHeap<float>(weights, true);

			int verticesLeft = vertices.Length;
			for (int i = vertices.Length - 1; i >= 0; i--)
			{
				int removedIndex = heap.Pop();
				ReweightNeighbors(vertices, tree, heap, removedIndex, weightRadius, weightRadiusMin);
				if (verticesLeft <= TargetSampleCount)
					verticesList.Add(vertices[removedIndex]);
				verticesLeft--;
			}
			mesh.vertices = verticesList.ToArray();
		}

		private float CalculateWeightRadiusMin(float weightRadius)
		{
			float minWeightRadius =  weightRadius * (1 - Mathf.Pow(1 / BaseSamplesFactor, _minWeightPow)) * _minWeightFactor;
			return minWeightRadius;
		}

		private float CalculateWeightRadius(float size)
		{
			float sphereSurface = 4 * Mathf.PI * size;
			return Mathf.Sqrt(sphereSurface / (2 * Mathf.Sqrt(3) * TargetSampleCount));
		}

		private void ReweightNeighbors(Vector3[] vertices, Vector3BinaryKDTree tree, Heap<float> heap, int removedPointIndex, float weightRadius, float weightRadiusMin)
		{
			IList<int> neighbors = tree.GetNearestToWithin(removedPointIndex, weightRadius);
			foreach(int neighborIndex in neighbors)
			{
				Vector3 neighbor = vertices[neighborIndex];
				float weightDelta = CalculateWeight(neighbor, vertices[removedPointIndex], weightRadius, weightRadiusMin);
				if(!heap.HasElementBeenRemoved(neighborIndex))
					heap.ChangeElementAt(neighborIndex, heap.GetElementAt(neighborIndex) - weightDelta);
			}
		}

		private List<float> WeightVertices(Vector3[] vertices, Vector3BinaryKDTree tree, float weightRadius, float weightRadiusMin)
		{
			List<float> result = new List<float>();
			for(int i = 0; i < vertices.Length; i++)
			{
				IList<int> neighbors = tree.GetNearestToWithin(i, weightRadius);
				float weight = CalculateWeight(vertices, vertices[i], neighbors, weightRadius, weightRadiusMin);
				result.Add(weight);
			}
			return result;
		}

		private float CalculateWeight(IList<Vector3> vertices, Vector3 vertex, IList<int> neighbors, float weightRadius, float weightRadiusMin)
		{
			float result = 0;
			foreach(int neighborIndex in neighbors)
			{
				Vector3 neighbor = vertices[neighborIndex];
				float weight = CalculateWeight(neighbor, vertex, weightRadius, weightRadiusMin);
				result += weight;
			}
			return result;
		}

		private float CalculateWeight(Vector3 neighbor, Vector3 vertex, float weightRadius, float weightRadiusMin)
		{
			
			float doubleWeightRadius = 2 * weightRadius;
			float weight = Mathf.Pow(1 - Mathf.Min((neighbor - vertex).magnitude, doubleWeightRadius) / doubleWeightRadius, _alpha);
			if (weight > weightRadiusMin * 2)
				return weight;
			else
				return weightRadiusMin * 2;
		}

		private Vector3[] CreateBaseSamples(float size)
		{
			int baseSamplesCount = (int)(TargetSampleCount * BaseSamplesFactor);
			Vector3[] vertices = new Vector3[baseSamplesCount];
			for (int i = 0; i < baseSamplesCount; i++)
				vertices[i] = CreateRandomVertex() * size;
			return vertices;
		}

		private Vector3 CreateRandomVertex()
		{
			float ran0 = (float)Seed.Random.NextDouble();
			float ran1 = (float)Seed.Random.NextDouble();
			float angle0 = ran0 * 2 * Mathf.PI;
			float angle1 = Mathf.Acos(2 * ran1 - 1) - (Mathf.PI / 2);
			float x = Mathf.Cos(angle1) * Mathf.Cos(angle0);
			float y = Mathf.Cos(angle1) * Mathf.Sin(angle0);
			float z = Mathf.Sin(angle1);
			Vector3 result = new Vector3(x, y, z);
			if (result == Vector3.zero) 
				return CreateRandomVertex();
			return result;
		}

		private static void ValidateSize(float size)
		{
			if (size <= 0)
				throw new ArgumentOutOfRangeException();
		}
	}
}