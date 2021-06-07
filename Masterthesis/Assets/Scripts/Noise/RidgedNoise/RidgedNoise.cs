using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
	public class RidgedNoise : Noise3D
	{
		private BillowNoise _bollowNoise;

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
			// Inverts the Billow Noise evaluated values
			return _bollowNoise.Evaluate(x, y, z) * (-1) + 1;
		}
	}
}