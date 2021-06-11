using System.Collections;
using System.Collections.Generic;
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
		private double _startFrequency = 1;
		public double StartFrequency => _startFrequency;
		[SerializeField]
		private double _startWeight = 1;
		public double StartWeight => _startWeight;

		public override NoiseType GetNoiseType()
		{
			return NoiseType.Octave;
		}
	}
}