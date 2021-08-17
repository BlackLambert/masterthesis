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

        protected virtual void Update()
        {
            _targetSlider.Slider.maxValue = _otherSlider.Slider.value;
        }
    }
}