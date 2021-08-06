using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public class ContinentalPlateSegment : Polygon
    {
        public ContinentalPlateSegment(
			VoronoiRegion voronoiRegion,
			int biomeID)
		{
			VoronoiRegion = voronoiRegion;
			BiomeID = biomeID;
		}

		public Vector3 Site => VoronoiRegion.Site;
		public VoronoiRegion VoronoiRegion { get; }
		public int BiomeID { get; }

		public override IList<int> VertexIndices => VoronoiRegion.VertexIndices;
	}
}