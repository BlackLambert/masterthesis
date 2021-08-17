using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public abstract class EdgesFinder 
    {
		protected Polygon[] _polygons;
		protected List<Vector2Int> _edges;

		public EdgesFinder()
		{
			Init(new Polygon[0]);
			_edges = new List<Vector2Int>();
		}

        public EdgesFinder(Polygon[] polygons)
		{
			Init(polygons);
            _edges = new List<Vector2Int>();
        }

		public void Init(Polygon[] polygons)
		{
			_polygons = polygons;
		}

		public Vector2Int[] Find()
		{
			_edges.Clear();
			FindEdges();
			return _edges.ToArray();
		}

		protected abstract void FindEdges();
	}
}