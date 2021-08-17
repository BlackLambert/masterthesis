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

		public VoronoiRegion(Vector3 site, int[] corners)
		{
            Site = site;
            VertexIndices = corners;
            Neighbors = new int[0];
        }

        public void SetNeighbors(int[] neighbors)
		{
            Neighbors = neighbors;
        }
    }
}