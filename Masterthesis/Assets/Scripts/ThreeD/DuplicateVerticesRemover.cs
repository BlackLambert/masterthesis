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
			Vector3 currentVertex;

			for(int i = 0; i < vertexIndices.Length; i++)
			{
				currentVertex = vertices[vertexIndices[i]];
				try
				{
					newVerticesToIndex.Add(currentVertex, index);
					index++;
				}
				catch (ArgumentException) { }
				newVertexIndices[i] = newVerticesToIndex[currentVertex];
			}

			testMesh.triangles = newVertexIndices;
			testMesh.vertices = newVerticesToIndex.Keys.ToArray();
		}
	}
}