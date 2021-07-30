using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public struct ContinentalPlateSegment 
    {
        public ContinentalPlateSegment(
            Vector3 site,
            bool oceanic)
		{
			Site = site;
			Oceanic = oceanic;
		}

		public Vector3 Site { get; }
		public bool Oceanic { get; }
	}
}