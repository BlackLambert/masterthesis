using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
	[CreateAssetMenu(fileName = "LayeredNoiseSettings", menuName = "Noise/LayeredNoiseSettings")]
	public class LayeredNoiseSettings : NoiseSettings
	{
		[SerializeField]
		private List<NoiseLayerSettings> _layers = new List<NoiseLayerSettings>();
		public List<NoiseLayerSettings> Layers => _layers;

		[SerializeField]
		private LayeredNoise.MappingMode _mapping = LayeredNoise.MappingMode.NegOneToOne;
		public LayeredNoise.MappingMode Mapping => _mapping;

		public override NoiseType GetNoiseType()
		{
			return NoiseType.Layered;
		}
	}
}