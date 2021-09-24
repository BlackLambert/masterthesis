using Newtonsoft.Json;
using System;

namespace SBaier.Master
{
	[Serializable]
	[JsonObject(MemberSerialization.OptIn)]
	public struct PlanetDimensions
	{
		[JsonConstructor]
		public PlanetDimensions(float kernelRadius, float hullMaxRadius, float relativeSeaLevel, float atmosphereRadius)
		{
			ValidateKernalThickness(kernelRadius);
			ValidateHullMaxRadius(hullMaxRadius, kernelRadius);
			ValidateRelativeSeaLevel(relativeSeaLevel);
			ValidateAtmosphereThickness(atmosphereRadius, hullMaxRadius);

			KernelRadius = kernelRadius;
			HullMaxRadius = hullMaxRadius;
			RelativeSeaLevel = relativeSeaLevel;
			AtmosphereRadius = atmosphereRadius;
			MaxHullThickness = hullMaxRadius - kernelRadius;
		}


		[JsonProperty("kernelRadius")]
		public float KernelRadius { get; }

		[JsonProperty("hullMaxRadius")]
		public float HullMaxRadius { get; }

		[JsonProperty("atmosphereRadius")]
		public float AtmosphereRadius { get; }

		[JsonProperty("relativeSeaLevel")]
		public float RelativeSeaLevel { get; }

		[JsonIgnore]
		public float MaxHullThickness { get; }


		private static void ValidateKernalThickness(float kernelRadius)
		{
			if (kernelRadius <= 0)
				throw new ArgumentOutOfRangeException();
		}

		private static void ValidateHullMaxRadius(float hullMaxRadius, float kernelRadius)
		{
			if (hullMaxRadius <= kernelRadius)
				throw new ArgumentOutOfRangeException();
		}

		private static void ValidateAtmosphereThickness(float atmosphereThickness, float maxHullRadius)
		{
			if (atmosphereThickness <= maxHullRadius)
				throw new ArgumentOutOfRangeException();
		}

		private static void ValidateRelativeSeaLevel(float seaLevel)
		{
			if (seaLevel < 0 || seaLevel > 1)
				throw new ArgumentOutOfRangeException();
		}
	}
}