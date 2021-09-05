using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public class PlanetMaterialLayerData
    {
        public float Height { get;  set; }
        public PlanetMaterialState State { get; }
        public PlanetMaterialType MaterialType { get; }
        public List<short> Materials { get; }
        
        public PlanetMaterialLayerData(
            List<short> materials,
            PlanetMaterialState state,
            PlanetMaterialType materialType,
            float height)
		{
            Materials = materials;
            State = state;
            MaterialType = materialType;
            Height = height;
        }
    }
}