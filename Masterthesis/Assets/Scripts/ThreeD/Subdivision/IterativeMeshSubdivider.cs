using System;
using UnityEngine;

namespace SBaier.Master
{
	public class IterativeMeshSubdivider : MeshSubdivider
	{
		private const int _indicesPerTriangle = 3;
		private DuplicateVerticesRemover _duplicateVerticesRemover;

		public IterativeMeshSubdivider(DuplicateVerticesRemover duplicateVerticesRemover)
		{
			_duplicateVerticesRemover = duplicateVerticesRemover;
		}


		public void Subdivide(Mesh mesh)
		{
			Subdivide(mesh, 1);
		}

		public void Subdivide(Mesh mesh, int amount)
		{
			CheckAmountValid(amount);

			int[] vertexIndices = mesh.triangles;
			Vector3[] vertices = mesh.vertices;

			int[] currentTriangle = new int[_indicesPerTriangle];
			Vector3[] cornerVertices = new Vector3[_indicesPerTriangle];
			Vector3[] edgeVectors = new Vector3[_indicesPerTriangle];
			Vector3[] egdeDelta = new Vector3[2];

			int trianglesAmount = vertexIndices.Length / _indicesPerTriangle;
			int newEdgesAmountPerEdge = amount + 1;
			int newVerticesAmountPerEdge = amount + 2;
			int newVerticesPerTriangle = (newVerticesAmountPerEdge * newVerticesAmountPerEdge + newVerticesAmountPerEdge) / 2;
			int newTrianglesPerTriangleAmount = GetNewTrianglesAmountPerTriangleFor(amount);
			Vector3[] newVertices = new Vector3[newVerticesPerTriangle * trianglesAmount];
			int[] newVertexIndices = new int[newTrianglesPerTriangleAmount * trianglesAmount * _indicesPerTriangle];

			int vertexIndex = 0;
			int triangleIndex = 0;

			for (int i = 0; i < trianglesAmount; i++)
			{
				for(int j = 0; j < currentTriangle.Length; j++)
					currentTriangle[j] = vertexIndices[i * _indicesPerTriangle + j];

				for(int j = 0; j < cornerVertices.Length; j++)
					cornerVertices[j] = vertices[currentTriangle[j]];

				for(int j = 0; j < edgeVectors.Length; j++)
					edgeVectors[j] = cornerVertices[(j + 1) % 3] - cornerVertices[j];

				egdeDelta[0] = (edgeVectors[2] / newEdgesAmountPerEdge) * (-1);
				egdeDelta[1] = edgeVectors[0] / newEdgesAmountPerEdge;

				for(int y = 0; y < newVerticesAmountPerEdge; y++)
				{
					
					for (int x = 0; x < newVerticesAmountPerEdge - y; x++)
					{
						newVertices[vertexIndex] = cornerVertices[0] + egdeDelta[0] * x + egdeDelta[1] * y;

						if (x < newVerticesAmountPerEdge - 1 - y && y < newVerticesAmountPerEdge -1)
						{
							newVertexIndices[triangleIndex] = vertexIndex;
							newVertexIndices[triangleIndex + 2] = vertexIndex + 1;
							newVertexIndices[triangleIndex + 1] = vertexIndex + newVerticesAmountPerEdge - y;
							triangleIndex += 3;

							if (x > 0)
							{
								newVertexIndices[triangleIndex] = vertexIndex;
								newVertexIndices[triangleIndex + 2] = vertexIndex + newVerticesAmountPerEdge - y;
								newVertexIndices[triangleIndex + 1] = vertexIndex + newVerticesAmountPerEdge - y - 1;
								triangleIndex += 3;
							}
						}
						vertexIndex++;
					}
				}
			}

			mesh.vertices = newVertices;
			mesh.triangles = newVertexIndices;
			_duplicateVerticesRemover.RunOn(mesh);
		}

		private void CheckAmountValid(int amount)
		{
			if (amount < 1)
				throw new ArgumentOutOfRangeException();
		}

		private int GetNewTrianglesAmountPerTriangleFor(int subdivisions)
		{
			int result = 0;
			for (int i = 0; i <= subdivisions; i++)
				result += 1 + (2 * i);
			return result;
		}
	}
}