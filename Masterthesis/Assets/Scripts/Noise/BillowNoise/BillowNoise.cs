using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
	public class BillowNoise : Noise3D
	{
		private Noise3D _baseNoise;

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
			return Math.Abs(perlinValue * 2 - 1);
		}
	}
}