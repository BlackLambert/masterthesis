using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public class PlanetLayerMaterialSerializer : IDToPortionSerializer
    {
        public short Serialize(PlanetLayerMaterial data)
		{
			return Serialize(data.MaterialID, data.Portion);
		}

		public new PlanetLayerMaterial Deserialize(short serializedData)
		{
			Result r = base.Deserialize(serializedData);
			return new PlanetLayerMaterial(r.ID, r.Portion);
		}
    }
}