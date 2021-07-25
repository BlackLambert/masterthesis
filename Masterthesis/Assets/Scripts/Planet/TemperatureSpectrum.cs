using System;

namespace SBaier.Master
{
    public struct TemperatureSpectrum
    {
        public TemperatureSpectrum(float min, float max)
		{
			Validate(min, max);

			Min = min;
			Max = max;
		}

		public float Min { get; }
		public float Max { get; }
		public float Delta => Max - Min;

		private static void Validate(float min, float max)
		{
			if (min > max)
				throw new ArgumentOutOfRangeException();
		}
	}
}