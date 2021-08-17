using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public abstract class BiomeSettings : ScriptableObject
    {
        [SerializeField]
        private Color _baseColor = Color.white;
        public Color BaseColor => _baseColor;

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