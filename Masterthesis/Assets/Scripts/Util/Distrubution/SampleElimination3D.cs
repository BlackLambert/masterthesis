using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
	/// <summary>
	/// Based on Yuksel C.: Sample Elimination for Generating Poisson Disk Sample Sets. 
	/// 2015. Computer Graphics Forum (Proceedings of EUROGRAPHICS 2015), 34, 2, 2015
	/// </summary>
	public class SampleElimination3D
	{
		private const float _minWeightPow = 1.5f;
		private const float _minWeightFactor = 0.65f;
		private const int _alpha = 8;
		private QuickSelector<Vector3> _quickSelector;

		public SampleElimination3D(QuickSelector<Vector3> quickSelector)
		{
			_quickSelector = quickSelector;
		}

		public IList<Vector3> Eliminate(IList<Vector3> points, int targetAmount, float sampleArea)
		{
			Vector3BinaryKDTree tree = new Vector3BinaryKDTree(points, _quickSelector);
			List<Vector3> verticesList = new List<Vector3>();
			float baseSamplesFactor = (float)targetAmount / (float)points.Count;
			float weightRadius = CalculateWeightRadius(targetAmount, sampleArea);
			float weightRadiusMin = CalculateWeightRadiusMin(weightRadius, baseSamplesFactor);
			List<float> weights = WeightVertices(points, tree, weightRadius, weightRadiusMin);
			Heap<float> heap = new BinaryHeap<float>(weights, true);

			int verticesLeft = points.Count;
			for (int i = points.Count - 1; i >= 0; i--)
			{
				int removedIndex = heap.Pop();
				ReweightNeighbors(points, tree, heap, removedIndex, weightRadius, weightRadiusMin);
				if (verticesLeft <= targetAmount)
					verticesList.Add(points[removedIndex]);
				verticesLeft--;
			}
			return verticesList;
		}

		private float CalculateWeightRadius(int targetAmount, float sampleArea)
		{
			return Mathf.Sqrt(sampleArea / (2 * Mathf.Sqrt(3) * targetAmount));
		}

		private float CalculateWeightRadiusMin(float weightRadius, float baseSamplesFactor)
		{
			float minWeightRadius = weightRadius * (1 - Mathf.Pow(1 / baseSamplesFactor, _minWeightPow)) * _minWeightFactor;
			return minWeightRadius;
		}

		private void ReweightNeighbors(IList<Vector3> vertices, Vector3BinaryKDTree tree, Heap<float> heap, int removedPointIndex, float weightRadius, float weightRadiusMin)
		{
			IList<int> neighbors = tree.GetNearestToWithin(removedPointIndex, weightRadius);
			foreach (int neighborIndex in neighbors)
			{
				Vector3 neighbor = vertices[neighborIndex];
				float weightDelta = CalculateWeight(neighbor, vertices[removedPointIndex], weightRadius, weightRadiusMin);
				if (!heap.HasElementBeenRemoved(neighborIndex))
					heap.ChangeElementAt(neighborIndex, heap.GetElementAt(neighborIndex) - weightDelta);
			}
		}

		private List<float> WeightVertices(IList<Vector3> vertices, Vector3BinaryKDTree tree, float weightRadius, float weightRadiusMin)
		{
			List<float> result = new List<float>();
			for (int i = 0; i < vertices.Count; i++)
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
			foreach (int neighborIndex in neighbors)
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
	}
}