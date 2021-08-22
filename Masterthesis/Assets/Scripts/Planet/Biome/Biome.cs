using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public abstract class Biome 
    {
        public Biome(ContinentalRegion.Type regionType,
			SolidPlanetLayerMaterialSettings rockMaterial,
			SolidPlanetLayerMaterialSettings topSolidMaterial,
			LiquidPlanetLayerMaterialSettings liquidMaterial,
			GasPlanetLayerMaterialSettings gasMaterial)
		{
			RegionType = regionType;
			RockMaterial = rockMaterial;
			TopSolidMaterial = topSolidMaterial;
			LiquidMaterial = liquidMaterial;
			GasMaterial = gasMaterial;
		}

		public ContinentalRegion.Type RegionType { get; }
		public SolidPlanetLayerMaterialSettings RockMaterial { get; }
		public SolidPlanetLayerMaterialSettings TopSolidMaterial { get; }
		public LiquidPlanetLayerMaterialSettings LiquidMaterial { get; }
		public GasPlanetLayerMaterialSettings GasMaterial { get; }
	}
}