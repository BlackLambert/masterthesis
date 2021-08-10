using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SBaier.Master
{
	public class SampleEliminationSphereGenerator : MeshGenerator
	{
		private RandomPointsOnSphereGenerator _randomPointsGenerator;
		private SampleElimination3D _sampleElimination;
		private QuickSelector<Vector3> _quickSelector;

		public int TargetSampleCount { get; }
		public float BaseSamplesFactor { get; }

		public SampleEliminationSphereGenerator(int targetSamples, 
			float baseSamplesFactor,
			SampleElimination3D sampleElimination,
			RandomPointsOnSphereGenerator randomPointsGenerator,
			QuickSelector<Vector3> quickSelector)
		{
			ValidateTargetSampes(targetSamples);
			ValidateBaseSamplesFactor(baseSamplesFactor);

			TargetSampleCount = targetSamples;
			BaseSamplesFactor = baseSamplesFactor;
			_sampleElimination = sampleElimination;
			_randomPointsGenerator = randomPointsGenerator;
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
			CreateTrianglesFor(mesh, size);
			_randomPointsGenerator.Reset();
		}

		private void CreateVericesFor(Mesh mesh, float size)
		{
			Vector3[] vertices = CreateBaseSamples(size);
			float sphereSurface = 4 * Mathf.PI * size;
			mesh.vertices = _sampleElimination.Eliminate(vertices, TargetSampleCount, sphereSurface);
		}

		private Vector3[] CreateBaseSamples(float size)
		{
			int baseSamplesCount = (int)(TargetSampleCount * BaseSamplesFactor);
			return _randomPointsGenerator.Generate(baseSamplesCount, size);
		}

		private static void ValidateSize(float size)
		{
			if (size <= 0)
				throw new ArgumentOutOfRangeException();
		}

		private void CreateTrianglesFor(Mesh mesh, float size)
		{
			Vector3[] vertices = mesh.vertices;
			Vector3BinaryKDTree tree = new Vector3BinaryKDTree(vertices, _quickSelector);
			Vector2 minMaxDistance = GetMinMaxVertexDistance(vertices, tree, size);
			float radius = minMaxDistance[1] * 5;
			List<int> triangles = new List<int>();

			for (int i = 0; i < vertices.Length; i++)
			{
				IList<int> iNeighbors = tree.GetNearestToWithin(i, radius);

				for (int j = 0; j < iNeighbors.Count; j++)
				{
					int i0 = iNeighbors[j];
					int i1 = -1;
					float minJK = float.MaxValue;
					Vector3 vj = vertices[iNeighbors[j]];
					for (int k = 0; k < iNeighbors.Count; k++)
					{
						int ik = iNeighbors[k];
						if (i0 == ik)
							continue;
						Vector3 vk = vertices[iNeighbors[k]];
						float distanceSqr = (vj - vk).sqrMagnitude;
						if (distanceSqr < minJK)
						{
							minJK = distanceSqr;
							i1 = iNeighbors[k];
						}
					}

					Vector3 vI = vertices[i];
					Vector3 vI0 = vertices[i0];
					Vector3 vI1 = vertices[i1];

					Vector3 d0 = vI - vI0;
					Vector3 d1 = vI - vI1;

					Vector3 normal = Vector3.Cross(d0, d1);
					float scalar = Vector3.Dot(vI, normal);

					if (scalar > 0)
					{
						triangles.Add(i);
						triangles.Add(i0);
						triangles.Add(i1);
					}
					else
					{
						triangles.Add(i);
						triangles.Add(i1);
						triangles.Add(i0);
					}
				}
			}

			mesh.triangles = triangles.ToArray();
		}

		private Vector2 GetMinMaxVertexDistance(Vector3[] vertices, Vector3BinaryKDTree tree, float size)
		{
			float min = float.MaxValue;
			float max = float.MinValue;
			float weightRadius = 2 * CalculateWeightRadius(size);
			for (int i = 0; i < vertices.Length; i++)
			{
				IList<int> neighborIndices = tree.GetNearestToWithin(i, weightRadius);
				for (int j = 0; j < neighborIndices.Count; j++)
				{
					float distance = (vertices[i] - vertices[neighborIndices[j]]).sqrMagnitude;
					if (distance < min)
						min = distance;
					if (distance > max)
						max = distance;
				}
			}
			return new Vector2(Mathf.Sqrt(min), Mathf.Sqrt(max));
		}

		private float CalculateWeightRadius(float size)
		{
			float sphereSurface = 4 * Mathf.PI * size;
			return Mathf.Sqrt(sphereSurface / (2 * Mathf.Sqrt(3) * TargetSampleCount));
		}
	}
}