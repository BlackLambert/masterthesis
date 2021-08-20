using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SBaier.Master
{
    public class CameraFocalDistanceController : MonoBehaviour
    {
        [SerializeField]
        private Transform _cameraTransform;
        [SerializeField]
        private float _maxDistance = 30f;
        [SerializeField]
        private float _minDistance = 10f;
        [SerializeField]
        private float _factor = 1f;

        protected virtual void Update()
		{
            if (Input.mouseScrollDelta.sqrMagnitude == 0 ||
                EventSystem.current.IsPointerOverGameObject())
                return;
            UpdatePosition(Input.mouseScrollDelta.y);
		}

		private void UpdatePosition(float delta)
        {
            Vector3 formerPos = _cameraTransform.localPosition;
            float valueZ = formerPos.z + delta * _factor;
            valueZ = Mathf.Clamp(valueZ, _minDistance, _maxDistance);
            _cameraTransform.localPosition = new Vector3(formerPos.x, formerPos.y, valueZ);
        }
	}
}