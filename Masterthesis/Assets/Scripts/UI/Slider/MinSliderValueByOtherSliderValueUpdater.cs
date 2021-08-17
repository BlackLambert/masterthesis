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

        protected virtual void Update()
        {
            _targetSlider.Slider.minValue = _otherSlider.Slider.value + _minimalMinValue;
        }
    }
}