using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace SBaier.Master
{
    public class PresetList : MonoBehaviour
    {
        private Dictionary<Preset, PresetListEntry> _entries;
		private PresetListEntry.Factory _factory;
		private Presets _presets;

		[SerializeField]
		private RectTransform _hook;

		[Inject]
		public void Construct(PresetListEntry.Factory factory,
			Presets presets)
		{
			_factory = factory;
			_presets = presets;
		}

		protected void Start()
		{
			_entries = new Dictionary<Preset, PresetListEntry>();
			_presets.OnAdded += OnPresetAdded;
			_presets.OnRemoved += OnPresetRemoved;
			Init();
		}

		protected void OnDestroy()
		{
			foreach (KeyValuePair<Preset, PresetListEntry> entry in new Dictionary<Preset, PresetListEntry>(_entries))
				Remove(entry.Key);
			_presets.OnAdded -= OnPresetAdded;
			_presets.OnRemoved -= OnPresetRemoved;
		}

		public void Init()
		{
			List<Preset> presets = _presets.GetCopy();
			foreach (Preset preset in presets)
				Add(preset);
		}

		private void OnPresetAdded(Preset preset)
		{
			Add(preset);
		}

		private void OnPresetRemoved(Preset preset)
		{
			Remove(preset);
		}

		private void Add(Preset preset)
		{
			PresetListEntry newEntry = _factory.Create(preset);
			newEntry.Base.SetParent(_hook);
			newEntry.transform.localScale = Vector3.one;
			newEntry.OnDelete += Delete;
			_entries.Add(preset, newEntry);
		}

		private void Remove(Preset preset)
		{
			PresetListEntry entry = _entries[preset];
			_entries.Remove(preset);
			entry.OnDelete -= Delete;
			entry.Destruct();
		}

		private void Delete(Preset preset)
		{
			_presets.Remove(preset);
		}
	}
}