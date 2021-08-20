using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SBaier.Master
{
    public class TrackballCameraController : MonoBehaviour
    {
        [SerializeField]
        private Transform _cameraTransform;
        [SerializeField]
        private float _movementFactor = 1;
        [SerializeField]
        private float _yBorder = 5f;

        private Vector3 _formerPositition;

        protected virtual void Update()
		{
            InitFormerPosition();
            RotateCamera();
        }

        private void InitFormerPosition()
		{
            if (!Input.GetMouseButtonDown(0) ||
                EventSystem.current.IsPointerOverGameObject())
                return;
            _formerPositition = Input.mousePosition;
        }

        private void RotateCamera()
		{
            if (!Input.GetMouseButton(0) ||
                EventSystem.current.IsPointerOverGameObject())
                return;
            UpdateCameraRotation();
            _formerPositition = Input.mousePosition;
        }

		private void UpdateCameraRotation()
		{
            Vector2 delta = Input.mousePosition - _formerPositition;
            float xAngle = delta.x * _movementFactor;
            _cameraTransform.Rotate(Vector3.up, xAngle, Space.World);
            float yAngle = delta.y * _movementFactor;
            float newYAngle = Vector3.Angle(_cameraTransform.forward, Vector3.up) + yAngle;
            if (newYAngle <= 180 - _yBorder && newYAngle >= _yBorder)
                _cameraTransform.Rotate(_cameraTransform.right, yAngle, Space.World);
        }
	}
}