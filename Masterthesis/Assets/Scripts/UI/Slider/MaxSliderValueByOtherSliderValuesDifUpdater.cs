using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SBaier.Master
{
    public class MaxSliderValueByOtherSliderValuesDifUpdater : MonoBehaviour
    {
        [SerializeField]
        private SliderPanel _targetSlider;
        [SerializeField]
        private SliderPanel _baseSlider;
        [SerializeField]
        private SliderPanel[] _difSliders;
        [SerializeField]
        private float _minimalMaxValue = 1;

        protected virtual void Start()
        {
            foreach (SliderPanel panel in _difSliders)
                panel.Slider.onValueChanged.AddListener(OnOtherValueChanged);
            _baseSlider.Slider.onValueChanged.AddListener(OnOtherValueChanged);
            UpdateMaxValue();
        }

        protected virtual void OnDestroy()
        {
            foreach (SliderPanel panel in _difSliders)
                panel.Slider.onValueChanged.RemoveListener(OnOtherValueChanged);
            _baseSlider.Slider.onValueChanged.RemoveListener(OnOtherValueChanged);
        }

        private void OnOtherValueChanged(float arg0)
        {
            UpdateMaxValue();
        }

		private void UpdateMaxValue()
		{
			float baseValue = _baseSlider.Slider.value;
			float sum = _difSliders.Sum(s => s.Slider.value);
			_targetSlider.Slider.maxValue = Mathf.Max(0, baseValue - (sum + _minimalMaxValue));
		}
	}
}