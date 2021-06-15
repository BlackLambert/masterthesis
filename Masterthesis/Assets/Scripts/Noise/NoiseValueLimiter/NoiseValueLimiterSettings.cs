using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
	[CreateAssetMenu(fileName = "NoiseValueLimiterSettings", menuName = "Noise/NoiseValueLimiterSettings")]
	public class NoiseValueLimiterSettings : NoiseSettings
	{
		public override NoiseType GetNoiseType()
		{
			return NoiseType.NoiseValueLimiter;
		}

		[SerializeField]
		private Vector2 _valueLimits = new Vector2(0, 1);
		public Vector2 ValueLimits => _valueLimits;

		[SerializeField]
		private NoiseSettings _baseNoise = null;
		public NoiseSettings BaseNoise => _baseNoise;
	}
}