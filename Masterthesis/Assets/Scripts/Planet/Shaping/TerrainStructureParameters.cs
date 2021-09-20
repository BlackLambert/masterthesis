using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
	[Serializable]
	public class TerrainStructureParameters
    {
        public TerrainStructureParameters(
			MountainSettings mountain,
			CanyonSettings canyon)
		{
			Mountain = mountain;
			Canyon = canyon;
		}

		public MountainSettings Mountain { get; }
		public CanyonSettings Canyon { get; }
	}
}