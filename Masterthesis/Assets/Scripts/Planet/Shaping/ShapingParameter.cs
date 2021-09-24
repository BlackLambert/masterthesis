using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
	[Serializable]
	[JsonObject(MemberSerialization.OptIn)]
	public class ShapingParameter
    {
        public ShapingParameter(
            TerrainStructureParameters plates)
		{
			Plates = plates;
		}

		[JsonProperty("plates")]
		public TerrainStructureParameters Plates { get; }
	}
}