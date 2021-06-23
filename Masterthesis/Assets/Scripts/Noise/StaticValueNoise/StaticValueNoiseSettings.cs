using UnityEngine;

namespace SBaier.Master
{
	[CreateAssetMenu(fileName = "StaticValueNoiseSettings", menuName = "Noise/StaticValueNoiseSettings")]
	public class StaticValueNoiseSettings : NoiseSettings
	{
		[Range(0, 1)]
		[SerializeField]
		private float _value;
		public float Value => _value;

		public override NoiseType GetNoiseType()
		{
			return NoiseType.Static;
		}
	}
}