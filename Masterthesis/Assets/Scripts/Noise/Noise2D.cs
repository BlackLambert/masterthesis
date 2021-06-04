using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
	public interface Noise2D
	{
		double Evaluate(double x, double y);
	}
}