using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
	[CreateAssetMenu(fileName = "WhiteNoiseSettings", menuName = "Noise/WhiteNoiseSettings")]
	public class WhiteNoiseSettings : NoiseSettings
	{
		public override NoiseType GetNoiseType()
		{
			return NoiseType.White;
		}
	}
}