using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
	[CreateAssetMenu(fileName = "BiomeSettings", menuName = "Biome/OceanBiomeSettings")]
	public class OceanBiomeSettings : BiomeSettings
	{
		public override ContinentalRegion.Type RegionType => ContinentalRegion.Type.Oceanic;
	}
}