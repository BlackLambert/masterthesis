using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SBaier.Master
{
    public class AddPresetButton : MonoBehaviour
    {
        [SerializeField]
        private Button _button;
        [SerializeField]
        private TMP_InputField _nameInput;

        private Presets _presets;
		private PlanetParameterMenu _menu;

		[Inject]
        private void Construct(Presets presets,
            PlanetParameterMenu menu)
		{
            _presets = presets;
            _menu = menu;
        }

        protected virtual void Start()
		{
            _button.onClick.AddListener(OnClick);
        }

        protected virtual void OnDestroy()
		{
            _button.onClick.RemoveListener(OnClick);
        }

		private void OnClick()
		{
            string presetName = _nameInput.text;
            if (string.IsNullOrEmpty(presetName))
                return;
            Preset preset = new Preset(presetName, _menu.CreateParameters());
            _presets.Add(preset);
        }
	}
}