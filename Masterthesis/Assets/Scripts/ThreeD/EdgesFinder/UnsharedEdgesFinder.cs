using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
	public class UnsharedEdgesFinder : EdgesFinder
	{
		public UnsharedEdgesFinder(Polygon[] polygons) : base(polygons)
		{
		}

		protected override void FindEdges()
		{
			int polygonsCount = _polygons.Length;
			for (int i = 0; i < polygonsCount; i++)
				FindUnsharedEdges(i);
		}

		private void FindUnsharedEdges(int polygonIndex)
		{
			Polygon polygon = _polygons[polygonIndex];
			int cornersCount = polygon.VertexIndices.Length;
			for (int i = 0; i < cornersCount; i++)
				FindUnsharedEdges(polygonIndex, i);
		}

		private void FindUnsharedEdges(int polygonIndex, int edgeIndex)
		{
			Polygon polygon = _polygons[polygonIndex];
			int corner0 = polygon.VertexIndices[edgeIndex];
			int corner1 = polygon.VertexIndices[(edgeIndex + 1) % polygon.VertexIndices.Length];
			Vector2Int edge = new Vector2Int(corner0, corner1);
			if (IsUnsharedEdge(polygonIndex, edge))
				_edges.Add(edge);
		}

		private bool IsUnsharedEdge(int polygonIndex, Vector2Int edge)
		{
			for (int i = 0; i < _polygons.Length; i++)
			{
				if (IsSharedEdge(polygonIndex, i, edge))
					return false;
			}
			return true;
		}

		private bool IsSharedEdge(int polygonIndex, int comparePolygonIndex, Vector2Int edge)
		{
			if (_polygons.Length == 1)
				return false;
			if (polygonIndex == comparePolygonIndex)
				return false;
			Polygon comparePolygon = _polygons[comparePolygonIndex];
			return comparePolygon.HasEdge(edge);
		}
	}
}