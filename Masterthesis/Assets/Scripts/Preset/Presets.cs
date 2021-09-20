using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace SBaier.Master
{
    [Serializable]
    public class Presets 
    {
        public event Action<Preset> OnAdded;
        public event Action<Preset> OnRemoved;
        [JsonIgnore]
        public int Count => _presetList.Count;

        [JsonProperty]
        private List<Preset> _presetList;

        public Presets(List<Preset> presets)
		{
            _presetList = presets;
        }

        public void Add(Preset preset)
        {
            _presetList.Add(preset);
            OnAdded?.Invoke(preset);
        }

        public void Remove(Preset preset)
        {
            _presetList.Remove(preset);
            OnRemoved?.Invoke(preset);
        }

        public List<Preset> GetCopy()
		{
            return new List<Preset>(_presetList);
		}
    }
}