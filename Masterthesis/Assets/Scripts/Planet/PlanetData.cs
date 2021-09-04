using System;
using System.Collections.Generic;

namespace SBaier.Master
{
    public class PlanetData
    {
        public PlanetDimensions Dimensions { get; }
		public TemperatureSpectrum TemperatureSpectrum { get; }
		public PlanetAxisData PlanetAxis { get; }
        public ContinentalPlates ContinentalPlates { get; set; }
		public Seed Seed { get; }
        public Dictionary<short, PlanetLayerMaterialSettings> Materials { get; }

        public PlanetData(
            PlanetDimensions dimensions,
            TemperatureSpectrum temperatureSpectrum,
            PlanetAxisData planetAxis,
            Seed seed,
            Dictionary<short, PlanetLayerMaterialSettings> materials)
		{
            Dimensions = dimensions;
			TemperatureSpectrum = temperatureSpectrum;
			PlanetAxis = planetAxis;
			Seed = seed;
            Materials = materials;
        }
	}
}