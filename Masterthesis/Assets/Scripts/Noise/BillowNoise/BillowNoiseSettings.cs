using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
	[CreateAssetMenu(fileName = "BillowNoiseSettings", menuName = "Noise/BillowNoiseSettings")]
	public class BillowNoiseSettings : NoiseSettings
	{
		[SerializeField]
		private NoiseSettings _baseNoiseSettings;
		public NoiseSettings BaseNoiseSettings => _baseNoiseSettings;

		public override NoiseType GetNoiseType()
		{
			return NoiseType.Billow;
		}
	}
}