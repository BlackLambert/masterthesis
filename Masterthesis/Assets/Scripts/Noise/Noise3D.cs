using System;
using Unity.Collections;
using UnityEngine;

namespace SBaier.Master
{
	public interface Noise3D : Noise2D
	{
		
		float Evaluate3D(Vector3 point );
		NativeArray<float> Evaluate3D(NativeArray<Vector3> points);
	}
}