using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SBaier.Master
{
    public class SphericalVoronoiCalculator
    {
		private readonly Triangle[] _delaunayTriangles;
		private readonly Vector3[] _sites;
		private readonly float _radius;

		public SphericalVoronoiCalculator(Triangle[] delaunayTriangles,
			Vector3[] sites, float radius)
		{
			_delaunayTriangles = delaunayTriangles;
			_sites = sites;
			_radius = radius;
		}

        public VoronoiDiagram CalculateVoronoiDiagram()
		{
			Vector3[] vertices = GetVoronoiVertices(_delaunayTriangles);
			int sitesCount = _sites.Length;
			VoronoiRegion[] regions = new VoronoiRegion[sitesCount];
			for (int i = 0; i < sitesCount; i++)
				regions[i] = CreateRegion(i);
            return new VoronoiDiagram(regions, vertices);
		}

		private Vector3[] GetVoronoiVertices(Triangle[] delaunayTriangles)
		{
			int count = delaunayTriangles.Length;
			Vector3[] result = new Vector3[count];
			for (int i = 0; i < count; i++)
				result[i] = CaculateVetexPosition(delaunayTriangles[i]);
			return result;
		}

		private Vector3 CaculateVetexPosition(Triangle triangle)
		{
			return triangle.CircumcenterCenter.normalized * _radius;
		}

		private VoronoiRegion CreateRegion(int siteIndex)
		{
			List<int> connectedTriangles = GetConnectedTriangles(siteIndex);
			return Create(connectedTriangles, siteIndex);
		}

		private List<int> GetConnectedTriangles(int siteIndex)
		{
			List<int> result = new List<int>();
			for (int j = 0; j < _delaunayTriangles.Length; j++)
				CheckAddTriangle(siteIndex, j, ref result);
			return result;
		}

		private void CheckAddTriangle(int siteIndex, int triangleIndex, ref List<int> result)
		{
			if (TriangleHasCorner(triangleIndex, siteIndex))
				result.Add(triangleIndex);
		}

		private bool TriangleHasCorner(int triangleIndex, int siteIndex)
		{
			Triangle triangle = _delaunayTriangles[triangleIndex];
			return triangle.HasCorner(siteIndex);
		}

		private VoronoiRegion Create(List<int> connectedTriangles, int siteIndex)
		{
			Vector3 site = _sites[siteIndex];
			int[] corners = new int[connectedTriangles.Count];
			List<int> voronoiNeighbors = new List<int>();
			List<Vector3> voronoiNeighborSites = new List<Vector3>();
			Triangle current = _delaunayTriangles[connectedTriangles[0]];
			Vector2Int nextEdge = GetNextEdge(current, siteIndex);
			int index = 0;
			for (int i = 0; i < connectedTriangles.Count; i++)
			{
				corners[i] = connectedTriangles[index];
				for (int j = 0; j < current.VertexIndices.Length; j++)
				{
					int corner = current.VertexIndices[j];
					if (corner == siteIndex || voronoiNeighbors.Contains(corner))
						continue;
					voronoiNeighbors.Add(corner);
					voronoiNeighborSites.Add(_sites[corner]);
				}
				nextEdge = GetNextEdge(current, siteIndex, nextEdge);
				index = FindNeighbor(nextEdge, connectedTriangles, index);
				current = _delaunayTriangles[connectedTriangles[index]];
			}
			return new VoronoiRegion(site, corners, voronoiNeighbors.ToArray(), voronoiNeighborSites.ToArray());
		}

		private Vector2Int GetNextEdge(Triangle current, int siteIndex)
		{
			return GetNextEdge(current, siteIndex, new Vector2Int(-1, -1));
		}

		private Vector2Int GetNextEdge(Triangle triangle, int siteIndex, Vector2Int formerEdge)
		{
			for (int i = 0; i < triangle.VertexIndices.Length; i++)
			{
				Vector2Int edge = new Vector2Int(triangle.VertexIndices[i], triangle.VertexIndices[(i + 1)%3]);
				if (edge[0] == formerEdge[0] && edge[1] == formerEdge[1] || edge[0] == formerEdge[1] && edge[1] == formerEdge[0])
					continue;
				if (edge[0] == siteIndex || edge[1] == siteIndex)
					return edge;
			}
			throw new ArgumentException();
		}

		private int FindNeighbor(Vector2Int nextEdge, List<int> neighborTriangles, int triangleIndex)
		{
			for (int i = 0; i < neighborTriangles.Count; i++)
			{
				if (i == triangleIndex)
					continue;

				Triangle triangle = _delaunayTriangles[neighborTriangles[i]];
				for (int j = 0; j < 3; j++)
				{
					Vector2Int edge = new Vector2Int(triangle.VertexIndices[j], triangle.VertexIndices[(j + 1) % 3]);
					if (edge[0] == nextEdge[0] && edge[1] == nextEdge[1] || edge[0] == nextEdge[1] && edge[1] == nextEdge[0])
						return i;
				}
			}
			throw new ArgumentException();
		}
	}
}