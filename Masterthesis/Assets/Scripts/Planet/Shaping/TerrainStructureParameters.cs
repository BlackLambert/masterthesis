using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
	[Serializable]
	[JsonObject(MemberSerialization.OptIn)]
	public class TerrainStructureParameters
    {
        public TerrainStructureParameters(
			MountainSettings mountain,
			CanyonSettings canyon)
		{
			Mountain = mountain;
			Canyon = canyon;
		}


		[JsonProperty("mountain")]
		public MountainSettings Mountain { get; }

		[JsonProperty("canyon")]
		public CanyonSettings Canyon { get; }
	}
}