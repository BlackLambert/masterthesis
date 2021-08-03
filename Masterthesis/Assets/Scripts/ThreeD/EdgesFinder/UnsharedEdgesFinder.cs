using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
	public class UnsharedEdgesFinder : EdgesFinder
	{
		public UnsharedEdgesFinder(IList<Polygon> polygons) : base(polygons)
		{
		}

		protected override bool CompareFunction(int polygonIndex, Vector2Int edge)
		{
			return IsUnsharedEdge(polygonIndex, edge);
		}

		private bool IsUnsharedEdge(int polygonIndex, Vector2Int edge)
		{
			for (int k = 0; k < _polygons.Count; k++)
			{
				if (HasSharedEdge(polygonIndex, k, edge))
					return false;
			}
			return true;
		}
	}
}