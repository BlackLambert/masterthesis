using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public class DrawGizmoSpheresAtVertexPositions : MonoBehaviour
    {
        [SerializeField]
        private MeshFilter _meshFilter;
        [SerializeField]
        private float _size = 0.01f;
        [SerializeField]
        private Color _color = Color.black;

        public void OnDrawGizmosSelected()
		{
            if (_meshFilter.sharedMesh == null)
                return;
            Vector3[] vertices = _meshFilter.sharedMesh.vertices;
            Gizmos.color = _color;
            foreach (Vector3 vertex in vertices)
                Gizmos.DrawSphere(transform.position + vertex, _size);
		}
    }
}