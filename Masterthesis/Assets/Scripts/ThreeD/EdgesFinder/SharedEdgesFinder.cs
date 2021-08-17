using UnityEngine;

namespace SBaier.Master
{
	public class SharedEdgesFinder : EdgesFinder
	{
		public SharedEdgesFinder() : base() { }
		public SharedEdgesFinder(Polygon[] polygons) : base(polygons) { }

		protected override void FindEdges()
		{
			int polygonsCount = _polygons.Length;
			for (int i = 0; i < polygonsCount; i++)
				FindSharedEdges(i);
		}

		private void FindSharedEdges(int polygonIndex)
		{
			Polygon polygon = _polygons[polygonIndex];
			int cornersCount = polygon.VertexIndices.Length;
			for (int i = 0; i < cornersCount; i++)
				FindSharedEdges(polygonIndex, i);
		}

		private void FindSharedEdges(int polygonIndex, int edgeIndex)
		{
			Polygon polygon = _polygons[polygonIndex];
			int corner0 = polygon.VertexIndices[edgeIndex];
			int corner1 = polygon.VertexIndices[(edgeIndex + 1) % polygon.VertexIndices.Length];
			Vector2Int edge = new Vector2Int(corner0, corner1);
			if (IsSharedEdge(polygonIndex, edge))
				_edges.Add(edge);
		}

		private bool IsSharedEdge(int polygonIndex, Vector2Int edge)
		{
			int polygonsCount = _polygons.Length;
			for (int i = polygonIndex + 1; i < polygonsCount; i++)
			{
				Polygon polygonToCompare = _polygons[i];
				if (polygonToCompare.HasEdge(edge))
					return true;
			}
			return false;
		}
	}
}