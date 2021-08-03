using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public class Biome 
    {
        public Biome(Color baseColor)
		{
			BaseColor = baseColor;
		}

		public Color BaseColor { get; }
	}
}