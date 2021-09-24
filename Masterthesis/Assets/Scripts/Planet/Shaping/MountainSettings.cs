using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
	[Serializable]
	[JsonObject(MemberSerialization.OptIn)]
	public class MountainSettings
    {
		public MountainSettings(
			float minBreadth,
			float maxBreadth,
			float minHeight,
			float maxHeight,
			float blendvalue)
		{
			MinBreadth = minBreadth;
			MaxBreadth = maxBreadth;
			MinHeight = minHeight;
			MaxHeight = maxHeight;
			Blendvalue = blendvalue;
		}


		[JsonProperty("mountainMinBreadth")]
		public float MinBreadth { get; }

		[JsonProperty("mountainMaxBreadth")]
		public float MaxBreadth { get; }

		[JsonProperty("mountainMinHeight")]
		public float MinHeight { get; }

		[JsonProperty("mountainMaxHeight")]
		public float MaxHeight { get; }

		[JsonProperty("mountainBlendvalue")]
		public float Blendvalue { get; }
	}
}