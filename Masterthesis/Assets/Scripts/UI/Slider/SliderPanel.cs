using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace SBaier.Master
{
    public class SliderPanel : MonoBehaviour
    {
        [SerializeField]
        private Slider _slider;
		public Slider Slider => _slider;
        [SerializeField]
        private TextMeshProUGUI _minLabel;
        [SerializeField]
        private TextMeshProUGUI _maxLabel;
        [SerializeField]
        private TextMeshProUGUI _valueLabel;

		private string _format => _slider.wholeNumbers ? string.Empty : "0.00";


		protected virtual void Start()
		{
			UpdateValueLabel();
			_slider.onValueChanged.AddListener(UpdateValueLabel);
		}

		protected virtual void Update()
		{
			UpdateRangeLabels();
		}

		private void UpdateValueLabel(float _)
		{
			UpdateValueLabel();
		}

		private void UpdateRangeLabels()
		{
			_minLabel.text = _slider.minValue.ToString(_format);
			_maxLabel.text = _slider.maxValue.ToString(_format);
		}

		private void UpdateValueLabel()
		{
			_valueLabel.text = _slider.value.ToString(_format);
		}

		public void Randomize(Seed seed)
		{
			float valueRange = _slider.maxValue - _slider.minValue;
			_slider.value = _slider.minValue + (float) seed.Random.NextDouble() * valueRange;
		}
	}
}