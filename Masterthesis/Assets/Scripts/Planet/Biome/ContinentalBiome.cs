using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
	public class ContinentalBiome : Biome
	{
		public ContinentalBiome(
			Color baseColor, 
			ContinentalRegion.Type regionType,
			Color slopeColor,
			float slopeThreshold) : 
			base(baseColor, regionType)
		{
			SlopeColor = slopeColor;
			SlopeThreshold = slopeThreshold;
		}

		public Color SlopeColor { get; }
		public float SlopeThreshold { get; }
	}
}