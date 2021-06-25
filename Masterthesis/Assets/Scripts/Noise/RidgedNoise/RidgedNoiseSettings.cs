using UnityEngine;

namespace SBaier.Master
{
	[CreateAssetMenu(fileName = "RidgedNoiseSettings", menuName = "Noise/RidgedNoiseSettings")]
	public class RidgedNoiseSettings : NoiseSettings
	{
		[SerializeField]
		private NoiseSettings _baseNoiseSettings;
		public NoiseSettings BaseNoiseSettings => _baseNoiseSettings;

		public override NoiseType GetNoiseType()
		{
			return NoiseType.Ridged;
		}
	}
}