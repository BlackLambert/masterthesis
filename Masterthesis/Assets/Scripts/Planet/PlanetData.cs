using System;
using System.Collections.Generic;
using UnityEngine;

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
        public uint LayerBitMask { get; private set; }
        private bool[] _layerActive;
        public Noise3D GradientNoise { get; }

        public PlanetData(
            PlanetDimensions dimensions,
            TemperatureSpectrum temperatureSpectrum,
            PlanetAxisData planetAxis,
            Seed seed,
            Dictionary<short, PlanetLayerMaterialSettings> materials,
            Noise3D gradientNoise)
		{
            Dimensions = dimensions;
			TemperatureSpectrum = temperatureSpectrum;
			PlanetAxis = planetAxis;
			Seed = seed;
            Materials = materials;
            GradientNoise = gradientNoise;
        }

        public void SetLayerBitMask(uint value)
		{
            LayerBitMask = value;
            _layerActive = GetLayerActive();
        }

		private bool[] GetLayerActive()
		{
            Array enumValues = Enum.GetValues(typeof(PlanetMaterialType));
            bool[] result = new bool[(int)Mathf.Pow(2, enumValues.Length)];
            foreach (PlanetMaterialType type in enumValues)
                result[(int)type] = (LayerBitMask & (uint)type) > 0;
            return result;
        }

		public bool IsLayerActive(PlanetMaterialType type)
		{
            return _layerActive[(int)type];
        }
	}
}