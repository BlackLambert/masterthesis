using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SBaier.Master
{
    public class MaxSliderValueByOtherSliderValuesSumUpdater : MonoBehaviour
    {
        [SerializeField]
        private SliderPanel _targetSlider;
        [SerializeField]
        private SliderPanel[] _otherSliders;
        [SerializeField]
        private float _minimalMaxValue = 1;

        protected virtual void Update()
        {
            float sum = _otherSliders.Sum(s => s.Slider.value);
            _targetSlider.Slider.maxValue = sum - _minimalMaxValue;
        }
    }
}