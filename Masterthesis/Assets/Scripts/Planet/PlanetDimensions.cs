using System;

namespace SBaier.Master
{
	public struct PlanetDimensions
	{
		public PlanetDimensions(float kernelRadius, float maxHullRadius, float relativeSeaLevel, float atmosphereRadius)
		{
			ValidateKernalThickness(kernelRadius);
			ValidateHullMaxRadius(maxHullRadius, kernelRadius);
			ValidateRelativeSeaLevel(relativeSeaLevel);
			ValidateAtmosphereThickness(atmosphereRadius, maxHullRadius);

			KernelRadius = kernelRadius;
			HullMaxRadius = maxHullRadius;
			RelativeSeaLevel = relativeSeaLevel;
			AtmosphereRadius = atmosphereRadius;
		}

		public float KernelRadius { get; }
		public float HullMaxRadius { get; }
		public float AtmosphereRadius { get; }
		public float RelativeSeaLevel { get; }
		public float MaxHullThickness => HullMaxRadius - KernelRadius;
		public float SeaLevel => MaxHullThickness * RelativeSeaLevel + KernelRadius;


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