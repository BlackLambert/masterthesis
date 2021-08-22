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
			float canyonsBreadthFactor,
			float canyonsBlendDistanceFactor,
			float canyonMin,
			float canyonMinBreadth)
		{
			MountainsBreadthFactor = mountainsBreadthFactor;
			MountainsBlendDistanceFactor = mountainsBlendDistanceFactor;
			MountainMin = mountainMin;
			MountainMinBreadth = mountainMinBreadth;
			CanyonsBreadthFactor = canyonsBreadthFactor;
			CanyonsBlendDistanceFactor = canyonsBlendDistanceFactor;
			CanyonMin = canyonMin;
			CanyonMinBreadth = canyonMinBreadth;
		}

		public float MountainsBreadthFactor { get; }
		public float MountainsBlendDistanceFactor { get; }
		public float MountainMin { get; }
		public float MountainMinBreadth { get; }
		public float CanyonsBreadthFactor { get; }
		public float CanyonsBlendDistanceFactor { get; }
		public float CanyonMin { get; }
		public float CanyonMinBreadth { get; }
	}
}