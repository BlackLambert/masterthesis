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
			ThicknessRadius = atmosphereThickness;
		}

		public float KernalThickness { get; }
		public float ThicknessRadius { get; }
		public float VariableAreaThickness => ThicknessRadius - KernalThickness;


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