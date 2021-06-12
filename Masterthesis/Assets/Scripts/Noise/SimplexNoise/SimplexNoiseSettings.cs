
using UnityEngine;

namespace SBaier.Master
{
	[CreateAssetMenu(fileName = "SimplexNoiseSettings", menuName = "Noise/SimplexNoiseSettings")]
	public class SimplexNoiseSettings : NoiseSettings
	{
		public override NoiseType GetNoiseType()
		{
			return NoiseType.Simplex;
		}
	}
}