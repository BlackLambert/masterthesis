using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SBaier.Master
{
	public class ContinentalPlates
	{
		public ContinentalPlates(
			ContinentalPlateSegment[] segments,
			ContinentalRegion[] regions,
			ContinentalPlate[] plates,
			Dictionary<Vector2Int, Vector2Int[]> plateBorders,
			VoronoiDiagram segmentsVoronoi,
			Triangle[] segmentsDelaunayTriangles, 
			Vector2Int[] plateNeighbors)
		{
			Segments = segments;
			Regions = regions;
			Plates = plates;
			PlateBorders = plateBorders;
			SegmentsVoronoi = segmentsVoronoi;
			SegmentsDelaunayTriangles = segmentsDelaunayTriangles;
			PlateNeighbors = plateNeighbors;
		}

		public ContinentalPlateSegment[] Segments { get; }
		public ContinentalRegion[] Regions { get; }
		public ContinentalPlate[] Plates { get; }
		public Vector3[] SegmentSites => GetSegmentSites();
		public Vector3[] SegmentCorners => SegmentsVoronoi.Vertices;
		public Vector2Int[] PlateNeighbors { get; }
		public Dictionary<Vector2Int, Vector2Int[]> PlateBorders { get; }
		public VoronoiDiagram SegmentsVoronoi { get; }
		public Triangle[] SegmentsDelaunayTriangles { get; }

		private Vector3[] GetSegmentSites()
		{
			return Segments.Select(s => s.Site).ToArray();
		}

		private Vector3[] GetRegionSites()
		{
			return Regions.Select(r => r.Site).ToArray();
		}
	}
}