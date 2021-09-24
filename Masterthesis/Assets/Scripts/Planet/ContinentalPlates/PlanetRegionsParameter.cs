using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
	[Serializable]
	[JsonObject(MemberSerialization.OptIn)]
	public class PlanetRegionsParameter 
    {
        public PlanetRegionsParameter(
            int segmentsAmount,
            int continentsAmount,
            int oceansAmount,
            int platesAmount,
			float warpFactor,
			int warpLayers,
			float blendFactor,
			float sampleEliminationFactor,
			float platesMinForce)
		{
			SegmentsAmount = segmentsAmount;
			ContinentsAmount = continentsAmount;
			OceansAmount = oceansAmount;
			PlatesAmount = platesAmount;
			WarpFactor = warpFactor;
			WarpLayers = warpLayers;
			BlendFactor = blendFactor;
			SampleEliminationFactor = sampleEliminationFactor;
			PlatesMinForce = platesMinForce;
		}

		[JsonProperty("segmentsAmount")]
		public int SegmentsAmount { get; }

		[JsonProperty("continentsAmount")]
		public int ContinentsAmount { get; }

		[JsonProperty("oceansAmount")]
		public int OceansAmount { get; }

		[JsonProperty("platesAmount")]
		public int PlatesAmount { get; }

		[JsonProperty("warpFactor")]
		public float WarpFactor { get; }

		[JsonProperty("warpLayers")]
		public int WarpLayers { get; }

		[JsonProperty("blendFactor")]
		public float BlendFactor { get; }

		[JsonProperty("sampleEliminationFactor")]
		public float SampleEliminationFactor { get; }

		[JsonProperty("platesMinForce")]
		public float PlatesMinForce { get; }
	}
}