using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public class PlanetLayerMaterialSerializer
    {
        public short Serialize(PlanetLayerMaterial data)
		{
			byte materialId = data.MaterialID;
			byte portion = (byte) (data.Portion * 100);
			int result = portion;
			result = (result << 8);
			result |= materialId;
			return (short)result;
		}

		public PlanetLayerMaterial Deserialize(short serializedData)
		{
			byte materialId = (byte) (serializedData & 0b_1111_1111);
			int portionInt = serializedData >> 8;
			float portion = portionInt / 100f;
			return new PlanetLayerMaterial(materialId, portion);
		}
    }
}