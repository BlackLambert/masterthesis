using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public struct BiomeOccurrence 
    {
		public BiomeOccurrence(byte iD, float portion)
		{
			ID = iD;
			Portion = portion;
		}

		public byte ID { get; }
		public float Portion { get; }
	}
}