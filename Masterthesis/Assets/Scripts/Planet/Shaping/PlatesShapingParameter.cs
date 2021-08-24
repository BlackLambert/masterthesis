using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public class PlatesShapingParameter
    {
        public PlatesShapingParameter(
            float mountainsBreadthFactor,
			float mountainsBlendDistanceFactor,
			float mountainMin,
			float mountainMinBreadth,
			float mountainHeightFactor,
			float canyonsBreadthFactor,
			float canyonsBlendDistanceFactor,
			float canyonMin,
			float canyonMinBreadth,
			float canyonDepthFactor)
		{
			MountainsBreadthFactor = mountainsBreadthFactor;
			MountainsBlendDistanceFactor = mountainsBlendDistanceFactor;
			MountainMin = mountainMin;
			MountainMinBreadth = mountainMinBreadth;
			MountainHeightFactor = mountainHeightFactor;
			CanyonsBreadthFactor = canyonsBreadthFactor;
			CanyonsBlendDistanceFactor = canyonsBlendDistanceFactor;
			CanyonMin = canyonMin;
			CanyonMinBreadth = canyonMinBreadth;
			CanyonDepthFactor = canyonDepthFactor;
		}

		public float MountainsBreadthFactor { get; }
		public float MountainsBlendDistanceFactor { get; }
		public float MountainMin { get; }
		public float MountainMinBreadth { get; }
		public float MountainHeightFactor { get; }
		public float CanyonsBreadthFactor { get; }
		public float CanyonsBlendDistanceFactor { get; }
		public float CanyonMin { get; }
		public float CanyonMinBreadth { get; }
		public float CanyonDepthFactor { get; }
	}
}