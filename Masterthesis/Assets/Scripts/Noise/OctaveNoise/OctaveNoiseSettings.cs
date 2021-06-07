using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
	[CreateAssetMenu(fileName = "OctaveNoiseSettings", menuName = "Noise/OctaveNoiseSettings")]
	public class OctaveNoiseSettings : NoiseSettings
	{
		[SerializeField]
		private List<OctaveSettings> _octaves = new List<OctaveSettings>();
		public List<OctaveSettings> Octaves => _octaves;

		public override NoiseType GetNoiseType()
		{
			return NoiseType.Octave;
		}
	}
}