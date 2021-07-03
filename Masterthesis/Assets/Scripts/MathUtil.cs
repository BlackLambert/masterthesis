using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public static class MathUtil
	{

		public static double Clamp01(double result)
		{
			return (result > 1) ? 1 : (result < 0) ? 0 : result;
		}

		public static float Clamp01(float result)
		{
			return (result > 1) ? 1 : (result < 0) ? 0 : result;
		}
	}
}