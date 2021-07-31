using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public class ContinentalPlateSegment : Polygon
    {
        public ContinentalPlateSegment(
			VoronoiRegion voronoiRegion,
            bool oceanic)
		{
			Region = voronoiRegion;
			Oceanic = oceanic;
		}

		public Vector3 Site => Region.Site;
		public VoronoiRegion Region { get; }
		public bool Oceanic { get; }

		public override IList<int> VertexIndices => Region.VertexIndices;
	}
}