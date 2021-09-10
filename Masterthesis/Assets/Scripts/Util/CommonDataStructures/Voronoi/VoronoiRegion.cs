using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public class VoronoiRegion : Polygon
    {
        public Vector3 Site { get; }
        public int[] Neighbors { get; private set; }
		public override int[] VertexIndices { get; }

		public VoronoiRegion(Vector3 site, int[] corners, int[] neighbors)
		{
            Site = site;
            VertexIndices = corners;
            Neighbors = neighbors;
        }
    }
}