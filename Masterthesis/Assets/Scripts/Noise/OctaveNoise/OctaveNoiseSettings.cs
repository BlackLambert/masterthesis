
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
		[SerializeField]
		private float _startFrequency = 1;
		public float StartFrequency => _startFrequency;
		[SerializeField]
		private float _startWeight = 1;
		public float StartWeight => _startWeight;

		public override NoiseType GetNoiseType()
		{
			return NoiseType.Octave;
		}
	}
}