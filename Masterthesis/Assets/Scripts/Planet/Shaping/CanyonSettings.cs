using Newtonsoft.Json;
using System;

namespace SBaier.Master
{
	[Serializable]
	[JsonObject(MemberSerialization.OptIn)]
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


		[JsonProperty("canyonMinBreadth")]
		public float MinBreadth { get; }

		[JsonProperty("canyonMaxBreadth")]
		public float MaxBreadth { get; }

		[JsonProperty("canyonMinDepth")]
		public float MinDepth { get; }

		[JsonProperty("canyonMaxDepth")]
		public float MaxDepth { get; }

		[JsonProperty("canyonBlendvalue")]
		public float Blendvalue { get; }
	}
}