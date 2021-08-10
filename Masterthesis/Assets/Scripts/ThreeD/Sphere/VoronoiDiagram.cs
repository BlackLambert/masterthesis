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
			Vector3 pointOnBorder = GetNearestBorderPointOf(point, regionIndex);
			Vector3 distance = point.FastSubstract(pointOnBorder);
			return distance.magnitude;
		}

		public Vector3 GetNearestBorderPointOf(Vector3 point, int regionIndex)
		{
			VoronoiRegion region = Regions[regionIndex];
			float minDistance = float.MaxValue;
			Vector3 pointOnBorder = Vector3.zero;
			int cornersAmount = region.Corners.Length;
			for (int i = 0; i < cornersAmount; i++)
			{
				Vector3 current = GetNearestBorderPointOf(point, region, i);
				Vector3 distanceVector = point.FastSubstract(current);
				float distance = distanceVector.magnitude;
				if (distance < minDistance)
				{
					minDistance = distance;
					pointOnBorder = current;
				}
			}
			return pointOnBorder;
		}
		private Vector3 GetNearestBorderPointOf(Vector3 point, VoronoiRegion region, int i)
		{
			int cornersAmount = region.Corners.Length;
			Vector3 corner0 = Vertices[region.Corners[i]];
			Vector3 corner1 = Vertices[region.Corners[(i + 1) % cornersAmount]];
			Vector3 border = corner1.FastSubstract(corner0);
			Vector3 pointToCorner0 = point.FastSubstract(corner0);
			float t0 = Vector3.Dot(border, pointToCorner0) / Vector3.Dot(border, border);
			if (t0 <= 0)
				return corner0;
			else if (t0 >= 1)
				return corner1;
			else
				return corner0.FastAdd(border.FastMultiply(t0));
		}
	}
}