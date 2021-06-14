using System;
using UnityEngine;

namespace SBaier.Master
{
	public class IcosahedronGenerator : MeshGenerator
	{
		public IcosahedronGenerator()
		{
		}

		public void GenerateMeshFor(Mesh mesh)
		{
			GenerateMeshFor(mesh, 1f);
		}

		public void GenerateMeshFor(Mesh mesh, float size)
		{
			SetVertices(mesh, size);
			SetTriangleIndices(mesh);
		}

		private void SetVertices(Mesh mesh, float size)
		{
			Vector3[] vertices = Icosahedron.Vertices;
			for (int i = 0; i < vertices.Length; i++)
				vertices[i] = vertices[i].normalized * size;
			mesh.vertices = vertices;
		}

		private void SetTriangleIndices(Mesh mesh)
		{
			mesh.triangles = Icosahedron.Triangles;
		}
	}
}