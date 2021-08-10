using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
	public class VoronoiDiagram
	{
		public VoronoiDiagram(VoronoiRegion[] regions, Vector3[] vertices)
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

		public float GetDistanceTo(Vector3 point, int regionIndex)
		{
			return (point - GetNearestBorderPointOf(point, regionIndex)).magnitude;
		}

		public Vector3 GetNearestBorderPointOf(Vector3 point, int regionIndex)
		{
			VoronoiRegion region = Regions[regionIndex];
			float minDistance = float.MaxValue;
			Vector3 pointOnBorder = Vector3.zero;
			int cornersAmount = region.Corners.Length;
			for (int i = 0; i < cornersAmount; i++)
			{
				Vector3 p = GetNearestBorderPointOf(point, region, i);
				float distance = (point - p).magnitude;
				if (distance < minDistance)
				{
					minDistance = distance;
					pointOnBorder = p;
				}
			}
			return pointOnBorder;
		}
		private Vector3 GetNearestBorderPointOf(Vector3 point, VoronoiRegion region, int i)
		{
			int cornersAmount = region.Corners.Length;
			Vector3 corner0 = Vertices[region.Corners[i]];
			Vector3 corner1 = Vertices[region.Corners[(i + 1) % cornersAmount]];
			Vector3 border = corner1 - corner0;
			Vector3 pointToCorner0 = point - corner0;
			float t0 = Vector3.Dot(border, pointToCorner0) / Vector3.Dot(border, border);
			if (t0 <= 0)
				return corner0;
			else if (t0 >= 1)
				return corner1;
			else
				return corner0 + t0 * border;
		}
	}
}