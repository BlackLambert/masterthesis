using System;

namespace SBaier.Master
{
	[Serializable]
	public struct TemperatureSpectrum
    {
		private const float _minTemp = -273.2f;

		public TemperatureSpectrum(float minimal, float maximal)
		{
			Validate(minimal, maximal);

			Minimal = minimal;
			Maximal = maximal;
		}

		public float Minimal { get; }
		public float Maximal { get; }
		public float Delta => Maximal - Minimal;

		private static void Validate(float min, float max)
		{
			if (min > max || min < _minTemp)
				throw new ArgumentOutOfRangeException();
		}
	}
}