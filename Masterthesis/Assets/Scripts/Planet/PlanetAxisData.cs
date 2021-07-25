using System;

namespace SBaier.Master
{
    public struct PlanetAxisData
    {
        public PlanetAxisData(float angle, float secondsPerRevolution)
		{
			ValidateAngle(angle);
			ValidateSecondsPerRevolution(secondsPerRevolution);

			Angle = angle;
			SecondsPerRevolution = secondsPerRevolution;
		}

		private static void ValidateAngle(float angle)
		{
			if (angle < 0 || angle > 90)
				throw new ArgumentOutOfRangeException();
		}

		private static void ValidateSecondsPerRevolution(float secondsPerRevolution)
		{
			if (secondsPerRevolution <= 0)
				throw new ArgumentOutOfRangeException();
		}

		public float Angle { get; }
		public float SecondsPerRevolution { get; }
		public float RotationPerSecond => 360 / SecondsPerRevolution;
	}
}