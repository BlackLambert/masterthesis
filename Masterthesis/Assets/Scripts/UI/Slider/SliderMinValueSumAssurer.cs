using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SBaier.Master
{
    public class SliderMinValueSumAssurer : MonoBehaviour
    {
        [SerializeField]
        private float _targetMinValueSum = 1;
        [SerializeField]
        private SliderPanel[] _sliders;

        protected virtual void Update()
		{
            float valueSum = _sliders.Sum(s => s.Slider.value);
            if (valueSum >= _targetMinValueSum)
                SetMinTo(0);
            else
            {
                float dif = (_targetMinValueSum - valueSum) / _sliders.Length;
                float delta = GetDelta(dif);
                SetMinTo(delta, dif);
            }
        }

		private float GetDelta(float dif)
		{
            float result = dif / _sliders.Length;
            if (!_sliders[0].Slider.wholeNumbers)
                return result;
            return (float) Mathf.CeilToInt(result);
        }

		private void SetMinTo(float value)
        {
            foreach (SliderPanel slider in _sliders)
                slider.Slider.minValue = value;
        }

        private void SetMinTo(float value, float target)
		{
            float sum = 0;
            foreach (SliderPanel slider in _sliders)
            {
                if (sum >= target)
                    break;
                slider.Slider.minValue = value;
                sum += value;
            }
		}
	}
}