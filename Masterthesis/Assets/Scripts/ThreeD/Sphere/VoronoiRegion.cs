using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public class VoronoiRegion : Polygon
    {
        public Vector3 Site { get; }
        public int[] Corners { get; }
        public int[] Neighbors { get; private set; }

		public override IList<int> VertexIndices => Corners;

		public VoronoiRegion(Vector3 site, int[] corners)
		{
            Site = site;
            Corners = corners;
            Neighbors = new int[0];
        }

        public void SetNeighbors(int[] neighbors)
		{
            Neighbors = neighbors;
        }
    }
}