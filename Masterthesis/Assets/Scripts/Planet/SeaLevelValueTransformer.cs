using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public class SeaLevelValueTransformer
    {
		private float _relativeSeaLevel;

		public SeaLevelValueTransformer(float relativeSeaLevel)
		{
            _relativeSeaLevel = relativeSeaLevel;

        }

        public float Revert(float normalizedValue)
		{
			float result = normalizedValue - _relativeSeaLevel;
			if (result < 0)
				result = (result / _relativeSeaLevel) * 0.5f;
			else
				result = (result / (1 - _relativeSeaLevel)) * 0.5f;
			return result + 0.5f;
		}

        public float Transform(float normalizedValue)
		{
			float result = normalizedValue - 0.5f;
			if (result < 0)
				result = (result / 0.5f) * _relativeSeaLevel;
			else
				result = result / 0.5f * (1 - _relativeSeaLevel);
			return result + _relativeSeaLevel;
		}
    }
}