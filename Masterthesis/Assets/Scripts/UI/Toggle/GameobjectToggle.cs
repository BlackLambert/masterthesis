using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SBaier.Master
{
    public class GameobjectToggle : MonoBehaviour
    {
        [SerializeField]
        private GameObject _target;
        [SerializeField]
        private Toggle _toggle;
        [SerializeField]
        private bool _activate;

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
            _target.SetActive(_activate && _toggle.isOn || !_activate && !_toggle.isOn);

        }
	}
}