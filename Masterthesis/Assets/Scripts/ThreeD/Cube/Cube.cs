using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public class Cube : MonoBehaviour
    {
		private const float _vP = 0.5f;

		public static readonly Vector3[] Vertices =
		{
			new Vector3(-_vP, -_vP, _vP),
			new Vector3(-_vP, -_vP, -_vP),
			new Vector3(_vP, -_vP, -_vP),
			new Vector3(_vP, -_vP, _vP),
			new Vector3(-_vP, _vP, _vP),
			new Vector3(-_vP, _vP, -_vP),
			new Vector3(_vP, _vP, -_vP),
			new Vector3(_vP, _vP, _vP)
		};

		public static readonly int[] VertexIndices =
		{
			0, 1, 2, //bottom
			0, 2, 3,
			0, 7, 4, //front
			0, 3, 7,
			3, 6, 7, //right
			3, 2, 6,
			2, 1, 5, //back
			2, 5, 6,
			1, 0, 4, //left
			1, 4, 5,
			4, 7, 5, //top
			5, 7, 6
		};
	}
}