using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public class Presets 
    {
        public event Action<Preset> OnAdded;
        public event Action<Preset> OnRemoved;
        private List<Preset> _presetList { get; }

        public Presets(List<Preset> presets)
		{
            _presetList = presets;
        }

        public void Add(Preset preset)
        {
            OnAdded?.Invoke(preset);
        }

        public void Remove(Preset preset)
		{
            OnRemoved?.Invoke(preset);
        }

        public List<Preset> GetCopy()
		{
            return new List<Preset>(_presetList);
		}
    }
}