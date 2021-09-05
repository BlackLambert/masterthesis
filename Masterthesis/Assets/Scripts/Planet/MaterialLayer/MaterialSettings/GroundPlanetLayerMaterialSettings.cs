using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
	[CreateAssetMenu(fileName = "GroundPlanetLayerMaterialSettings", menuName = "Planet/GroundPlanetLayerMaterialSettings")]
	public class GroundPlanetLayerMaterialSettings : SolidPlanetLayerMaterialSettings
	{
		public override PlanetMaterialType Type => PlanetMaterialType.Ground;
	}
}