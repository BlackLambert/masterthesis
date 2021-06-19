using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SBaier.Master
{
	public class IterativeMeshSubdivider : MeshSubdivider
	{
		private const int _indicesPerTriangle = 3;
		private const int _edgesPerTriangle = 3;

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
			Vector3[] edgeDelta = new Vector3[3];

			int trianglesAmount = vertexIndices.Length / _indicesPerTriangle;
			int newEdgesAmountPerEdge = amount + 1;
			int newVerticesAmountPerEdge = amount + 2;
			int newVerticesPerTriangle = (newVerticesAmountPerEdge * newVerticesAmountPerEdge + newVerticesAmountPerEdge) / 2;
			int newTrianglesPerTriangleAmount = GetNewTrianglesAmountPerTriangleFor(amount);
			int[] currentTriangleVetexIndices = new int[newVerticesPerTriangle];

			List<Vector3> newVertices = new List<Vector3>(newVerticesPerTriangle * trianglesAmount);
			int[] newVertexIndices = new int[newTrianglesPerTriangleAmount * trianglesAmount * _indicesPerTriangle];

			// Shared edges handling
			Dictionary<Vector2Int, Vector2Int> sharedEdgeIndices = GetSharedEdgesIndices(mesh);
			Dictionary<Vector2Int, int[]> sharedVertexIndices = new Dictionary<Vector2Int, int[]>();

			foreach (Vector2Int sharedEdge in sharedEdgeIndices.Values)
				sharedVertexIndices[sharedEdge] = new int[newVerticesAmountPerEdge];
				
			int[][] sharedVertexIndicesToFill = new int[3][];
			bool[] fillSharedEdge = new bool[3];
			int[][] sharedVerticesToRead = new int[3][];
			bool[] useSharedEdge = new bool[3];
			Vector2Int[] edges = new Vector2Int[3];

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

				
				edgeDelta[0] = edgeVectors[0] / newEdgesAmountPerEdge;
				edgeDelta[1] = edgeVectors[1] / newEdgesAmountPerEdge;
				edgeDelta[2] = edgeVectors[2] / newEdgesAmountPerEdge;


				// Shared edges handling
				for (int j = 0; j < currentTriangle.Length; j++)
					edges[j] = new Vector2Int(i * _indicesPerTriangle + j, i * _indicesPerTriangle + (j + 1) % _indicesPerTriangle);
				// Set shared edges to fill the indices
				for (int j = 0; j < currentTriangle.Length; j++)
				{
					fillSharedEdge[j] = sharedVertexIndices.ContainsKey(edges[j]);
					sharedVertexIndicesToFill[j] = fillSharedEdge[j] ? sharedVertexIndices[edges[j]] : null;
				}
				// Set shared edges to read indices from
				for (int j = 0; j < currentTriangle.Length; j++)
				{
					useSharedEdge[j] = sharedEdgeIndices.ContainsKey(edges[j]);
					sharedVerticesToRead[j] = useSharedEdge[j] ? sharedVertexIndices[sharedEdgeIndices[edges[j]]] : null;
				}

				for (int j = 0; j < currentTriangleVetexIndices.Length; j++)
					currentTriangleVetexIndices[j] = -1;


				int currentTriangleIndex = 0;
				Vector3 pos = cornerVertices[0];

				// Move around the three edges, determine indices, create vertices
				for (int j = 0; j < _edgesPerTriangle; j++)
				{
					int maxE = newVerticesAmountPerEdge - 1;
					for (int e = 0; e <= maxE; e++)
					{
						if (e == maxE)
						{
							if (useSharedEdge[j])
								currentTriangleVetexIndices[currentTriangleIndex] = sharedVerticesToRead[j][maxE - e];
							continue;
						}

						int index = currentTriangleVetexIndices[currentTriangleIndex] == -1 ? vertexIndex : currentTriangleVetexIndices[currentTriangleIndex];

						if (useSharedEdge[j])
							index = sharedVerticesToRead[j][maxE - e];
						if(fillSharedEdge[j])
							sharedVertexIndicesToFill[j][e] = index;

						
						if (index == vertexIndex)
						{
							newVertices.Add(pos);
							vertexIndex++;
						}
						
						currentTriangleVetexIndices[currentTriangleIndex] = index;

						if (e == 0 && fillSharedEdge[(j + 2) % _edgesPerTriangle])
							sharedVertexIndicesToFill[(j + 2) % _edgesPerTriangle][maxE] = currentTriangleVetexIndices[currentTriangleIndex];

						pos += edgeDelta[j];
						if (j == 0)
							currentTriangleIndex += newVerticesAmountPerEdge - e;
						else if (j == 1)
							currentTriangleIndex -= (e + 1);
						else if (j == 2)
							currentTriangleIndex -= 1;
					}
				}

				int currentVertexIndex = 0;
				for (int y = 0; y < newVerticesAmountPerEdge; y++)
				{
					int maxX = newVerticesAmountPerEdge - y - 1;
					int maxY = newVerticesAmountPerEdge - 1;

					for (int x = 0; x <= maxX; x++)
					{

						if (x > 0 && y > 0 && x < maxX && y < maxY)
						{
							newVertices.Add(cornerVertices[0] + edgeDelta[0] * y + edgeDelta[2] * (-1) * x);
							currentTriangleVetexIndices[currentVertexIndex] = vertexIndex;
							vertexIndex++;
						}

						bool createTrianglePointingUp = y > 0;
						bool createTrianglePointingDown = createTrianglePointingUp && y < newVerticesAmountPerEdge && x > 0;

						// Triangle creation
						if (createTrianglePointingUp)
						{
							// Create triangle pointing upwards
							newVertexIndices[triangleIndex] = currentTriangleVetexIndices[currentVertexIndex];
							newVertexIndices[triangleIndex + 1] = currentTriangleVetexIndices[currentVertexIndex - maxX - 1];
							newVertexIndices[triangleIndex + 2] = currentTriangleVetexIndices[currentVertexIndex - maxX - 2];
							triangleIndex += 3;

							if (createTrianglePointingDown)
							{
								// Create triangle pointing downwards
								newVertexIndices[triangleIndex] = currentTriangleVetexIndices[currentVertexIndex];
								newVertexIndices[triangleIndex + 1] = currentTriangleVetexIndices[currentVertexIndex - maxX - 2];
								newVertexIndices[triangleIndex + 2] = currentTriangleVetexIndices[currentVertexIndex - 1];
								triangleIndex += 3;
							}
						}
						currentVertexIndex++;
					}
				}
				if(i == 20)
				{
					mesh.triangles = null;
					mesh.vertices = newVertices.ToArray();
					mesh.triangles = newVertexIndices;
					break;
				}
			}
			mesh.triangles = null;
			mesh.vertices = newVertices.ToArray();
			mesh.triangles = newVertexIndices;
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

		private Dictionary<Vector2Int, Vector2Int> GetSharedEdgesIndices(Mesh mesh)
		{
			Dictionary<Vector2Int, Vector2Int> result = new Dictionary<Vector2Int, Vector2Int>();
			Dictionary<Vector2Int, Vector2Int> edgeToIndices = new Dictionary<Vector2Int, Vector2Int>();

			int[] vertexIndices = mesh.triangles;
			int verticesPerTriangle = 3;
			int trianglesCount = vertexIndices.Length / verticesPerTriangle;
			int edgesPerTriangle = 3;

			for (int i = 0; i < trianglesCount; i++)
			{
				for (int j = 0; j < edgesPerTriangle; j++)
				{
					int index = i * verticesPerTriangle + j;
					int nextIndex = i * verticesPerTriangle + (j + 1) % verticesPerTriangle;
					int vertexIndex1 = vertexIndices[index];
					int vertexIndex2 = vertexIndices[nextIndex];
					Vector2Int edge = new Vector2Int(vertexIndex1, vertexIndex2);
					Vector2Int reverseEdge = new Vector2Int(vertexIndex2, vertexIndex1);
					Vector2Int indices = new Vector2Int(index, nextIndex);

					if (edgeToIndices.ContainsKey(edge))
						result.Add(indices, edgeToIndices[edge]);
					else if(edgeToIndices.ContainsKey(reverseEdge))
						result.Add(indices, edgeToIndices[reverseEdge]);
					else
						edgeToIndices.Add(edge, indices);
				}
			}
			return result;
		}
	}
}