using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public class SealevelSlider : MonoBehaviour
    {
        [SerializeField]
        private float _minimalMin = 0.01f;
        [SerializeField]
        private float _minimalMax = 0.01f;
        [SerializeField]
        private SliderPanel _bedrockSlider;
        [SerializeField]
        private SliderPanel _maxHullSlider;
        [SerializeField]
        private SliderPanel _slider;

		protected virtual void Update()
		{
			UpdateValueRange();
		}

		private void UpdateValueRange()
		{
			_slider.Slider.minValue = _bedrockSlider.Slider.value + _minimalMin;
			_slider.Slider.maxValue = _maxHullSlider.Slider.value - _minimalMax;
        }
    }
}