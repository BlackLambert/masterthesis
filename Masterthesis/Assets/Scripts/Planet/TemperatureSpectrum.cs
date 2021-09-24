using Newtonsoft.Json;
using System;

namespace SBaier.Master
{
	[Serializable]
	[JsonObject(MemberSerialization.OptIn)]
	public struct TemperatureSpectrum
    {
		private const float _minTemp = -273.2f;

		[JsonConstructor]
		public TemperatureSpectrum(float minimal, float maximal)
		{
			Validate(minimal, maximal);

			Minimal = minimal;
			Maximal = maximal;
		}

		[JsonProperty("tempMinimal")]
		public float Minimal { get; }

		[JsonProperty("tempMaximal")]
		public float Maximal { get; } 

		[JsonIgnore]
		public float Delta => Maximal - Minimal;

		private static void Validate(float min, float max)
		{
			if (min > max || min < _minTemp)
				throw new ArgumentOutOfRangeException();
		}
	}
}