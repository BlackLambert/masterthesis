using System;
using UnityEngine;

namespace SBaier.Master
{
	public interface Noise2D : Noise
	{
		float Evaluate2D(Vector2 point);
		float[] Evaluate2D(Vector2[] points);
	}
}