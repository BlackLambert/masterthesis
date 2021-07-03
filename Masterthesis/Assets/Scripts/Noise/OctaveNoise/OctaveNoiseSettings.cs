
using UnityEngine;

namespace SBaier.Master
{
	[CreateAssetMenu(fileName = "OctaveNoiseSettings", menuName = "Noise/OctaveNoiseSettings")]
	public class OctaveNoiseSettings : NoiseSettings
	{
		[SerializeField]
		private int _octavesCount = 3;
		public int OctavesCount => _octavesCount;
		[SerializeField]
		private NoiseSettings _baseNoise;
		public NoiseSettings BaseNoise => _baseNoise;

		public override NoiseType GetNoiseType()
		{
			return NoiseType.Octave;
		}
	}
}