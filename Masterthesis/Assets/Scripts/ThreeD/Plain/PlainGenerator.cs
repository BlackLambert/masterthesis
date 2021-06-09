using UnityEngine;

namespace SBaier.Master
{
	public class PlainGenerator : MeshGenerator
	{
		private readonly Vector3[] _vertices =
		{
			new Vector3(-0.5f, 0, -0.5f),
			new Vector3(-0.5f, 0, 0.5f),
			new Vector3(0.5f, 0, 0.5f),
			new Vector3(0.5f, 0, -0.5f)
		};

		private readonly int[] _triangleVertexIndices = new int[]
		{
			0, 1, 2,
			0, 2, 3
		};

		public void GenerateMeshFor(Mesh mesh)
		{
			GenerateMeshFor(mesh, 1);
		}

		public void GenerateMeshFor(Mesh mesh, float size)
		{
			Vector3[] vertices = _vertices;
			for (int i = 0; i < vertices.Length; i++)
				vertices[i] = vertices[i] * size;
			mesh.vertices = _vertices;
			mesh.triangles = _triangleVertexIndices;
		}
	}
}