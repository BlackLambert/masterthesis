using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public class CanyonSettings 
    {
		public CanyonSettings(
			float minBreadth,
			float maxBreadth,
			float minDepth,
			float maxDepth,
			float blendvalue)
		{
			MinBreadth = minBreadth;
			MaxBreadth = maxBreadth;
			MinDepth = minDepth;
			MaxDepth = maxDepth;
			Blendvalue = blendvalue;
		}

		public float MinBreadth { get; }
		public float MaxBreadth { get; }
		public float MinDepth { get; }
		public float MaxDepth { get; }
		public float Blendvalue { get; }
	}
}