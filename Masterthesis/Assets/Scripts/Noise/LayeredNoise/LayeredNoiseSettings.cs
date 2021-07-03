using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
	[CreateAssetMenu(fileName = "LayeredNoiseSettings", menuName = "Noise/LayeredNoiseSettings")]
	public class LayeredNoiseSettings : NoiseSettings
	{
		[SerializeField]
		private List<NoiseSettings> _layers = new List<NoiseSettings>();
		public List<NoiseSettings> Layers => _layers;

		[SerializeField]
		private LayeredNoise.MappingMode _mapping = LayeredNoise.MappingMode.NegOneToOne;
		public LayeredNoise.MappingMode Mapping => _mapping;

		public override NoiseType GetNoiseType()
		{
			return NoiseType.Layered;
		}
	}
}