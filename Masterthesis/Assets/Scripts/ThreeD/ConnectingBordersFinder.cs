using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public class ConnectingBordersFinder
	{
		private IList<Polygon> _polygons;

		public ConnectingBordersFinder(IList<Polygon> polygons)
		{
			_polygons = polygons;
		}

		public Dictionary<Vector2Int, Vector2Int[]> Find()
		{
			Dictionary<Vector2Int, Vector2Int[]> result = new Dictionary<Vector2Int, Vector2Int[]>();
			for (int i = 0; i < _polygons.Count; i++)
				Find(result, i);
			return result;
		}

		private void Find(Dictionary<Vector2Int, Vector2Int[]> result, int plate0Index)
		{
			for (int j = plate0Index + 1; j < _polygons.Count; j++)
				Find(result, plate0Index, j);
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
			EdgesFinder finder = new UnsharedEdgesFinder(polygons);
			finder.Find();
			Vector2Int[] borders = finder.Edges.ToArray();
			return borders;
		}
	}
}