using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    [CreateAssetMenu(fileName = "PlanetLayerMaterial", menuName = "Planet/PlanetLayerMaterial")]
    public class PlanetLayerMaterial : ScriptableObject
    {
        [Range(0f, 1f)]
        [SerializeField]
        private float _density = 0;
        public float Density => _density;

        [SerializeField]
        private string _name = "New Material";
        public string Name => _name;
	}
}