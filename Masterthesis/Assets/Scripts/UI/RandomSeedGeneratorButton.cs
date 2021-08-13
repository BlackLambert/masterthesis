using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SBaier.Master
{
    public class RandomSeedGeneratorButton : MonoBehaviour
    {
        [SerializeField]
        private TextInputPanel _seedPanel;
        [SerializeField]
        private Button _button;

        protected virtual void Start()
		{
            _button.onClick.AddListener(GenerateSeed);
        }

        protected virtual void OnDestroy()
		{
            _button.onClick.RemoveListener(GenerateSeed);
        }

		private void GenerateSeed()
		{
            int seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
            _seedPanel.InputField.text = seed.ToString();
        }
	}
}