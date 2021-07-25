using System;

namespace SBaier.Master
{
	public struct PlanetDimensions
	{
		public PlanetDimensions(float kernalThickness, float atmosphereThickness)
		{
			ValidateKernalThickness(kernalThickness);
			ValidateAtmosphereThickness(atmosphereThickness);

			KernalThickness = kernalThickness;
			AtmosphereThickness = atmosphereThickness;
		}

		public float KernalThickness { get; }
		public float AtmosphereThickness { get; }
		public float VariableAreaThickness => AtmosphereThickness - KernalThickness;


		private static void ValidateKernalThickness(float kernalRadius)
		{
			if (kernalRadius <= 0)
				throw new ArgumentOutOfRangeException();
		}
		private static void ValidateAtmosphereThickness(float atmosphereThickness)
		{
			if (atmosphereThickness <= 0)
				throw new ArgumentOutOfRangeException();
		}
	}
}