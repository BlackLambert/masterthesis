using System;
using Unity.Collections;
using UnityEngine;

namespace SBaier.Master
{
	public interface Noise2D : Noise
	{
		float Evaluate2D(Vector2 point);
		NativeArray<float> Evaluate2D(NativeArray<Vector2> points);
	}
}