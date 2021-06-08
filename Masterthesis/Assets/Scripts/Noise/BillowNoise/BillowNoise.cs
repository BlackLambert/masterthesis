using System;
using System.Runtime.CompilerServices;

namespace SBaier.Master
{
	public class BillowNoise : Noise3D
	{
		public Noise3D BaseNoise { get; }

		public NoiseType NoiseType => NoiseType.Billow;

		public BillowNoise(Noise3D baseNoise)
		{
			BaseNoise = baseNoise;
		}

		public double Evaluate(double x, double y)
		{
			return Evaluate(x, y, 0);
		}

		public double Evaluate(double x, double y, double z)
		{
			double perlinValue = BaseNoise.Evaluate(x, y, z);
			double stretchedValue = stretchToNegativeValueRange(perlinValue);
			return Math.Abs(stretchedValue);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private double stretchToNegativeValueRange(double perlinValue)
		{
			return perlinValue * 2 - 1;
		}
	}
}