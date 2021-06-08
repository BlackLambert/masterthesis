using System.Runtime.CompilerServices;

namespace SBaier.Master
{
	public class RidgedNoise : Noise3D
	{
		private BillowNoise _bollowNoise;

		public NoiseType NoiseType => NoiseType.Ridged;

		public RidgedNoise(BillowNoise billowNoise)
		{
			_bollowNoise = billowNoise;
		}

		public double Evaluate(double x, double y)
		{
			return Evaluate(x, y, 0);
		}

		public double Evaluate(double x, double y, double z)
		{
			double billowValue = _bollowNoise.Evaluate(x, y, z);
			return InvertValue(billowValue);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private double InvertValue(double billowValue)
		{
			return billowValue * (-1) + 1;
		}
	}
}