using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public class VolumetricIcosahedron : MonoBehaviour
    {
        [SerializeField]
        private IcoTetrahedron _tetrahedronPrefab;
		[SerializeField]
		private Transform _meshHook;
		private IcoTetrahedron[] _icoTetrahedra = new IcoTetrahedron[20];

		protected virtual void Reset()
		{
			_meshHook = GetComponent<Transform>();
		}

        protected virtual void Start()
		{
			GenerateTetraheda();
			GenerateMesh();
		}

		private void GenerateTetraheda()
		{
			for (int i = 0; i < _icoTetrahedra.Length; i++)
				_icoTetrahedra[i] = GenerateTetrahedron();
		}

		private IcoTetrahedron GenerateTetrahedron()
		{
			IcoTetrahedron tetrahedron = GameObject.Instantiate(_tetrahedronPrefab);
			tetrahedron.transform.SetParent(_meshHook, false);
			tetrahedron.transform.localPosition = Vector3.zero;
			return tetrahedron;
		}

		private void GenerateMesh()
		{
			const int triangleVertexCount = 3;
			for(int i = 0; i< _icoTetrahedra.Length; i++)
			{
				Vector3[] vertices = new Vector3[4];
				vertices[0] = Vector3.zero;
				vertices[1] = Icosahedron.Vertices[Icosahedron.Triangles[triangleVertexCount * i]];
				vertices[2] = Icosahedron.Vertices[Icosahedron.Triangles[triangleVertexCount * i + 1]];
				vertices[3] = Icosahedron.Vertices[Icosahedron.Triangles[triangleVertexCount * i + 2]];
				_icoTetrahedra[i].UpdateMesh(vertices);
			}
		}
	}
}