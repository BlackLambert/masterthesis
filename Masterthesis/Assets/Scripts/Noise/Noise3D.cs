using System;
using UnityEngine;

namespace SBaier.Master
{
	public interface Noise3D : Noise2D
	{
		float Evaluate3D(Vector3 point );
		float[] Evaluate3D(Vector3[] points);

	}
}