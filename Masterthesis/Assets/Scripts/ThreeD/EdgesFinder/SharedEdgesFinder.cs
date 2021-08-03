using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
	public class SharedEdgesFinder : EdgesFinder
	{
		public SharedEdgesFinder(IList<Polygon> polygons) : base(polygons)
		{
		}

		protected override bool CompareFunction(int polygonIndex, Vector2Int edge)
		{
			return IsSharedEdge(polygonIndex, edge);
		}

		private bool IsSharedEdge(int polygonIndex, Vector2Int edge)
		{
			for (int k = 0; k < _polygons.Count; k++)
			{
				Vector2Int reverse = new Vector2Int(edge.y, edge.x);
				if (Edges.Contains(edge) || Edges.Contains(reverse))
					return false;
				if (HasSharedEdge(polygonIndex, k, edge))
					return true;
			}
			return false;
		}
	}
}