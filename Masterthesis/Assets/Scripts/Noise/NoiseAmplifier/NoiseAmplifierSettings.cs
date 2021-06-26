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
		[SerializeField]
		private NoiseAmplifier.Mode _mode = NoiseAmplifier.Mode.Linear;
		public NoiseAmplifier.Mode Mode => _mode;

		public override NoiseType GetNoiseType()
		{
			return NoiseType.Amplifier;
		}
	}
}