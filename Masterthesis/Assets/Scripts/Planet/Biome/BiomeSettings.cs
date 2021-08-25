using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public abstract class BiomeSettings : ScriptableObject
    {
        [SerializeField]
        private SolidPlanetLayerMaterialSettings _rockMaterial;
        public SolidPlanetLayerMaterialSettings RockMaterial => _rockMaterial;
        [SerializeField]
        private SolidPlanetLayerMaterialSettings _topSolidMaterial;
        public SolidPlanetLayerMaterialSettings TopSolidMaterial => _topSolidMaterial;
        [SerializeField]
        private LiquidPlanetLayerMaterialSettings _waterMaterial;
        public LiquidPlanetLayerMaterialSettings WaterMaterial => _waterMaterial;
        [SerializeField]
        private GasPlanetLayerMaterialSettings _airMaterial;
        public GasPlanetLayerMaterialSettings AirMaterial => _airMaterial;
        [SerializeField]
        private VegetationPlanetLayerMaterialSettings _groundVegetation;
        public VegetationPlanetLayerMaterialSettings GroundVegetation => _groundVegetation;

        [SerializeField]
        private int _frequency = 1;
        public int Frequency => _frequency;
        public abstract ContinentalRegion.Type RegionType { get; }

        [SerializeField]
        private bool _useTemeratureSpectrum = true;
        public bool UseTemeratureSpectrum => _useTemeratureSpectrum;

        [SerializeField]
        private Vector2 _temperatureSpecturm = new Vector2(0, 30);
        public Vector2 TemperatureSpecturm => _temperatureSpecturm;
    }
}