using UnityEngine;

namespace SBaier.Master
{
	[CreateAssetMenu(fileName = "OctaveSettings", menuName = "Noise/OctaveSettings")]
	public class OctaveSettings : ScriptableObject
	{
		[SerializeField]
		private NoiseSettings _noiseSettings;
		public NoiseSettings NoiseSettings => _noiseSettings;

		[SerializeField]
		private double _amplitude = 1;
		public double Amplitude => _amplitude;

		[SerializeField]
		private double _frequencyFactor = 1;
		public double FrequencyFactor => _frequencyFactor;
	}
}