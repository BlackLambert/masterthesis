using System;
using UnityEngine;

namespace SBaier.Master
{
	public class SphereMeshFormer : MeshFormer
	{
		public void Form(Mesh testMesh)
		{
			Form(testMesh, 1);
		}

		public void Form(Mesh testMesh, float size)
		{
			Vector3[] newVertices = new Vector3[testMesh.vertexCount];
			Vector3[] formerVertices = testMesh.vertices;
			for (int i = 0; i < testMesh.vertexCount; i++)
			{
				if (formerVertices[i].normalized.magnitude == 0)
					formerVertices[i] = Vector3.up;
				newVertices[i] = formerVertices[i].normalized * size;
			}
			testMesh.vertices = newVertices;
		}
	}
}