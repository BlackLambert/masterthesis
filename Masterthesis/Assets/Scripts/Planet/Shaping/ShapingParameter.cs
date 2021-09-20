using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
	[Serializable]
	public class ShapingParameter
    {
        public ShapingParameter(
            TerrainStructureParameters plates)
		{
			Plates = plates;
		}

		public TerrainStructureParameters Plates { get; }
	}
}