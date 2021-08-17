using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public class BiomeFactory
    {
        public Biome[] Create(BiomeSettings[] settings)
		{
            Biome[] result = new Biome[settings.Length];
            for (int i = 0; i < settings.Length; i++)
                result[i] = Create(settings[i]);
            return result;
        }

        public Biome Create(BiomeSettings settings)
		{
			if (settings is ContinentBiomeSettings)
				return CreateContinentalBiome(settings as ContinentBiomeSettings);
			else if (settings is OceanBiomeSettings)
				return CreateOceanBiome(settings as OceanBiomeSettings);
			throw new NotImplementedException();
		}

		private Biome CreateContinentalBiome(ContinentBiomeSettings settings)
		{
			return new ContinentalBiome(settings.BaseColor, settings.RegionType, settings.MountainSlopeColor, settings.SlopeThreshold);
		}

		private Biome CreateOceanBiome(OceanBiomeSettings settings)
		{
			return new OceanBiome(settings.BaseColor, settings.RegionType);
		}
	}
}