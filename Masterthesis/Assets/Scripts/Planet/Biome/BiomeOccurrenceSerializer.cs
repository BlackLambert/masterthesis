using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public class BiomeOccurrenceSerializer : IDToPortionSerializer
    {
        public short Serialize(BiomeOccurrence occurrence)
		{
            return Serialize(occurrence.ID, occurrence.Portion);
		}

        public new BiomeOccurrence Deserialize(short serializedData)
		{
            Result r = base.Deserialize(serializedData);
            return new BiomeOccurrence(r.ID, r.Portion);
		}
    }
}