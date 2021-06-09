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
            List<int> triangles = new List<int>();

            for (int i = 0; i < mesh.vertices.Length; i++)
                vertices.Add(mesh.vertices[i], i);

            for (int i = 0; i < mesh.triangles.Length / 3; i++)
            {
                int i0 = mesh.triangles[i * 3];
                int i1 = mesh.triangles[i * 3 + 1];
                int i2 = mesh.triangles[i * 3 + 2];

                Vector3 vertex0 = mesh.vertices[i0];
                Vector3 vertex1 = mesh.vertices[i1];
                Vector3 vertex2 = mesh.vertices[i2];
                Vector3 vertex01 = Subdivide(vertex0, vertex1);
                Vector3 vertex12 = Subdivide(vertex1, vertex2);
                Vector3 vertex20 = Subdivide(vertex2, vertex0);
                AddIfNew(ref vertices, vertex01);
                AddIfNew(ref vertices, vertex12);
                AddIfNew(ref vertices, vertex20);

                int i3 = vertices[vertex01];
                int i4 = vertices[vertex12];
                int i5 = vertices[vertex20];

                triangles.AddRange(new int[]
                {
                    i0, i3, i5,
                    i3, i1, i4,
                    i5, i4, i2,
                    i5, i3, i4
                });
            }
            mesh.vertices = vertices.Keys.ToArray();
            mesh.triangles = triangles.ToArray();
        }

        private Vector3 Subdivide(Vector3 vecA, Vector3 vecB)
		{
            return vecA + (vecB - vecA) * 0.5f;
        }

        private void AddIfNew(ref Dictionary<Vector3, int> vertices, Vector3 vertex)
		{
            if (!vertices.ContainsKey(vertex))
                vertices.Add(vertex, vertices.Count);
        }
    }
}