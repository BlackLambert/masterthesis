using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public class DelaunayMesh : MonoBehaviour
    {
        [SerializeField]
        private MeshFilter _meshFilter;
        private Triangle[] _triangles;

        protected virtual void Start()
		{
            _meshFilter.sharedMesh = new Mesh();
        }

        public void UpdateView(Vector3[] vertices, Triangle[] triangles)
        {
            _triangles = triangles;
            _meshFilter.sharedMesh = new Mesh();
            _meshFilter.sharedMesh.vertices = vertices;
            _meshFilter.sharedMesh.triangles = CreateTriangles();
            _meshFilter.sharedMesh.RecalculateNormals();
        }

        private int[] CreateTriangles()
        {
            int[] result = new int[_triangles.Length * 3];
            for (int i = 0; i < _triangles.Length; i++)
				AddTriangleIndices(result, i);
			return result;
        }

		private void AddTriangleIndices(int[] result, int index)
		{
			Triangle triangle = _triangles[index];
			result[index * 3] = triangle.VertexIndices[0];
			result[index * 3 + 1] = triangle.VertexIndices[1];
			result[index * 3 + 2] = triangle.VertexIndices[2];
		}
	}
}