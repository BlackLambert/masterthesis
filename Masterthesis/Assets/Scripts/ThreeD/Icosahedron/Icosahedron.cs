using UnityEngine;

namespace SBaier.Master
{
    public class Icosahedron
    {
		private const float _e0 = 0.525731112119133606f;
		private const float _e1 = 0.850650808352039932f;
		private const float _e2 = 0;

		public static readonly Vector3[] Vertices =
		{
			new Vector3(-_e0, _e2, _e1),
			new Vector3(_e0, _e2, _e1),
			new Vector3(-_e0, _e2, -_e1),
			new Vector3(_e0, _e2, -_e1),

			new Vector3(_e2, _e1, _e0),
			new Vector3(_e2, _e1, -_e0),
			new Vector3(_e2, -_e1, _e0),
			new Vector3(_e2, -_e1, -_e0),

			new Vector3(_e1, _e0, _e2),
			new Vector3(-_e1, _e0, _e2),
			new Vector3(_e1, -_e0, _e2),
			new Vector3(-_e1, -_e0, _e2),
		};

		public static readonly int[] Triangles =
		{
			0, 1, 4,
			0, 4, 9, 
			9, 4, 5,
			4, 8, 5,
			4, 1, 8,
			8, 1, 10,
			8, 10, 3, 
			5, 8, 3,
			5, 3, 2,
			2, 3, 7,
			7, 3, 10,
			7, 10, 6,
			7, 6, 11,
			11, 6, 0,
			0, 6, 1,
			6, 10, 1,
			9, 11, 0,
			9, 2, 11,
			9, 5, 2,
			7, 11, 2
		};
	}
}