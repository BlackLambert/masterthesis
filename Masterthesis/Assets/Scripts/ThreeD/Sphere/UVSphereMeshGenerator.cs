using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
	public class UVSphereMeshGenerator : MeshGenerator
	{
		private const int _minimalRingsAmount = 1;
		private const int _minimalSegmentsAmount = 3;

		public int RingsAmount { get; set; }
		public int SegmentsAmount { get; set; }

		public UVSphereMeshGenerator(int ringsAmount, int segementsAmount)
		{
			ValidateRingsAmountRange(ringsAmount);
			ValidateSegmentsAmountRange(segementsAmount);

			RingsAmount = ringsAmount;
			SegmentsAmount = segementsAmount;
		}

		public void GenerateMeshFor(Mesh mesh)
		{
			GenerateMeshFor(mesh, 1);
		}

		public void GenerateMeshFor(Mesh mesh, float size)
		{
			ValidateSize(size);
			mesh.vertices = CreateVertices(size);
			mesh.triangles = CreateTriangles();
			mesh.normals = mesh.vertices;
		}

		private Vector3[] CreateVertices(float size)
		{
			Vector3[] result = new Vector3[GetVertexAmount()];
			float segmentDetla = (1f / SegmentsAmount * Mathf.PI) * 2;
			float ringsDetla = 1f / (RingsAmount + 1) * Mathf.PI;

			result[0] = Vector3.down * size;
			result[result.Length - 1] = Vector3.up * size;
			for (int ring = 1; ring <= RingsAmount; ring++)
			{
				float ringAngle = Mathf.PI - (ringsDetla * ring);
				float sinRingAngle = Mathf.Sin(ringAngle);
				float cosRingAngle = Mathf.Cos(ringAngle);
				for (int segment = 0; segment < SegmentsAmount; segment++)
				{
					float segmentAngle = segmentDetla * segment;
					float sinSegmentAngle = Mathf.Sin(segmentAngle);
					float cosSegmentAngle = Mathf.Cos(segmentAngle);

					float x = sinRingAngle * cosSegmentAngle;
					float z = sinRingAngle * sinSegmentAngle;
					float y = cosRingAngle;

					int index = 1 + (ring - 1) * SegmentsAmount + segment;
					result[index] = new Vector3(x, y, z) * size;
				}
			}

			return result;
		}

		private int[] CreateTriangles()
		{
			int[] result = new int[GetVertexIndicesAmount()];
			int vertexAmount = GetVertexAmount();

			int index = 0;
			for (int ring = 1; ring <= RingsAmount + 1; ring++)
			{
				for (int segment = 0; segment < SegmentsAmount; segment++)
				{
					int ringStartIndex = 1 + (ring - 1) * SegmentsAmount;
					int v0 = ringStartIndex + segment - SegmentsAmount;
					int v1 = ringStartIndex + segment;
					int v2 = ringStartIndex + (segment + 1) % SegmentsAmount;
					int v3 = ringStartIndex + (segment + 1) % SegmentsAmount - SegmentsAmount;

					if (ring == 1)
					{
						result[index] = 0;
						result[index + 1] = v1;
						result[index + 2] = v2;
						index += 3;
					}
					else if (ring == RingsAmount + 1)
					{
						result[index] = vertexAmount - 1 - SegmentsAmount + segment;
						result[index + 1] = vertexAmount - 1;
						result[index + 2] = vertexAmount - 1 - SegmentsAmount + (segment + 1) % SegmentsAmount;
						index += 3;
					}
					else
					{
						result[index] = v0;
						result[index + 1] = v1;
						result[index + 2] = v2;
						result[index + 3] = v0;
						result[index + 4] = v2;
						result[index + 5] = v3;
						index += 6;
					}
				}
			}
			return result;
		}

		private int GetVertexIndicesAmount()
		{
			return RingsAmount * SegmentsAmount * 3 * 2;
		}

		private int GetVertexAmount()
		{
			int poleVertices = 2;
			int otherVertices = RingsAmount * SegmentsAmount;
			return poleVertices + otherVertices;
		}

		private void ValidateSize(float size)
		{
			if (size <= 0)
				throw new ArgumentOutOfRangeException();
		}

		private void ValidateSegmentsAmountRange(double segementsAmount)
		{
			if (segementsAmount < _minimalSegmentsAmount)
				throw new ArgumentOutOfRangeException();
		}

		private void ValidateRingsAmountRange(double ringsAmount)
		{
			if (ringsAmount < _minimalRingsAmount)
				throw new ArgumentOutOfRangeException();
		}
	}
}