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
                    return "gasf�rmig";
                case PlanetMaterialState.Liquid:
                    return "fl�ssig";
                case PlanetMaterialState.Solid:
                    return "fest";
                default:
                    throw new NotImplementedException();
			}
		}
    }
}