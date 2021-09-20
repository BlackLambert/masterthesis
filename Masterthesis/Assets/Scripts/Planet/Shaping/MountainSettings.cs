using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public class MountainSettings
    {
		public MountainSettings(
			float minBreadth,
			float maxBreadth,
			float minHeight,
			float maxHeight,
			float beldvalue)
		{
			MinBreadth = minBreadth;
			MaxBreadth = maxBreadth;
			MinHeight = minHeight;
			MaxHeight = maxHeight;
			Blendvalue = beldvalue;
		}

		public float MinBreadth { get; }
		public float MaxBreadth { get; }
		public float MinHeight { get; }
		public float MaxHeight { get; }
		public float Blendvalue { get; }
	}
}