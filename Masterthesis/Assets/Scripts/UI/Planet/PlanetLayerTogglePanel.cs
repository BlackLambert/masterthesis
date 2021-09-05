using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SBaier.Master
{
    public class PlanetLayerTogglePanel : MonoBehaviour
    {
        [SerializeField]
        private Toggle _vegetationToggle;
        [SerializeField]
        private Toggle _liquidToggle;
        [SerializeField]
        private Toggle _groundToggle;
        [SerializeField]
        private Toggle _rockToggle;
		[SerializeField]
		private CanvasGroup _canvasGroup;

		public event Action OnToggleChanged;

        protected virtual void Start()
		{
            Init();
		}

		public void Show()
		{
			Show(true);
		}

		public void Hide()
		{
			Show(false);
		}

		public void ResetView()
		{
			ActivateToggles();
			UpdateToggleEnableState();
		}

		public uint GetLayerMask()
		{
			uint result = 0;
			result |= _rockToggle.isOn ? (uint)PlanetMaterialType.Rock : 0;
			result |= _groundToggle.isOn ? (uint)PlanetMaterialType.Ground : 0;
			result |= _liquidToggle.isOn ? (uint)PlanetMaterialType.Liquid : 0;
			result |= _vegetationToggle.isOn ? (uint)PlanetMaterialType.Vegetation : 0;
			result |= (uint)PlanetMaterialType.Gas;
			return result;
		}

		private void Init()
		{
			AddListeners();
			ResetView();
			Hide();
		}

		private void AddListeners()
		{
			_vegetationToggle.onValueChanged.AddListener(OnToggleValueChanged);
			_liquidToggle.onValueChanged.AddListener(OnToggleValueChanged);
			_groundToggle.onValueChanged.AddListener(OnToggleValueChanged);
			_rockToggle.onValueChanged.AddListener(OnToggleValueChanged);
		}

		private void ActivateToggles()
		{
			_rockToggle.isOn = true;
			_groundToggle.isOn = true;
			_liquidToggle.isOn = true;
			_vegetationToggle.isOn = true;
		}

		private void UpdateToggleEnableState()
		{
			_rockToggle.interactable = false;
			_groundToggle.interactable = _rockToggle.isOn && !_liquidToggle.isOn;
            _liquidToggle.interactable = _groundToggle.isOn && !_vegetationToggle.isOn;
            _vegetationToggle.interactable = _liquidToggle.isOn;
        }

		private void OnToggleValueChanged(bool _)
		{
            UpdateToggleEnableState();
			OnToggleChanged?.Invoke();
		}

		private void Show(bool show)
		{
			_canvasGroup.alpha = show ? 1 : 0;
			_canvasGroup.blocksRaycasts = show;
		}
	}
}