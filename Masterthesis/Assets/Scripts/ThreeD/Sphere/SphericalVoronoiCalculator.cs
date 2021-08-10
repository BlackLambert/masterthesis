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

		public SphericalVoronoiCalculator(IList<Triangle> delaunayTriangles,
			IList<Vector3> sites)
		{
			this._delaunayTriangles = delaunayTriangles;
			this._sites = sites;
		}

        public VoronoiDiagram CalculateVoronoiDiagram()
		{
			Vector3[] vertices = GetVertices(_delaunayTriangles);
            VoronoiRegion[] regions = new VoronoiRegion[_sites.Count];
			for (int i = 0; i < _sites.Count; i++)
			{
				Vector3 site = _sites[i];
				List<int> neighborTriangles = new List<int>();
				for (int j = 0; j < _delaunayTriangles.Count; j++)
				{
					Triangle triangle = _delaunayTriangles[j];
					if (!triangle.ContainsCorner(i))
						continue;
					neighborTriangles.Add(j);
				}
				regions[i] = Create(site, neighborTriangles, i);
			}
			SetNeighbors(regions);
            return new VoronoiDiagram(regions, vertices);
		}

		private Vector3[] GetVertices(IList<Triangle> delaunayTriangles)
		{
			Vector3[] result = new Vector3[delaunayTriangles.Count];
			for (int i = 0; i < delaunayTriangles.Count; i++)
				result[i] = delaunayTriangles[i].Circumcenter;
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
					if (n[0] == i)
						neighbors.Add(n[1]);
					if (n[1] == i)
						neighbors.Add(n[0]);
				}
				currentRegion.SetNeighbors(neighbors.ToArray());
			}
		}
	}
}