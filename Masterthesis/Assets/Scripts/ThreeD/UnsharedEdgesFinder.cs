using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public class UnsharedEdgesFinder 
    {
		private IList<Polygon> _polygons;
        public List<Vector2Int> Result { get; }

        public UnsharedEdgesFinder(IList<Polygon> polygons)
		{
			Init(polygons);
            Result = new List<Vector2Int>();
        }

		public void Init(IList<Polygon> polygons)
		{
			_polygons = polygons;
		}

		public void Calculate()
		{
			Result.Clear();
			for (int i = 0; i < _polygons.Count; i++)
				AddUnsharedEdges(i);
		}

		private void AddUnsharedEdges(int polygonIndex)
		{
			Polygon polygon = _polygons[polygonIndex];
			for (int i = 0; i < polygon.VertexIndices.Count; i++)
				AddUnsharedEdges(polygonIndex, i);
		}

		private void AddUnsharedEdges(int polygonIndex, int edgeIndex)
		{
			Polygon polygon = _polygons[polygonIndex];
			int corner0 = polygon.VertexIndices[edgeIndex];
			int corner1 = polygon.VertexIndices[(edgeIndex + 1) % polygon.VertexIndices.Count];
			Vector2Int edge = new Vector2Int(corner0, corner1);
			bool isShared = false;
			for (int k = 0; k < _polygons.Count; k++)
				isShared = isShared || HasSharedEdge(polygonIndex, k, edge);
			if (!isShared)
				Result.Add(edge);
		}

		private bool HasSharedEdge(int polygonIndex, int comparePolygonIndex, Vector2Int edge)
		{
			if (polygonIndex == comparePolygonIndex)
				return false;
			Polygon polygonToCompare = _polygons[comparePolygonIndex];
			return polygonToCompare.ContainsEdge(edge);
		}
	}
}