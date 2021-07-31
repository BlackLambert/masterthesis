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
	}
}