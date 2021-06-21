using System;
using UnityEngine;

namespace SBaier.Master
{
	public interface Noise2D : Noise
	{
		double Evaluate(double x, double y);
		double[] Evaluate(Vector2[] points);
	}
}