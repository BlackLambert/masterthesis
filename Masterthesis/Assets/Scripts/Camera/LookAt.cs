using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public class LookAt : MonoBehaviour
    {
        [SerializeField]
        private Transform _transform;
        [SerializeField]
        private Transform _lookAtTarget;

        protected virtual void Update()
		{
            _transform.LookAt(_lookAtTarget);
        }
    }
}