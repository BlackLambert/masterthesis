using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public struct PlanetLayerMaterial 
    {
		public PlanetLayerMaterial(byte materialID, float portion)
		{
			MaterialID = materialID;
			Portion = portion;
		}

		public byte MaterialID { get; }
		public float Portion { get; }
	}
}