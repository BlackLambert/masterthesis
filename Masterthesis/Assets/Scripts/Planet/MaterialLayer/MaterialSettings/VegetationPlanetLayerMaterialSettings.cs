using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    [CreateAssetMenu(fileName = "VegetationPlanetLayerMaterial", menuName = "Planet/VegetationPlanetLayerMaterial")]
    public class VegetationPlanetLayerMaterialSettings : SolidPlanetLayerMaterialSettings
    {
        [SerializeField]
        private PlanetLayerMaterialSettings _groundRequirements;
        public PlanetLayerMaterialSettings GroundRequirements => _groundRequirements;

        [SerializeField]
        private AnimationCurve _heightRequirements;
        public AnimationCurve HeightRequirements => _heightRequirements;

		public override PlanetMaterialType Type => PlanetMaterialType.Vegetation;
	}
}