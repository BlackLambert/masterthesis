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
			float canyonsBreadthFactor,
			float canyonsBlendDistanceFactor)
		{
			MountainsBreadthFactor = mountainsBreadthFactor;
			MountainsBlendDistanceFactor = mountainsBlendDistanceFactor;
			CanyonsBreadthFactor = canyonsBreadthFactor;
			CanyonsBlendDistanceFactor = canyonsBlendDistanceFactor;
		}

		public float MountainsBreadthFactor { get; }
		public float MountainsBlendDistanceFactor { get; }
		public float CanyonsBreadthFactor { get; }
		public float CanyonsBlendDistanceFactor { get; }
	}
}