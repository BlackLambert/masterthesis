using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public class SphericalVoronoiCalculator
    {
		private readonly IList<Triangle> _delaunayTriangles;
		private readonly IList<Vector3> _sites;
		private readonly float _radius;

		public SphericalVoronoiCalculator(IList<Triangle> delaunayTriangles,
			IList<Vector3> sites, float radius)
		{
			_delaunayTriangles = delaunayTriangles;
			_sites = sites;
			_radius = radius;
		}

        public VoronoiDiagram CalculateVoronoiDiagram()
		{
			Vector3[] vertices = GetVertices(_delaunayTriangles);
            VoronoiRegion[] regions = new VoronoiRegion[_sites.Count];
			for (int i = 0; i < _sites.Count; i++)
				regions[i] = CreateRegion(i);
			SetNeighbors(regions);
            return new VoronoiDiagram(regions, vertices);
		}

		private VoronoiRegion CreateRegion(int siteIndex)
		{
			Vector3 site = _sites[siteIndex];
			List<int> neighborTriangles = new List<int>();
			for (int j = 0; j < _delaunayTriangles.Count; j++)
			{
				if(TriangleHasCorner(j, siteIndex))
					neighborTriangles.Add(j);
			}
			return Create(site, neighborTriangles, siteIndex);
		}

		private bool TriangleHasCorner(int triangleIndex, int siteIndex)
		{
			Triangle triangle = _delaunayTriangles[triangleIndex];
			return triangle.HasCorner(siteIndex);
		}

		private Vector3[] GetVertices(IList<Triangle> delaunayTriangles)
		{
			Vector3[] result = new Vector3[delaunayTriangles.Count];
			for (int i = 0; i < delaunayTriangles.Count; i++)
				result[i] = delaunayTriangles[i].CircumcenterCenter.normalized * _radius;
			return result;
		}

		private VoronoiRegion Create(Vector3 site, List<int> neighborTriangles, int siteIndex)
		{
			int[] corners = new int[neighborTriangles.Count];
			Triangle current = _delaunayTriangles[neighborTriangles[0]];
			Vector2Int nextEdge = GetNextEdge(current, siteIndex);
			int index = 0;
			for (int i = 0; i < neighborTriangles.Count; i++)
			{
				corners[i] = neighborTriangles[index];
				nextEdge = GetNextEdge(current, siteIndex, nextEdge);
				index = FindNeighbor(nextEdge, neighborTriangles, index);
				current = _delaunayTriangles[neighborTriangles[index]];
			}
			return new VoronoiRegion(site, corners);
		}

		private Vector2Int GetNextEdge(Triangle current, int siteIndex)
		{
			return GetNextEdge(current, siteIndex, new Vector2Int(-1, -1));
		}

		private Vector2Int GetNextEdge(Triangle current, int siteIndex, Vector2Int formerEdge)
		{
			for (int i = 0; i < 3; i++)
			{
				Vector2Int edge = new Vector2Int(current.VertexIndices[i], current.VertexIndices[(i + 1)%3]);
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

		private void SetNeighbors(VoronoiRegion[] regions)
		{
			ConnectingBordersFinder finder = new ConnectingBordersFinder(regions);
			finder.Calcualte();
			Vector2Int[] allNeighbors = finder.Neighbors;
			for (int i = 0; i < regions.Length; i++)
			{
				VoronoiRegion currentRegion = regions[i];
				List<int> neighbors = new List<int>();

				for (int j = 0; j < allNeighbors.Length; j++)
				{
					Vector2Int n = allNeighbors[j];
					if (n.x == i)
						neighbors.Add(n.y);
					if (n.y == i)
						neighbors.Add(n.x);
				}
				currentRegion.SetNeighbors(neighbors.ToArray());
			}
		}
	}
}