using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
	public class BillowNoise : Noise2D, Noise3D
	{
		private PerlinNoise _perlinNoise;

		public BillowNoise(PerlinNoise perlinNoise)
		{
			_perlinNoise = perlinNoise;
		}

		public double Evaluate(double x, double y)
		{
			return Evaluate(x, y, 0);
		}

		public double Evaluate(double x, double y, double z)
		{
			double perlinValue = _perlinNoise.Evaluate(x, y, z);
			return Math.Abs(perlinValue * 2 - 1);
		}
	}
}