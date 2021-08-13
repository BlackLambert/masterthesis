using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public class MaxHullRadiusSlider : MonoBehaviour
    {
        [SerializeField]
        private float _minimalMin = 0.1f;
        [SerializeField]
        private SliderPanel _bedrockSlider;
        [SerializeField]
        private SliderPanel _slider;

        protected virtual void Update()
		{
            _slider.Slider.minValue = _bedrockSlider.Slider.value + _minimalMin;
        }
    }
}