using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace SBaier.Master
{
    public class MeshFaceSeparatorOnStart : MonoBehaviour
    {
        [SerializeField]
        private Transform _hook;
        [SerializeField]
        private MeshFilter _origin;
        private MeshFaceSeparator _separator;

        [Inject]
        public void Construct(MeshFaceSeparator separator)
		{
            _separator = separator;
        }

        protected virtual void Start()
		{
            MeshFaceSeparatorTarget[] result = _separator.Separate(_origin.sharedMesh);
            foreach (MeshFaceSeparatorTarget target in result)
                target.Base.SetParent(_hook, false);
        }
    }
}