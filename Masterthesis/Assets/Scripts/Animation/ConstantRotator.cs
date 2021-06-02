using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public class ConstantRotator : MonoBehaviour
    {
        [SerializeField]
        private Vector3 _deltaPerSecond = new Vector3(0, 30, 0);
        [SerializeField]
        private Transform _target;

        protected virtual void Update()
		{
            _target.Rotate(_deltaPerSecond * Time.deltaTime);
        }

        protected virtual void Reset()
		{
            _target = GetComponent<Transform>();
        }
    }
}