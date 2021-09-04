using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace SBaier.Master
{
    public class MeshToMeshColliderSetter : MonoBehaviour
    {
        [SerializeField]
        private MeshCollider _collider;
        [SerializeField]
        private MeshFilter _meshFilter;


        protected virtual void Start()
		{
            _collider.sharedMesh = _meshFilter.sharedMesh;
        }
    }
}