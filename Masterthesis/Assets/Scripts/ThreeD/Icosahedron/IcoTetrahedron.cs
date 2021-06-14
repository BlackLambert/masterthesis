using System;
using UnityEngine;

namespace SBaier.Master
{
    public class IcoTetrahedron : MonoBehaviour, Movable
    {
        [SerializeField]
        private MeshFilter _meshFilter;
        private Vector3 _outwardVector = Vector3.zero;

        private readonly int[] _triangles =
        {
            0, 2, 1,
            0, 1, 3,
            0, 3, 2,
            1, 2, 3
        };

        protected virtual void Reset()
		{
            _meshFilter = GetComponent<MeshFilter>();
        }

        protected virtual void Awake()
		{
            _meshFilter.sharedMesh = new Mesh();
        }

        public void UpdateMesh(Vector3[] vertices)
		{
            if (vertices.Length != 4)
                throw new ArgumentException();
            _meshFilter.sharedMesh.Clear();
            _meshFilter.sharedMesh.vertices = vertices;
            _meshFilter.sharedMesh.triangles = _triangles;
            CalculateOuterTriangleCenter();
            _meshFilter.sharedMesh.RecalculateNormals();
        }

		private void CalculateOuterTriangleCenter()
		{
            Vector3 sideCenter = (_meshFilter.sharedMesh.vertices[3] - _meshFilter.sharedMesh.vertices[2]) / 2 + _meshFilter.sharedMesh.vertices[2];
            Vector3 outerTriangleCenter = (sideCenter - _meshFilter.sharedMesh.vertices[1]) / 2 + _meshFilter.sharedMesh.vertices[1];
            _outwardVector = outerTriangleCenter - transform.localPosition;
        }

        public void Move(float delta)
		{
            Vector3 deltaMovement = _outwardVector.normalized * delta;
            transform.localPosition += deltaMovement;
        }
	}
}