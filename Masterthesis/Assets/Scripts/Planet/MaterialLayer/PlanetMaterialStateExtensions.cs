using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace SBaier.Master
{
    public static class PlanetMaterialStateExtensions
    {
        public static string ToName(this PlanetMaterialState state)
		{
            switch(state)
			{
                case PlanetMaterialState.Gas:
                    return "gasförmig";
                case PlanetMaterialState.Liquid:
                    return "flüssig";
                case PlanetMaterialState.Solid:
                    return "fest";
                default:
                    throw new NotImplementedException();
			}
		}
    }
}