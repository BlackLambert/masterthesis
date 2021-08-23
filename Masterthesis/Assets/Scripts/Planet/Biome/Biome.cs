using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public abstract class Biome 
    {
        public Biome(ContinentalRegion.Type regionType,
			SolidPlanetLayerMaterialSettings rockMaterial,
			SolidPlanetLayerMaterialSettings groundMaterial,
			LiquidPlanetLayerMaterialSettings liquidMaterial,
			GasPlanetLayerMaterialSettings gasMaterial)
		{
			RegionType = regionType;
			RockMaterial = rockMaterial;
			GroundMaterial = groundMaterial;
			LiquidMaterial = liquidMaterial;
			GasMaterial = gasMaterial;
		}

		public ContinentalRegion.Type RegionType { get; }
		public SolidPlanetLayerMaterialSettings RockMaterial { get; }
		public SolidPlanetLayerMaterialSettings GroundMaterial { get; }
		public LiquidPlanetLayerMaterialSettings LiquidMaterial { get; }
		public GasPlanetLayerMaterialSettings GasMaterial { get; }

		public PlanetLayerMaterialSettings GetMeterial(int index)
		{
			switch(index)
			{
				case 0:
					return RockMaterial;
				case 1:
					return GroundMaterial;
				case 2:
					return LiquidMaterial;
				case 3:
					return GasMaterial;
				default:
					throw new NotImplementedException();
			}
		}
	}
}