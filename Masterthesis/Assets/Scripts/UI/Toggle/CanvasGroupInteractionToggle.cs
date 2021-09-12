using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SBaier.Master
{
    public class CanvasGroupInteractionToggle : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup _target;
        [SerializeField]
        private Toggle _toggle;
        [SerializeField]
        private bool _interactable;

        protected virtual void Start()
        {
            UpdateView();
            _toggle.onValueChanged.AddListener(OnToggleChanged);
        }

        protected virtual void OnDestroy()
        {
            _toggle.onValueChanged.RemoveListener(OnToggleChanged);
        }

        private void OnToggleChanged(bool arg0)
        {
            UpdateView();
        }

        private void UpdateView()
        {
            _target.interactable = _interactable && _toggle.isOn || !_interactable && !_toggle.isOn;
        }
    }
}