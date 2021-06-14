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
        private Transform _target = null;

        protected virtual void Reset()
        {
            _target = GetComponent<Transform>();
        }

        protected virtual void Update()
		{
			RotateRarget();
		}

		private void RotateRarget()
		{
			_target.Rotate(_deltaPerSecond * Time.deltaTime);
		}
    }
}