using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SBaier.Master
{
    public class PresetListEntry : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _presetName;
        [SerializeField]
        private Button _deleteButton;
        [SerializeField]
        private Button _loadPresetButton;
        [SerializeField]
        private Button _updatePresetButton;
        [SerializeField]
        private RectTransform _base;
        public RectTransform Base => _base;

        private PlanetParameterMenu _menu;
        private Preset _preset;

        public event Action<Preset> OnDelete;

        [Inject]
        private void Construct(Preset preset, PlanetParameterMenu menu)
		{
            _preset = preset;
            _menu = menu;
        }

		protected virtual void Start()
		{
            _deleteButton.onClick.AddListener(Delete);
            _loadPresetButton.onClick.AddListener(LoadPreset);
            _updatePresetButton.onClick.AddListener(UpdatePreset);
            _presetName.text = _preset.Name;
        }

		protected virtual void OnDestroy()
		{
            _deleteButton.onClick.RemoveListener(Delete);
            _loadPresetButton.onClick.RemoveListener(LoadPreset);
            _updatePresetButton.onClick.RemoveListener(UpdatePreset);
        }

        public void Destruct()
		{
            Destroy(_base.gameObject);
        }

		private void LoadPreset()
		{
            _menu.Load(_preset.Parameters);
        }

        private void Delete()
		{
            OnDelete?.Invoke(_preset);
        }

        private void UpdatePreset()
        {
            _preset.Update(_menu.CreateParameters());
        }

        public class Factory : PlaceholderFactory<Preset, PresetListEntry> { }
    }
}