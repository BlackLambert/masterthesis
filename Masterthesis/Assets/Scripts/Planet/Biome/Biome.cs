using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public abstract class Biome 
    {
        public Biome(Color baseColor, ContinentalRegion.Type regionType)
		{
			BaseColor = baseColor;
			RegionType = regionType;
		}

		public Color BaseColor { get; }
		public ContinentalRegion.Type RegionType { get; }
	}
}