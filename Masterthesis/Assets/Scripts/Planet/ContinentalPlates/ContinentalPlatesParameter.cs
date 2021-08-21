using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public class ContinentalPlatesParameter 
    {
        public ContinentalPlatesParameter(
            int segmentsAmount,
            int continentsAmount,
            int oceansAmount,
            int platesAmount,
			float warpFactor,
			float blendFactor,
			float sampleEliminationFactor,
			float platesMinForce)
		{
			SegmentsAmount = segmentsAmount;
			ContinentsAmount = continentsAmount;
			OceansAmount = oceansAmount;
			PlatesAmount = platesAmount;
			WarpFactor = warpFactor;
			BlendFactor = blendFactor;
			SampleEliminationFactor = sampleEliminationFactor;
			PlatesMinForce = platesMinForce;
		}

		public int SegmentsAmount { get; }
		public int ContinentsAmount { get; }
		public int OceansAmount { get; }
		public int PlatesAmount { get; }
		public float WarpFactor { get; }
		public float BlendFactor { get; }
		public float SampleEliminationFactor { get; }
		public float PlatesMinForce { get; }
	}
}