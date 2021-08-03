using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public abstract class EdgesFinder 
    {
		protected IList<Polygon> _polygons;
        public List<Vector2Int> Edges { get; }

        public EdgesFinder(IList<Polygon> polygons)
		{
			Init(polygons);
            Edges = new List<Vector2Int>();
        }

		public void Init(IList<Polygon> polygons)
		{
			_polygons = polygons;
		}

		public void Find()
		{
			FindEdges();
		}

		private void FindEdges()
		{
			Edges.Clear();
			for (int i = 0; i < _polygons.Count; i++)
				AddEdges(i);
		}

		private void AddEdges(int polygonIndex)
		{
			Polygon polygon = _polygons[polygonIndex];
			for (int i = 0; i < polygon.VertexIndices.Count; i++)
				AddEdges(polygonIndex, i);
		}

		private void AddEdges(int polygonIndex, int edgeIndex)
		{
			Polygon polygon = _polygons[polygonIndex];
			int corner0 = polygon.VertexIndices[edgeIndex];
			int corner1 = polygon.VertexIndices[(edgeIndex + 1) % polygon.VertexIndices.Count];
			Vector2Int edge = new Vector2Int(corner0, corner1);
			if (CompareFunction(polygonIndex, edge))
				Edges.Add(edge);
		}
		protected abstract bool CompareFunction(int polygonIndex, Vector2Int edge);


		protected bool HasSharedEdge(int polygonIndex, int comparePolygonIndex, Vector2Int edge)
		{
			if (_polygons.Count <= 1)
				return false;
			if (polygonIndex == comparePolygonIndex)
				return false;
			Polygon polygonToCompare = _polygons[comparePolygonIndex];
			return polygonToCompare.ContainsEdge(edge);
		}
	}
}