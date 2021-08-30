using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
	public class IDToPortionSerializer
	{
		protected short Serialize(byte iD, float portion)
		{
			byte bytePortion = (byte)(portion * 100);
			int result = bytePortion;
			result = (result << 8);
			result |= iD;
			return (short)result;
		}

		protected Result Deserialize(short serializedData)
		{
			byte materialId = (byte)(serializedData & 0b_1111_1111);
			int portionInt = serializedData >> 8;
			float portion = portionInt / 100f;
			return new Result(materialId, portion);
		}

		protected struct Result
		{
			public Result(byte iD, float portion)
			{
				ID = iD;
				Portion = portion;
			}

			public byte ID { get; }
			public float Portion { get; }
		}
	}
}

