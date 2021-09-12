using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SBaier.Master
{
    public class MinSliderValueByOtherSliderValuesSumUpdater : MonoBehaviour
    {
        [SerializeField]
        private SliderPanel _targetSlider;
        [SerializeField]
        private SliderPanel[] _otherSliders;
        [SerializeField]
        private float _minimalMinValue = 1;

        protected virtual void Start()
        {
			foreach (SliderPanel panel in _otherSliders)
                panel.Slider.onValueChanged.AddListener(OnOtherValueChanged);
            UpdateMinValue();

        }

        protected virtual void OnDestroy()
        {
            foreach (SliderPanel panel in _otherSliders)
                panel.Slider.onValueChanged.RemoveListener(OnOtherValueChanged);
        }

        private void OnOtherValueChanged(float arg0)
        {
            UpdateMinValue();
        }

		private void UpdateMinValue()
		{
			float sum = _otherSliders.Sum(s => s.Slider.value);
			_targetSlider.Slider.minValue = sum + _minimalMinValue;
		}
	}
}