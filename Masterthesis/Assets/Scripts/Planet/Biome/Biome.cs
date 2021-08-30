using System;

namespace SBaier.Master
{
    public abstract class Biome 
    {
        public Biome(ContinentalRegion.Type regionType,
			SolidPlanetLayerMaterialSettings rockMaterial,
			SolidPlanetLayerMaterialSettings groundMaterial,
			LiquidPlanetLayerMaterialSettings liquidMaterial,
			GasPlanetLayerMaterialSettings gasMaterial,
			VegetationPlanetLayerMaterialSettings vegetation)
		{
			RegionType = regionType;
			RockMaterial = rockMaterial;
			GroundMaterial = groundMaterial;
			LiquidMaterial = liquidMaterial;
			GasMaterial = gasMaterial;
			Vegetation = vegetation;
		}

		public ContinentalRegion.Type RegionType { get; }
		public SolidPlanetLayerMaterialSettings RockMaterial { get; }
		public SolidPlanetLayerMaterialSettings GroundMaterial { get; }
		public LiquidPlanetLayerMaterialSettings LiquidMaterial { get; }
		public GasPlanetLayerMaterialSettings GasMaterial { get; }
		public VegetationPlanetLayerMaterialSettings Vegetation { get; }

	}
}