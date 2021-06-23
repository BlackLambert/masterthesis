using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
	[CreateAssetMenu(fileName = "NoiseAmplifierSettings", menuName = "Noise/NoiseAmplifierSettings")]
	public class NoiseAmplifierSettings : NoiseSettings
	{
		[SerializeField]
		private NoiseSettings _baseNoise;
		public NoiseSettings BaseNoise => _baseNoise;
		[SerializeField]
		private NoiseSettings _amplifierNoise;
		public NoiseSettings AmplifierNoise => _amplifierNoise;

		public override NoiseType GetNoiseType()
		{
			return NoiseType.Amplifier;
		}
	}
}