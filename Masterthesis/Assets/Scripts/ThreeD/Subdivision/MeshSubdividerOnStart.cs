using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace SBaier.Master
{
    public class MeshSubdividerOnStart : MonoBehaviour
    {
        [SerializeField]
        private int _amount = 1;
        [SerializeField]
        private MeshFilter _meshFilter;


        private MeshSubdivider _subdivider;

        [Inject]
        public void Construct(MeshSubdivider subdivider)
		{
            _subdivider = subdivider;
        }

        protected virtual void Start()
		{
            _subdivider.Subdivide(_meshFilter.sharedMesh, _amount);
            _meshFilter.sharedMesh.RecalculateNormals();
        }
    }
}