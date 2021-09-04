using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SBaier.Master
{
    public class BehavioursOnPointerOverUIDisabler : MonoBehaviour
    {
		private const int _uiLayer = 5;
		[SerializeField]
        private MonoBehaviour[] _targets;
        List<RaycastResult> _hits = new List<RaycastResult>();
        PointerEventData _eventData;

        private bool _overUI = false;

        protected virtual void Start()
		{
            _overUI = EventSystem.current.IsPointerOverGameObject();
            _eventData = new PointerEventData(EventSystem.current);
            UpdateComponents();
        }

        protected virtual void Update()
		{
            if (_overUI == IsPointerOverUI())
                return;
            _overUI = !_overUI;
            UpdateComponents();
        }

		private void UpdateComponents()
		{
			foreach (MonoBehaviour behaviour in _targets)
                behaviour.enabled = !_overUI;
        }

        private bool IsPointerOverUI()
        {
            _hits.Clear();
            _eventData.position = Input.mousePosition;
            EventSystem.current.RaycastAll(_eventData, _hits);
            foreach (RaycastResult hit in _hits)
            {
                if (hit.gameObject.layer == _uiLayer)
                    return true;
            }
            return false;
        }
    }
}