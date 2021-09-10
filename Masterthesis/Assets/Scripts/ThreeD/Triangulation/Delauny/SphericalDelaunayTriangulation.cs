using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SBaier.Master
{
	/// <summary>
	/// Sources: http://media.steampowered.com/apps/valve/2014/DirkGregorius_ImplementingQuickHull.pdf
	/// https://github.com/Habrador/Computational-geometry
	/// </summary>
	public class SphericalDelaunayTriangulation
    {
		public List<Triangle> Result { get; private set; }
		private IList<Vector3> _vertices;

		public SphericalDelaunayTriangulation(IList<Vector3> vertices)
		{
			ValidateVertices(vertices);
			_vertices = vertices;
		}

        public void Create()
		{
			Result = CreateInitialTetrahedron();
			HashSet<int> addedIndices = GetUniqueIndices();
			for (int i = 0; i < _vertices.Count; i++)
			{
				if (addedIndices.Contains(i))
					continue;
				AddPoint(i);
			}
		}

		private HashSet<int> GetUniqueIndices()
		{
			HashSet<int> result = new HashSet<int>();
			foreach (Triangle triangle in Result)
			{
				int[] corners = triangle.VertexIndices;
				result.Add(corners[0]);
				result.Add(corners[1]);
				result.Add(corners[2]);
			}
			return result;
		}

		private void AddPoint(int index)
		{
			Vector3 point = _vertices[index];
			List<Polygon> trianglesInView = new List<Polygon>();
			List<int> trianglesToRemove = new List<int>();
			for (int i = Result.Count - 1; i >= 0; i--)
			{
				Triangle triangle = Result[i];
				if (IsInView(point, triangle))
				{
					trianglesInView.Add(triangle);
					trianglesToRemove.Add(i);
				}
			}

			for (int i = 0; i < trianglesToRemove.Count; i++)
				Result.RemoveAt(trianglesToRemove[i]);

			EdgesFinder unsharedEdgesFinder = new UnsharedEdgesFinder(trianglesInView.ToArray());
			Vector2Int[] unsharedEdges = unsharedEdgesFinder.Find();

			foreach (Vector2Int edge in unsharedEdges)
				ReconnectPoints(index, edge);
		}

		private bool IsInView(Vector3 point, Triangle triangle)
		{
			Vector3 pointToTriangleCorner = _vertices[triangle.VertexIndices[0]] - point;
			float dotProduct = Vector3.Dot(triangle.Normal, pointToTriangleCorner);
			return dotProduct < 0;
		}

		private void ReconnectPoints(int pointIndex, Vector2Int edge)
		{
			Vector3Int indices = new Vector3Int(edge[0], edge[1], pointIndex);
			Result.Add(CreateTriangle(indices));
		}

		private List<Triangle> CreateInitialTetrahedron()
		{
			Vector3 v0 = _vertices[0];
			int i1 = FindFarthestAwayFrom(new Vector3[]{ v0});
			Vector3 v1 = _vertices[i1];
			int i2 = FindFarthestAwayFrom(new Vector3[] { v0, v1});
			Vector3 v2 = _vertices[i2];
			int i3 = FindFarthestAwayFrom(new Vector3[] { v0, v1, v2});
			Vector3 v3 = _vertices[i3];
			Vector3 tetrahedronCenter = (v0 + v1 + v2 + v3) / 4;
			Triangle t0 = CreateTriangle(new Vector3Int(0, i1, i2), tetrahedronCenter);
			Triangle t1 = CreateTriangle(new Vector3Int(0, i1, i3), tetrahedronCenter);
			Triangle t2 = CreateTriangle(new Vector3Int(i1, i2, i3), tetrahedronCenter);
			Triangle t3 = CreateTriangle(new Vector3Int(i2, 0, i3), tetrahedronCenter);
			return new List<Triangle> { t0, t1, t2, t3 };
		}

		private int FindFarthestAwayFrom(Vector3[] targetPoints)
		{
			float combinedDistance = 0;
			int result = -1;

			for (int i = 0; i < _vertices.Count; i++)
			{
				for (int j = 0; j < targetPoints.Length; j++)
				{
					if (targetPoints[j] == _vertices[i])
						continue;
				}

				float currentCombinedDistance = 0;
				for (int j = 0; j < targetPoints.Length; j++)
					currentCombinedDistance += (targetPoints[j] - _vertices[i]).magnitude;
				if(currentCombinedDistance > combinedDistance)
				{
					combinedDistance = currentCombinedDistance;
					result = i;
				}

			}

			return result;
		}

		private Triangle CreateTriangle(Vector3Int vertexIndices)
		{
			Vector3 normal = CalculateNormal(vertexIndices);
			Vector3 circumcenter = CalculateCircumcenter(vertexIndices);
			return new Triangle(new int[] {vertexIndices.x, vertexIndices.y, vertexIndices.z }, normal, circumcenter);
		}

		private Triangle CreateTriangle(Vector3Int vertexIndices, Vector3 center)
		{
			Vector3 normal = CalculateNormal(vertexIndices);
			Vector3 centerToPoint = _vertices[vertexIndices[0]] - center;
			if (Vector3.Dot(centerToPoint, normal) < 0)
				return CreateTriangle(new Vector3Int(vertexIndices[0], vertexIndices[2], vertexIndices[1]), center);
			return CreateTriangle(vertexIndices);
		}

		private Vector3 CalculateNormal(Vector3Int vertexIndices)
		{
			Vector3 xy = _vertices[vertexIndices[1]] - _vertices[vertexIndices[0]];
			Vector3 xz = _vertices[vertexIndices[2]] - _vertices[vertexIndices[0]];
			Vector3 normal = Vector3.Cross(xy, xz).normalized;
			return normal;
		}

		/// <summary>
		/// Source: https://gamedev.stackexchange.com/questions/60630/how-do-i-find-the-circumcenter-of-a-triangle-in-3d
		/// </summary>
		/// <param name="vertexIndices"></param>
		/// <param name="vertices"></param>
		/// <returns></returns>
		private Vector3 CalculateCircumcenter(Vector3Int vertexIndices)
		{
			Vector3 v0 = _vertices[vertexIndices[0]];
			Vector3 v1 = _vertices[vertexIndices[1]];
			Vector3 v2 = _vertices[vertexIndices[2]];

			Vector3 v02 = v2 - v0;
			Vector3 v01 = v1 - v0;
			Vector3 v01Xv02 = Vector3.Cross(v01, v02);

			Vector3 toCircumsphereCenter = (Vector3.Cross(v01Xv02, v01) * v02.sqrMagnitude + Vector3.Cross(v02, v01Xv02) * v01.sqrMagnitude) / (2.0f * v01Xv02.sqrMagnitude);
			return v0 + toCircumsphereCenter;
		}

		private void ValidateVertices(IList<Vector3> vertices)
		{
			if (vertices.Count < 4)
				throw new ArgumentException();
		}
	}
}