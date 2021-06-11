using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
	[CreateAssetMenu(fileName = "LayeredNoiseSettings", menuName = "Noise/LayeredNoiseSettings")]
	public class LayeredNoiseSettings : NoiseSettings
	{
		[SerializeField]
		private List<NoiseLayerSettings> _octaves = new List<NoiseLayerSettings>();
		public List<NoiseLayerSettings> Layers => _octaves;

		public override NoiseType GetNoiseType()
		{
			return NoiseType.Layered;
		}
	}
}