using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
	[CreateAssetMenu(fileName = "RidgedNoiseSettings", menuName = "Noise/RidgedNoiseSettings")]
	public class RidgedNoiseSettings : NoiseSettings
	{
		[SerializeField]
		private BillowNoiseSettings _billowNoiseSettings;
		public BillowNoiseSettings BillowNoiseSettings => _billowNoiseSettings;

		public override NoiseType GetNoiseType()
		{
			return NoiseType.Ridged;
		}
	}
}