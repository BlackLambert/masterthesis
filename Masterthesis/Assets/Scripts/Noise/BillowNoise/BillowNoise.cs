using System;
using System.Runtime.CompilerServices;

namespace SBaier.Master
{
	public class BillowNoise : Noise3D
	{
		private Noise3D _baseNoise;
		public Noise3D BaseNoise => _baseNoise;

		public BillowNoise(Noise3D baseNoise)
		{
			_baseNoise = baseNoise;
		}

		public double Evaluate(double x, double y)
		{
			return Evaluate(x, y, 0);
		}

		public double Evaluate(double x, double y, double z)
		{
			double perlinValue = _baseNoise.Evaluate(x, y, z);
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