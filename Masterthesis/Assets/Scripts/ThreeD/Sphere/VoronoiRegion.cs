using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public class VoronoiRegion : Polygon
    {
        public Vector3 Site { get; }
        public int[] _corners;

		public override IList<int> VertexIndices => _corners;

		public VoronoiRegion(Vector3 site, int[] corners)
		{
            Site = site;
            _corners = corners;
		}
    }
}