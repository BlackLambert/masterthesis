using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
	/// <summary>
	/// Sources: http://media.steampowered.com/apps/valve/2014/DirkGregorius_ImplementingQuickHull.pdf
	/// https://github.com/Habrador/Computational-geometry
	/// </summary>
	public class SphericalDelaunayTriangulation
    {
        public DelaunayTriangle[] Create(IList<Vector3> vertices)
		{
            ValidateVertices(vertices);


			List<DelaunayTriangle> result = CreateInitialTetrahedron(vertices);
			HashSet<int> addedIndices = GetUniqueIndices(result);
			for (int i = 0; i < vertices.Count; i++)
			{
				if (addedIndices.Contains(i))
					continue;
				AddPoint(i, vertices, result);
			}
			return result.ToArray();
		}

		private HashSet<int> GetUniqueIndices(List<DelaunayTriangle> tirangles)
		{
			HashSet<int> result = new HashSet<int>();
			foreach (DelaunayTriangle triangle in tirangles)
			{
				result.Add(triangle.VertexIndices[0]);
				result.Add(triangle.VertexIndices[1]);
				result.Add(triangle.VertexIndices[2]);
			}
			return result;
		}

		private void AddPoint(int index, IList<Vector3> vertices, List<DelaunayTriangle> triangles)
		{
			Vector3 point = vertices[index];
			List<DelaunayTriangle> trianglesToDelete = new List<DelaunayTriangle>();
			foreach (DelaunayTriangle triangle in triangles)
			{
				Vector3 pointToTriangleCorner = vertices[triangle.VertexIndices[0]] - point;
				float dotProduct = Vector3.Dot(triangle.Normal, pointToTriangleCorner);
				if (dotProduct < 0)
					trianglesToDelete.Add(triangle);
			}

			foreach (DelaunayTriangle triangle in trianglesToDelete)
			{
				triangles.Remove(triangle);
			}

			Vector2Int[] unsharedEdges = GetUnsharedEdges(trianglesToDelete);

			foreach (Vector2Int edge in unsharedEdges)
				ReconnectPoints(index, vertices, triangles, edge);
		}

		private Vector2Int[] GetUnsharedEdges(List<DelaunayTriangle> trianglesToDelete)
		{
			List<Vector2Int> result = new List<Vector2Int>();
			for (int i = 0; i < trianglesToDelete.Count; i++)
			{
				DelaunayTriangle triangle = trianglesToDelete[i];
				for (int j = 0; j < 3; j++)
				{
					Vector2Int edge = new Vector2Int(triangle.VertexIndices[j], triangle.VertexIndices[(j+1)%3]);
					bool isShared = false;
					for (int k = 0; k < trianglesToDelete.Count; k++)
					{
						if (k == i)
							continue;
						DelaunayTriangle triangleToCompare = trianglesToDelete[k];
						isShared = isShared || triangleToCompare.ContainsEdge(edge);
					}
					if (!isShared)
						result.Add(edge);
				}
			}
			return result.ToArray();
		}

		private void ReconnectPoints(int pointIndex, IList<Vector3> vertices, List<DelaunayTriangle> result, Vector2Int edge)
		{
			Vector3Int indices = new Vector3Int(edge[0], edge[1], pointIndex);
			result.Add(CreateTriangle(indices, vertices));
		}

		private List<DelaunayTriangle> CreateInitialTetrahedron(IList<Vector3> vertices)
		{
			Vector3 v0 = vertices[0];
			int i1 = FindFarthestAwayFrom(new Vector3[]{ v0}, vertices);
			Vector3 v1 = vertices[i1];
			int i2 = FindFarthestAwayFrom(new Vector3[] { v0, v1}, vertices);
			Vector3 v2 = vertices[i2];
			int i3 = FindFarthestAwayFrom(new Vector3[] { v0, v1, v2}, vertices);
			Vector3 v3 = vertices[i3];
			Vector3 tetrahedronCenter = (v0 + v1 + v2 + v3) / 4;
			DelaunayTriangle t0 = CreateTriangle(new Vector3Int(0, i1, i2), vertices, tetrahedronCenter);
			DelaunayTriangle t1 = CreateTriangle(new Vector3Int(0, i1, i3), vertices, tetrahedronCenter);
			DelaunayTriangle t2 = CreateTriangle(new Vector3Int(i1, i2, i3), vertices, tetrahedronCenter);
			DelaunayTriangle t3 = CreateTriangle(new Vector3Int(i2, 0, i3), vertices, tetrahedronCenter);
			return new List<DelaunayTriangle> { t0, t1, t2, t3 };
		}

		private int FindFarthestAwayFrom(Vector3[] targetPoints, IList<Vector3> points)
		{
			float combinedDistance = 0;
			int result = -1;

			for (int i = 0; i < points.Count; i++)
			{
				for (int j = 0; j < targetPoints.Length; j++)
				{
					if (targetPoints[j] == points[i])
						continue;
				}

				float currentCombinedDistance = 0;
				for (int j = 0; j < targetPoints.Length; j++)
					currentCombinedDistance += (targetPoints[j] - points[i]).magnitude;
				if(currentCombinedDistance > combinedDistance)
				{
					combinedDistance = currentCombinedDistance;
					result = i;
				}

			}

			return result;
		}

		private DelaunayTriangle CreateTriangle(Vector3Int vertexIndices, IList<Vector3> vertices)
		{
			Vector3 xy = vertices[vertexIndices[1]] - vertices[vertexIndices[0]];
			Vector3 xz = vertices[vertexIndices[2]] - vertices[vertexIndices[0]];
			Vector3 normal = Vector3.Cross(xy, xz).normalized;
			return new DelaunayTriangle(vertexIndices, normal);
		}

		private DelaunayTriangle CreateTriangle(Vector3Int vertexIndices, IList<Vector3> vertices, Vector3 center)
		{
			Vector3 xy = vertices[vertexIndices[1]] - vertices[vertexIndices[0]];
			Vector3 xz = vertices[vertexIndices[2]] - vertices[vertexIndices[0]];
			Vector3 normal = Vector3.Cross(xy, xz).normalized;
			Vector3 centerToPoint = vertices[vertexIndices[0]] - center;
			if (Vector3.Dot(centerToPoint, normal) < 0)
				return CreateTriangle(new Vector3Int(vertexIndices[0], vertexIndices[2], vertexIndices[1]), vertices, center);
			return new DelaunayTriangle(vertexIndices, normal);
		}

		private void ValidateVertices(IList<Vector3> vertices)
		{
			if (vertices.Count < 4)
				throw new ArgumentException();
		}
	}
}