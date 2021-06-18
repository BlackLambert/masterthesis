using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SBaier.Master
{
	public class DuplicateVerticesRemover
	{
		public void RunOn(Mesh testMesh)
		{
			Dictionary<Vector3, int> newVerticesToIndex = new Dictionary<Vector3, int>(new Vector3EqualityComparer());
			Vector3[] vertices = testMesh.vertices;
			int[] vertexIndices = testMesh.triangles;
			int[] newVertexIndices = new int[vertexIndices.Length];
			int index = 0;

			for(int i = 0; i < vertexIndices.Length; i++)
			{
				try
				{
					newVerticesToIndex.Add(vertices[vertexIndices[i]], index);
					index++;
				}
				catch (ArgumentException) { }
				newVertexIndices[i] = newVerticesToIndex[vertices[vertexIndices[i]]];
			}

			testMesh.triangles = newVertexIndices;
			testMesh.vertices = newVerticesToIndex.Keys.ToArray();
		}
	}
}