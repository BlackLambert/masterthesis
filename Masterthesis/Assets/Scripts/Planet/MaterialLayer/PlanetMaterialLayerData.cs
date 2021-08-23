using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public class PlanetMaterialLayerData
    {
        public float Height { get;  set; }
        public PlanetMaterialState State { get; }
        public List<short> Materials { get; }
        
        public PlanetMaterialLayerData(
            List<short> materials,
            PlanetMaterialState state,
            float height)
		{
            Materials = materials;
            State = state;
            Height = height;
        }
    }
}