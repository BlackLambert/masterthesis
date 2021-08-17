using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public class ContinentalPlateSegment : Polygon
    {
        public ContinentalPlateSegment(
			VoronoiRegion voronoiRegion)
		{
			VoronoiRegion = voronoiRegion;
		}

		public Vector3 Site => VoronoiRegion.Site;
		public VoronoiRegion VoronoiRegion { get; }
		public int BiomeID { get; set; }
		public int[] Neighbors => VoronoiRegion.Neighbors;

		public override int[] VertexIndices => VoronoiRegion.VertexIndices;
	}
}