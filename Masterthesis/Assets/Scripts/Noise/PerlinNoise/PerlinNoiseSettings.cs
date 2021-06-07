using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
	[CreateAssetMenu(fileName = "PerlinNoiseSettings", menuName = "Noise/PerlinNoiseSettings")]
	public class PerlinNoiseSettings : NoiseSettings
	{
		public override NoiseType GetNoiseType()
		{
			return NoiseType.Perlin;
		}
	}
}