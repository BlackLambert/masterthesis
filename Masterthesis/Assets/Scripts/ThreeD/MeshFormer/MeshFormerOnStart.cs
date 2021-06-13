using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace SBaier.Master
{
    public class MeshFormerOnStart : MonoBehaviour
    {
        [SerializeField]
        private float _size = 1;
		[SerializeField]
		private MeshFilter _meshFilter = null;

		private MeshFormer _meshFormer;

		[Inject]
		public void Construct(MeshFormer meshFormer)
		{
			_meshFormer = meshFormer;
		}

		protected virtual void Start()
		{
			_meshFormer.Form(_meshFilter.sharedMesh, _size);
			_meshFilter.sharedMesh.RecalculateNormals();
		}
	}
}