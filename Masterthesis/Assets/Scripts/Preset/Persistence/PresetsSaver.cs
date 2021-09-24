using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace SBaier.Master
{
    public class PresetsSaver : FileSaver<Presets>
    {
		private Presets _presets;

		[Inject]
        public void Construct(Presets presets)
		{
            _presets = presets;
        }

		protected override object GetSerializableObject()
		{
			return _presets;
		}
	}
}