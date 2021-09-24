using Newtonsoft.Json;
using System;

namespace SBaier.Master
{
	[Serializable]
	[JsonObject(MemberSerialization.OptIn)]
	public class PlanetAxisData
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


		[JsonProperty("angle")]
		public float Angle { get; }

		[JsonProperty("secondsPerRevolution")]
		public float SecondsPerRevolution { get; }

		[JsonIgnore]
		public float RotationPerSecond => 360 / SecondsPerRevolution;
	}
}