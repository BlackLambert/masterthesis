using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
	[CreateAssetMenu(fileName = "RockPlanetLayerMaterialSettings", menuName = "Planet/RockPlanetLayerMaterialSettings")]
	public class RockPlanetLayerMaterialSettings : SolidPlanetLayerMaterialSettings
	{
		public override PlanetMaterialType Type => PlanetMaterialType.Rock;
	}
}