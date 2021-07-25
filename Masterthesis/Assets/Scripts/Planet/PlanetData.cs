using System;

namespace SBaier.Master
{
    public class PlanetData
    {
        public PlanetDimensions Dimensions { get; }
		public TemperatureSpectrum TemperatureSpectrum { get; }
		public PlanetAxisData PlanetAxis { get; }
		public Seed Seed { get; }

        public PlanetData(
            PlanetDimensions dimensions,
            TemperatureSpectrum temperatureSpectrum,
            PlanetAxisData planetAxis,
            Seed seed)
		{
            Dimensions = dimensions;
			TemperatureSpectrum = temperatureSpectrum;
			PlanetAxis = planetAxis;
			Seed = seed;
        }
	}
}