using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public abstract class PlanetLayerMaterialSettings : ScriptableObject
    {
        [SerializeField]
        private byte _iD = 0;
        public byte ID => _iD;


        [SerializeField]
        private string _name = "New Material";
        public string Name => _name;

        [Range(0, 1)]
        [SerializeField]
        private float _density = 1;
        public float Density => _density;

        [SerializeField]
        private int _frequency = 1;
        public int Frequency => _frequency;

        [SerializeField]
        private Color _imageColor = Color.white;
        public Color ImageColor => _imageColor;

        public abstract PlanetMaterialState State { get; }
        public abstract PlanetMaterialType Type { get; }
    }
}