using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SBaier.Master
{
    public class RecursiveMeshSubdivider : MeshSubdivider
    {
        public void Subdivide(Mesh mesh, int amount)
		{
            if (amount < 1)
                throw new ArgumentOutOfRangeException();
            while (amount > 0)
            {
                Subdivide(mesh);
                amount--;
            }
        }

        public void Subdivide(Mesh mesh)
		{
            Dictionary<Vector3, int> vertices = new Dictionary<Vector3, int>(new Vector3EqualityComparer());
            int trianglesCount = mesh.triangles.Length * 4;
            int[] triangles = new int[trianglesCount];
            int[] formerTriangles = mesh.triangles;
            Vector3[] verticesCache = new Vector3[6];
            Vector3[] formerVertices = mesh.vertices;
            int trianglesIndex = 0;

            for (int i = 0; i < formerVertices.Length; i++)
                vertices.Add(formerVertices[i], i);

            for (int i = 0; i < formerTriangles.Length / 3; i++)
            {
                int i0 = formerTriangles[i * 3];
                int i1 = formerTriangles[i * 3 + 1];
                int i2 = formerTriangles[i * 3 + 2];

                verticesCache[0] = formerVertices[i0];
                verticesCache[1] = formerVertices[i1];
                verticesCache[2] = formerVertices[i2];
                verticesCache[3] = Subdivide(verticesCache[0], verticesCache[1]);
                verticesCache[4] = Subdivide(verticesCache[1], verticesCache[2]);
                verticesCache[5] = Subdivide(verticesCache[2], verticesCache[0]);
                AddIfNew(ref vertices, verticesCache[3]);
                AddIfNew(ref vertices, verticesCache[4]);
                AddIfNew(ref vertices, verticesCache[5]);

                int i3 = vertices[verticesCache[3]];
                int i4 = vertices[verticesCache[4]];
                int i5 = vertices[verticesCache[5]];

                triangles[trianglesIndex] = i0;
                triangles[trianglesIndex + 1] = i3;
                triangles[trianglesIndex + 2] = i5;
                triangles[trianglesIndex + 3] = i3;
                triangles[trianglesIndex + 4] = i1;
                triangles[trianglesIndex + 5] = i4;
                triangles[trianglesIndex + 6] = i5;
                triangles[trianglesIndex + 7] = i4;
                triangles[trianglesIndex + 8] = i2;
                triangles[trianglesIndex + 9] = i5;
                triangles[trianglesIndex + 10] = i3;
                triangles[trianglesIndex + 11] = i4;

                trianglesIndex += 12;
            }
            mesh.vertices = vertices.Keys.ToArray();
            mesh.triangles = triangles;
        }

        private Vector3 Subdivide(Vector3 vecA, Vector3 vecB)
		{
            return vecA + (vecB - vecA) * 0.5f;
        }

        private void AddIfNew(ref Dictionary<Vector3, int> vertices, Vector3 vertex)
		{
            if (!vertices.ContainsKey(vertex))
                vertices[vertex] = vertices.Count;
        }
    }
}