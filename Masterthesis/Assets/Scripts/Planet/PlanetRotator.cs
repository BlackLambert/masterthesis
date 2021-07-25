using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace SBaier.Master
{
    public class PlanetRotator : MonoBehaviour
    {
        private PlanetAxisData _axis;

        [SerializeField]
        private Transform _target;

        [Inject]
        private void Construct(PlanetAxisData axis)
		{
            _axis = axis;
        }

        protected virtual void Update()
		{
            _target.Rotate(Vector3.up, _axis.RotationPerSecond * Time.deltaTime, Space.Self);

        }
    }
}