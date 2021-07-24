using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public class PlanetLayerData
    {
        public float Height { get; set; }
        public int MaterialIndex { get; set; }
        
        public PlanetLayerData(
            int materialIndex,
            float height)
		{
            MaterialIndex = materialIndex;
            Height = height;
        }
    }
}