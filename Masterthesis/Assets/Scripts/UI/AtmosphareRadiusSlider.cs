using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public class AtmosphareRadiusSlider : MonoBehaviour
    {
        [SerializeField]
        private float _minimalMin = 0.1f;
        [SerializeField]
        private SliderPanel _maxHullSlider;
        [SerializeField]
        private SliderPanel _slider;

        protected virtual void Update()
		{
            _slider.Slider.minValue = _maxHullSlider.Slider.value + _minimalMin;
        }
    }
}