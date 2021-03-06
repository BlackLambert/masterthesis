using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    [CreateAssetMenu(fileName = "LiquidPlanetLayerMaterial", menuName = "Planet/LiquidPlanetLayerMaterial")]
    public class LiquidPlanetLayerMaterialSettings : PlanetLayerMaterialSettings
    {
        [SerializeField]
        private Gradient _baseGradient = new Gradient();
        public Gradient BaseGradient => _baseGradient;

        [SerializeField]
        private Color _depthColor;
        public Color DepthColor => _depthColor;

		public override PlanetMaterialState State => PlanetMaterialState.Liquid;
		public override PlanetMaterialType Type => PlanetMaterialType.Liquid;
	}
}