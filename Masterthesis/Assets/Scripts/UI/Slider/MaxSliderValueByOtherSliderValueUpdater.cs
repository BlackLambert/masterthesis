using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public class MaxSliderValueByOtherSliderValueUpdater : MonoBehaviour
    {
        [SerializeField]
        private SliderPanel _targetSlider;
        [SerializeField]
        private SliderPanel _otherSlider;

		protected virtual void Start()
        {
            _otherSlider.Slider.onValueChanged.AddListener(OnOtherValueChanged);
            UpdateMaxValue();
        }

        protected virtual void OnDestroy()
        {
            _otherSlider.Slider.onValueChanged.RemoveListener(OnOtherValueChanged);
        }

        private void OnOtherValueChanged(float arg0)
        {
            UpdateMaxValue();
        }

        private void UpdateMaxValue()
        {
            _targetSlider.Slider.maxValue = _otherSlider.Slider.value;
        }
    }
}