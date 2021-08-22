using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public abstract class PlanetLayerMaterialSettings : ScriptableObject
    {
        [SerializeField]
        private int _iD = 0;
        public int ID => _iD;


        [SerializeField]
        private string _name = "New Material";
        public string Name => _name;

        [Range(0, 1)]
        [SerializeField]
        private float _density = 1;
        public float Density => _density;

        public abstract PlanetLayerMaterialState State { get; }
    }
}