using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public abstract class SolidPlanetLayerMaterialSettings : PlanetLayerMaterialSettings
    {
        [SerializeField]
        private Gradient _baseGradient = new Gradient();
        public Gradient BaseGradient => _baseGradient;


        public override PlanetMaterialState State => PlanetMaterialState.Solid;
    }
}