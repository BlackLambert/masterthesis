using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
	public class ContinentalPlates
	{
		public ContinentalPlates(
			ContinentalPlateSegment[] segments,
			ContinentalPlate[] plates,
			Vector3[] segmentSites,
			Dictionary<Vector2Int, Vector2Int[]> plateBorders,
			VoronoiDiagram segmentsVoronoi,
			Triangle[] segmentsDelaunayTriangles)
		{
			Segments = segments;
			Plates = plates;
			SegmentSites = segmentSites;
			PlateBorders = plateBorders;
			SegmentsVoronoi = segmentsVoronoi;
			SegmentsDelaunayTriangles = segmentsDelaunayTriangles;
		}

		public ContinentalPlateSegment[] Segments { get; }
		public ContinentalPlate[] Plates { get; }
		public Vector3[] SegmentSites { get; }
		public Dictionary<Vector2Int, Vector2Int[]> PlateBorders { get; }
		public VoronoiDiagram SegmentsVoronoi { get; }
		public Triangle[] SegmentsDelaunayTriangles { get; }
	}
}