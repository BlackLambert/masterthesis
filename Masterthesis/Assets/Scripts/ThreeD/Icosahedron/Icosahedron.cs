using UnityEngine;

namespace SBaier.Master
{
    public class Icosahedron : MonoBehaviour
    {
        [SerializeField]
        private MeshFilter _meshFilter;

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
			0, 4, 1,
			0, 9, 4, 
			9, 5, 4,
			4, 5, 8,
			4, 8, 1,
			8, 10, 1,
			8, 3, 10, 
			5, 3, 8,
			5, 2, 3,
			2, 7, 3,
			7, 10, 3,
			7, 6, 10,
			7, 11, 6,
			11, 0, 6,
			0, 1, 6,
			6, 1, 10,
			9, 0, 11,
			9, 11, 2,
			9, 2, 5,
			7, 2, 11
		};

		protected virtual void Start()
		{
			_meshFilter.sharedMesh = new Mesh();
			GenerateMesh();
		}

		private void GenerateMesh()
		{
			_meshFilter.sharedMesh.Clear();
			_meshFilter.sharedMesh.vertices = Vertices;
			_meshFilter.sharedMesh.triangles = Triangles;
		}
	}
}