using UnityEngine;

namespace SBaier.Master
{
	[CreateAssetMenu(fileName = "NoiseLayerSettings", menuName = "Noise/LayerSettings")]
	public class NoiseLayerSettings : ScriptableObject
	{
		[SerializeField]
		private NoiseSettings _noiseSettings;
		public NoiseSettings NoiseSettings => _noiseSettings;

		[SerializeField]
		private double _weight = 1;
		public double Weight => _weight;

		[SerializeField]
		private float _frequencyFactor = 1;
		public float FrequencyFactor => _frequencyFactor;
	}
}