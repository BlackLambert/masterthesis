using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public class ConnectingBordersFinder
	{
		private IList<Polygon> _polygons;
		public Dictionary<Vector2Int, Vector2Int[]> NeighborsToBorders { get; private set; }
		public Vector2Int[] Neighbors { get; private set; }

		public ConnectingBordersFinder(IList<Polygon> polygons)
		{
			_polygons = polygons;
		}

		public void Calcualte()
		{
			Dictionary<Vector2Int, Vector2Int[]> result = new Dictionary<Vector2Int, Vector2Int[]>();
			for (int i = 0; i < _polygons.Count; i++)
				Find(result, i);
			NeighborsToBorders = result;
			Neighbors = FindNeighbors(result);
		}

		private void Find(Dictionary<Vector2Int, Vector2Int[]> result, int polygonIndex)
		{
			for (int j = polygonIndex + 1; j < _polygons.Count; j++)
				Find(result, polygonIndex, j);
		}

		private void Find(Dictionary<Vector2Int, Vector2Int[]> result, int poly0Index, int poly1Index)
		{
			Vector2Int polyBorderIJ = new Vector2Int(poly0Index, poly1Index);
			Polygon[] polygones = new Polygon[] { _polygons[poly0Index], _polygons[poly1Index] };
			Vector2Int[] edges = GetBorders(polygones);
			result.Add(polyBorderIJ, edges);
		}

		private static Vector2Int[] GetBorders(IList<Polygon> polygons)
		{
			EdgesFinder finder = new SharedEdgesFinder(polygons);
			finder.Find();
			Vector2Int[] borders = finder.Edges.ToArray();
			return borders;
		}

		private Vector2Int[] FindNeighbors(Dictionary<Vector2Int, Vector2Int[]> plateBorders)
		{
			List<Vector2Int> result = new List<Vector2Int>();
			foreach (KeyValuePair<Vector2Int, Vector2Int[]> pair in plateBorders)
			{
				if (pair.Value.Length == 0)
					continue;
				result.Add(pair.Key);
			}
			return result.ToArray();
		}
	}
}