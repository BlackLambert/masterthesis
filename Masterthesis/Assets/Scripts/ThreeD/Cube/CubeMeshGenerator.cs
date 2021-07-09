using System;
using System.Linq;
using UnityEngine;

namespace SBaier.Master
{
	public class CubeMeshGenerator : MeshGenerator
	{
		public void GenerateMeshFor(Mesh mesh)
		{
			GenerateMeshFor(mesh, 1f);
		}

		public void GenerateMeshFor(Mesh mesh, float size)
		{
			ValidateSize(size);
			Vector3[] vertices = Cube.Vertices.ToArray();
			for (int i = 0; i < vertices.Length; i++)
				vertices[i] = vertices[i] * size;
			mesh.vertices = vertices;
			mesh.triangles = Cube.VertexIndices;

		}

		private void ValidateSize(float size)
		{
			if (size <= 0)
				throw new ArgumentOutOfRangeException();
		}
	}
}