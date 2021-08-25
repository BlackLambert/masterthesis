using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
	public class OceanBiome : Biome
	{
		public OceanBiome(ContinentalRegion.Type regionType, 
			SolidPlanetLayerMaterialSettings rockMaterial,
			SolidPlanetLayerMaterialSettings topSolidMaterial, 
			LiquidPlanetLayerMaterialSettings liquidMaterial, 
			GasPlanetLayerMaterialSettings gasMaterial,
			VegetationPlanetLayerMaterialSettings vegetationSettings) : 
			base(regionType, rockMaterial, topSolidMaterial, liquidMaterial, gasMaterial, vegetationSettings)
		{
		}
	}
}