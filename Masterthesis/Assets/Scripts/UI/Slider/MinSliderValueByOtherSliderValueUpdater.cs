using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public class MinSliderValueByOtherSliderValueUpdater : MonoBehaviour
    {
        [SerializeField]
        private SliderPanel _targetSlider;
        [SerializeField]
        private SliderPanel _otherSlider;
        [SerializeField]
        private float _minimalMinValue = 1;

        protected virtual void Start()
		{
            _otherSlider.Slider.onValueChanged.AddListener(OnOtherValueChanged);
			UpdateMinValue();

		}

        protected virtual void OnDestroy()
		{
            _otherSlider.Slider.onValueChanged.RemoveListener(OnOtherValueChanged);
		}

		private void OnOtherValueChanged(float arg0)
		{
			UpdateMinValue();
		}

		private void UpdateMinValue()
		{
			_targetSlider.Slider.minValue = _otherSlider.Slider.value + _minimalMinValue;
		}
	}
}