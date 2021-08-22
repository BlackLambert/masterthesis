using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    [CreateAssetMenu(fileName = "GasPlanetLayerMaterial", menuName = "Planet/GasPlanetLayerMaterial")]
    public class GasPlanetLayerMaterialSettings : PlanetLayerMaterialSettings
    {
        public override PlanetLayerMaterialState State => PlanetLayerMaterialState.Gas;
    }
}