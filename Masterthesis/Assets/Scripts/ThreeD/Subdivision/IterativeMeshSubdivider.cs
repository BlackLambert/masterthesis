using System;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
	public class IterativeMeshSubdivider : MeshSubdivider
	{
		private const int _verticesPerTriangle = 3;
		private const int _edgesPerTriangle = 3;

		// mesh values
		private int[] _meshVertexIndices;
		private Vector3[] _meshVertices;

		// current triangle values
		private int[] _currentTriangle = new int[_verticesPerTriangle];
		private Vector3[] _cornerVertices = new Vector3[_verticesPerTriangle];
		private Vector3[] _edgeVectors = new Vector3[_verticesPerTriangle];
		private Vector3[] _edgeDelta = new Vector3[_verticesPerTriangle];
		private int[] _newVertexIndicesOfCurrentTriangle;

		// shared edges
		private Dictionary<Vector2Int, Vector2Int> _sharedEdges = new Dictionary<Vector2Int, Vector2Int>();
		private Dictionary<Vector2Int, int[]> _sharedVertexIndices = new Dictionary<Vector2Int, int[]>();
		private int[][] _sharedVertexIndicesToWrite = new int[_edgesPerTriangle][];
		private bool[] _writeSharedEdge = new bool[_edgesPerTriangle];
		private int[][] _sharedVerticesToRead = new int[_edgesPerTriangle][];
		private bool[] _useSharedEdge = new bool[_edgesPerTriangle];
		private Vector2Int[] _edges = new Vector2Int[_edgesPerTriangle];

		// new mesh values
		private List<Vector3> _newVertices;
		private int[] _newVertexIndices;

		int _vertexIndex = 0;
		int _triangleIndex = 0;
		int _currentVertexIndex = 0;
		int _currentTriangleIndex = 0;

		public void Subdivide(Mesh mesh)
		{
			Subdivide(mesh, 1);
		}

		public void Subdivide(Mesh mesh, int subdivisionAmount)
		{
			CheckAmountValid(subdivisionAmount);

			InitCurrentMeshValues(mesh);
			InitNewMeshValues(subdivisionAmount);
			InitSharedEdgesIndices();
			InitSharedVertexIndices(subdivisionAmount);

			CalculateNewMeshValues(subdivisionAmount);

			SetNewMeshValues(mesh);
		}

		private void CalculateNewMeshValues(int subdivisionAmount)
		{
			int meshTrianglesAmount = GetMeshTrianglesAmount();
			for (int triangleIdex = 0; triangleIdex < meshTrianglesAmount; triangleIdex++)
				CalculateNewValuesOfTriangle(triangleIdex, subdivisionAmount);
		}

		private void CalculateNewValuesOfTriangle(int triangleIndex, int subdivisionAmount)
		{
			int newVerticesAmountPerEdge = GetNewVerticesAmountPerEdge(subdivisionAmount);

			InitCurrentTriangle(triangleIndex, subdivisionAmount);
			SetNewMeshValuesOfTheEdges(newVerticesAmountPerEdge);
			CreateNewMeshValuesForCurrentTriangle(newVerticesAmountPerEdge);
		}

		private void InitCurrentTriangle(int triangleIndex, int subdivisionAmount)
		{
			_currentVertexIndex = 0;
			_currentTriangleIndex = 0;

			InitCurrentTriangleVertexIndices(triangleIndex);
			InitCornerVertices();
			InitEdgeVectors();
			InitEdgeDeltas(subdivisionAmount);
			InitEdgesOf(triangleIndex);
			InitSharedEdgesToWrite();
			InitSharedEdgesToRead();
			InitVertexIndiciesOfCurrentTriangle(subdivisionAmount);
		}

		private void InitSharedEdgesToRead()
		{
			for (int i = 0; i < _currentTriangle.Length; i++)
				InitSharedEdgeToRead(i);
		}

		private void InitSharedEdgesToWrite()
		{
			for (int i = 0; i < _currentTriangle.Length; i++)
				InitSharedEdgeToWrite(i);
		}

		private void InitEdgesOf(int triangleIndex)
		{
			for (int edgeIndex = 0; edgeIndex < _currentTriangle.Length; edgeIndex++)
				InitEdge(triangleIndex, edgeIndex);
		}

		private void InitEdgeDeltas(int subdivisionAmount)
		{
			int newEdgesAmountPerEdge = subdivisionAmount + 1;
			for (int i = 0; i < _currentTriangle.Length; i++)
				InitEdgeDelta(newEdgesAmountPerEdge, i);
		}

		private void InitEdgeVectors()
		{
			for (int edgeIndex = 0; edgeIndex < _edgeVectors.Length; edgeIndex++)
				InitEdgeVector(edgeIndex);
		}

		private void InitCornerVertices()
		{
			for (int edgeIndex = 0; edgeIndex < _cornerVertices.Length; edgeIndex++)
				InitCornerVertex(edgeIndex);
		}

		private void InitCurrentTriangleVertexIndices(int triangleIndex)
		{
			for (int index = 0; index < _currentTriangle.Length; index++)
				InitCurrentTriangleVertexIndex(triangleIndex, index);
		}

		private void CheckAmountValid(int amount)
		{
			if (amount < 0)
				throw new ArgumentOutOfRangeException();
		}

		private void InitNewMeshValues(int subdivisionsAmount)
		{
			int currentTrianglesAmount = GetMeshTrianglesAmount();
			int newVerticesAmountPerEdge = GetNewVerticesAmountPerEdge(subdivisionsAmount);
			int newTrianglesPerTriangleAmount = GetNewTrianglesAmountPerTriangleFor(subdivisionsAmount);
			int newVerticesPerTriangle = GetNewVerticesPerTriangle(newVerticesAmountPerEdge);

			int newVerticesLength = newVerticesPerTriangle * currentTrianglesAmount;
			_newVertices = new List<Vector3>(newVerticesLength);
			int newVertexIndicesLength = newTrianglesPerTriangleAmount * currentTrianglesAmount * _verticesPerTriangle;
			_newVertexIndices = new int[newVertexIndicesLength];

			_vertexIndex = 0;
			_triangleIndex = 0;
		}

		private void InitSharedEdgesIndices()
		{
			_sharedEdges.Clear();
			Dictionary<Vector2Int, Vector2Int> edgeToIndices = new Dictionary<Vector2Int, Vector2Int>();
			int trianglesCount = _meshVertexIndices.Length / _verticesPerTriangle;

			for (int triangleIndex = 0; triangleIndex < trianglesCount; triangleIndex++)
				InitSharedEdgeVerticesForTriangle(ref edgeToIndices, triangleIndex);
		}

		private void InitSharedEdgeVerticesForTriangle(ref Dictionary<Vector2Int, Vector2Int> edgeToIndices, int triangleIndex)
		{
			for (int edgeIndex = 0; edgeIndex < _edgesPerTriangle; edgeIndex++)
				InitSharedEdgeVerticesFor(ref edgeToIndices, triangleIndex, edgeIndex);
		}

		private void InitSharedEdgeVerticesFor(ref Dictionary<Vector2Int, Vector2Int> edgeToIndices, int triangleIndex, int edgeIndex)
		{
			int startIndex = triangleIndex * _verticesPerTriangle + edgeIndex;
			int endIndex = triangleIndex * _verticesPerTriangle + (edgeIndex + 1) % _verticesPerTriangle;
			int meshVertexStartIndex = _meshVertexIndices[startIndex];
			int meshVertexEndIndex = _meshVertexIndices[endIndex];
			Vector2Int edge = new Vector2Int(meshVertexStartIndex, meshVertexEndIndex);
			Vector2Int reverseEdge = new Vector2Int(meshVertexEndIndex, meshVertexStartIndex);
			Vector2Int meshVertexIndices = new Vector2Int(startIndex, endIndex);

			if (edgeToIndices.ContainsKey(edge))
				_sharedEdges.Add(meshVertexIndices, edgeToIndices[edge]);
			else if (edgeToIndices.ContainsKey(reverseEdge))
				_sharedEdges.Add(meshVertexIndices, edgeToIndices[reverseEdge]);
			else
				edgeToIndices.Add(edge, meshVertexIndices);
		}

		private void InitSharedVertexIndices(int subivisionsAmount)
		{
			int newVerticesAmountPerEdge = GetNewVerticesAmountPerEdge(subivisionsAmount);
			_sharedVertexIndices.Clear();
			foreach (Vector2Int sharedEdge in _sharedEdges.Values)
				_sharedVertexIndices[sharedEdge] = new int[newVerticesAmountPerEdge];
		}

		private void InitVertexIndiciesOfCurrentTriangle(int subdivisionAmount)
		{
			int newVerticesAmountPerEdge = GetNewVerticesAmountPerEdge(subdivisionAmount);
			int newVerticesPerTriangle = GetNewVerticesPerTriangle(newVerticesAmountPerEdge);
			_newVertexIndicesOfCurrentTriangle = new int[newVerticesPerTriangle];
			for (int j = 0; j < _newVertexIndicesOfCurrentTriangle.Length; j++)
				_newVertexIndicesOfCurrentTriangle[j] = -1;
		}

		private void SetNewMeshValuesOfTheEdges(int newVerticesAmountPerEdge)
		{
			for (int edgeIndex = 0; edgeIndex < _edgesPerTriangle; edgeIndex++)
				SetNewMeshValuesOfEdge(newVerticesAmountPerEdge, _cornerVertices[edgeIndex], edgeIndex);
		}

		private void SetNewMeshValuesOfEdge(int newVerticesAmountPerEdge, Vector3 startPos, int edgeIndex)
		{
			Vector3 pos = startPos;
			int maxEdgeVertexIndex = newVerticesAmountPerEdge - 1;
			for (int edgeVertexIndex = 0; edgeVertexIndex <= maxEdgeVertexIndex; edgeVertexIndex++)
			{
				bool isEdgeEnd = edgeVertexIndex == maxEdgeVertexIndex;
				if (isEdgeEnd)
				{
					if (_useSharedEdge[edgeIndex])
						_newVertexIndicesOfCurrentTriangle[_currentTriangleIndex] = _sharedVerticesToRead[edgeIndex][maxEdgeVertexIndex - edgeVertexIndex];
					continue;
				}

				int index = _newVertexIndicesOfCurrentTriangle[_currentTriangleIndex] == -1 ? _vertexIndex : _newVertexIndicesOfCurrentTriangle[_currentTriangleIndex];

				if (_useSharedEdge[edgeIndex])
					index = _sharedVerticesToRead[edgeIndex][maxEdgeVertexIndex - edgeVertexIndex];
				if (_writeSharedEdge[edgeIndex])
					_sharedVertexIndicesToWrite[edgeIndex][edgeVertexIndex] = index;


				if (index == _vertexIndex)
					CreateVertex(pos);
				_newVertexIndicesOfCurrentTriangle[_currentTriangleIndex] = index;

				if (edgeVertexIndex == 0 && _writeSharedEdge[(edgeIndex + 2) % _edgesPerTriangle])
					_sharedVertexIndicesToWrite[(edgeIndex + 2) % _edgesPerTriangle][maxEdgeVertexIndex] = _newVertexIndicesOfCurrentTriangle[_currentTriangleIndex];

				pos = pos.FastAdd(_edgeDelta[edgeIndex]);
				_currentTriangleIndex += GetCurrentTriangleIndexDelta(newVerticesAmountPerEdge, edgeIndex, edgeVertexIndex);
			}
		}

		private int GetCurrentTriangleIndexDelta(int newVerticesAmountPerEdge, int edgeIndex, int edgeVertexIndex)
		{
			switch(edgeIndex)
			{
				case 0:
					return newVerticesAmountPerEdge - edgeVertexIndex;
				case 1:
					return -(edgeVertexIndex + 1);
				case 2:
					return -1;
				default:
					throw new NotImplementedException();
			}
		}

		private void CreateNewMeshValuesForCurrentTriangle(int newVerticesAmountPerEdge)
		{
			_currentVertexIndex = 0;
			int maxRow = newVerticesAmountPerEdge - 1;
			for (int row = 0; row <= maxRow; row++)
				CreateNewMeshValueForRow(row, maxRow);
		}

		private void CreateNewMeshValueForRow(int row, int maxRow)
		{
			int maxColumn = maxRow - row;
			for (int column = 0; column <= maxColumn; column++)
				CreateNewMeshValuesAt(column, row, maxColumn, maxRow);
		}

		private void CreateNewMeshValuesAt(int column, int row, int maxColumn, int maxRow)
		{
			CreateInnerVertexAt(column, row, maxColumn, maxRow);
			CreateTrianglesAt(column, row, maxColumn, maxRow);
			_currentVertexIndex++;
		}

		private void CreateInnerVertexAt(int column, int row, int maxColumn, int maxRow)
		{
			bool notFirstColumn = column > 0;
			bool notLastColumn = column < maxColumn;
			bool notFirstRow = row > 0;
			bool notLastRow = row < maxRow;
			bool createVertex = notFirstColumn && notLastColumn && notFirstRow && notLastRow;

			if (!createVertex)
				return;

			Vector3 columnDelta = _edgeDelta[2].FastMultiply(-1).FastMultiply(column);
			Vector3 rowDelta = _edgeDelta[0].FastMultiply(row);
			Vector3 newVertexPos = _cornerVertices[0].FastAdd(columnDelta).FastAdd(rowDelta);
			_newVertexIndicesOfCurrentTriangle[_currentVertexIndex] = _vertexIndex;
			CreateVertex(newVertexPos);
		}

		private void CreateVertex(Vector3 pos)
		{
			_newVertices.Add(pos);
			_vertexIndex++;
		}

		private void CreateTrianglesAt(int column, int row, int maxColumn, int maxRow)
		{
			bool notFirstRow = row > 0;
			bool notFirstCollumn = column > 0;
			bool notLastRow = row < maxRow;

			bool createTrianglePointingUp = notFirstRow;
			bool createTrianglePointingDown = createTrianglePointingUp && notLastRow && notFirstCollumn;

			int i0 = _currentVertexIndex;
			int i1 = _currentVertexIndex - maxColumn - 1;
			int i2 = _currentVertexIndex - maxColumn - 2;
			int i3 = _currentVertexIndex - 1;

			
			if (createTrianglePointingUp)
				CreateTriangle(new Vector3Int(i0, i1, i2));
			if (createTrianglePointingDown)
				CreateTriangle(new Vector3Int(i0, i2, i3));
		}

		private int GetNewTrianglesAmountPerTriangleFor(int subdivisions)
		{
			int result = 0;
			for (int i = 0; i <= subdivisions; i++)
				result += 1 + (2 * i);
			return result;
		}

		private int GetMeshTrianglesAmount()
		{
			return _meshVertexIndices.Length / _verticesPerTriangle;
		}

		private static int GetNewVerticesAmountPerEdge(int subdivisionsAmount)
		{
			return subdivisionsAmount + 2;
		}

		private static int GetNewVerticesPerTriangle(int newVerticesAmountPerEdge)
		{
			return (newVerticesAmountPerEdge * newVerticesAmountPerEdge + newVerticesAmountPerEdge) / 2;
		}

		private void SetNewMeshValues(Mesh mesh)
		{
			mesh.vertices = _newVertices.ToArray();
			mesh.triangles = _newVertexIndices;
		}

		private void CreateTriangle(Vector3Int indices)
		{
			_newVertexIndices[_triangleIndex] = _newVertexIndicesOfCurrentTriangle[indices.x];
			_newVertexIndices[_triangleIndex + 1] = _newVertexIndicesOfCurrentTriangle[indices.y];
			_newVertexIndices[_triangleIndex + 2] = _newVertexIndicesOfCurrentTriangle[indices.z];
			_triangleIndex += 3;
		}

		private void InitSharedEdgeToRead(int j)
		{
			_useSharedEdge[j] = _sharedEdges.ContainsKey(_edges[j]);
			_sharedVerticesToRead[j] = _useSharedEdge[j] ? _sharedVertexIndices[_sharedEdges[_edges[j]]] : null;
		}

		private void InitSharedEdgeToWrite(int j)
		{
			_writeSharedEdge[j] = _sharedVertexIndices.ContainsKey(_edges[j]);
			_sharedVertexIndicesToWrite[j] = _writeSharedEdge[j] ? _sharedVertexIndices[_edges[j]] : null;
		}

		private void InitEdge(int triangleIndex, int edgeIndex)
		{
			int start = triangleIndex * _verticesPerTriangle + edgeIndex;
			int end = triangleIndex * _verticesPerTriangle + (edgeIndex + 1) % _verticesPerTriangle;
			_edges[edgeIndex] = new Vector2Int(start, end);
		}

		private void InitEdgeDelta(int newEdgesAmountPerEdge, int edgeIndex)
		{
			Vector3 edge = _edgeVectors[edgeIndex];
			_edgeDelta[edgeIndex] = edge / newEdgesAmountPerEdge;
		}

		private void InitEdgeVector(int edgeIndex)
		{
			int endVectorIndex = (edgeIndex + 1) % _verticesPerTriangle;
			Vector3 endVector = _cornerVertices[endVectorIndex];
			Vector3 startVector = _cornerVertices[edgeIndex];
			_edgeVectors[edgeIndex] = endVector - startVector;
		}

		private void InitCornerVertex(int edgeIndex)
		{
			int triangleIndex = _currentTriangle[edgeIndex];
			_cornerVertices[edgeIndex] = _meshVertices[triangleIndex];
		}

		private void InitCurrentMeshValues(Mesh mesh)
		{
			_meshVertexIndices = mesh.triangles;
			_meshVertices = mesh.vertices;
		}

		private void InitCurrentTriangleVertexIndex(int triangleIndex, int triangleVerticesIndex)
		{
			int meshVerticesIndex = triangleIndex * _verticesPerTriangle + triangleVerticesIndex;
			_currentTriangle[triangleVerticesIndex] = _meshVertexIndices[meshVerticesIndex];
		}
	}
}