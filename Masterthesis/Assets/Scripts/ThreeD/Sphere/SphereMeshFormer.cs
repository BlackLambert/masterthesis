using System;
using UnityEngine;

namespace SBaier.Master
{
	public class SphereMeshFormer : MeshFormer
	{
		Vector3[] _newVertices;
		Vector3[] _formerVertices;

		public void Form(Mesh testMesh)
		{
			Form(testMesh, 1);
		}

		public void Form(Mesh testMesh, float size)
		{
			_formerVertices = testMesh.vertices;
			if(_newVertices == null || _newVertices.Length != _formerVertices.Length)
				_newVertices = new Vector3[_formerVertices.Length];

			for (int i = 0; i < testMesh.vertexCount; i++)
			{
				if (_formerVertices[i].normalized.magnitude == 0)
					_formerVertices[i] = Vector3.up;
				_newVertices[i] = _formerVertices[i].normalized * size;
			}

			testMesh.vertices = _newVertices;
		}
	}
}