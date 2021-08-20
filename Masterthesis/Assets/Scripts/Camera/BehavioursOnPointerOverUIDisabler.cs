using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SBaier.Master
{
    public class BehavioursOnPointerOverUIDisabler : MonoBehaviour
    {
        [SerializeField]
        private MonoBehaviour[] _targets;

        private bool _overUI = false;

        protected virtual void Start()
		{
            _overUI = EventSystem.current.IsPointerOverGameObject();
            UpdateComponents();
        }

        protected virtual void Update()
		{
            if (_overUI == EventSystem.current.IsPointerOverGameObject())
                return;
            _overUI = !_overUI;
            UpdateComponents();
        }

		private void UpdateComponents()
		{
			foreach (MonoBehaviour behaviour in _targets)
                behaviour.enabled = !_overUI;
		}
	}
}