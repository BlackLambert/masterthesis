using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
	public class VoronoiDiagram : PolygonBody
	{
		public VoronoiDiagram(VoronoiRegion[] regions, Vector3[] vertices):
			base(regions, vertices)
		{
			Regions = regions;
			Vertices = vertices;
		}

		public VoronoiRegion[] Regions { get; }
		public Vector3[] Vertices { get; }


		public VoronoiRegion[] GetNeighborsOf(int regionIndex)
		{
			VoronoiRegion region = Regions[regionIndex];
			int[] neighbors = region.Neighbors;
			VoronoiRegion[] result = new VoronoiRegion[neighbors.Length];
			for (int i = 0; i < neighbors.Length; i++)
				result[i] = Regions[neighbors[i]];
			return result;
		}
	}
}