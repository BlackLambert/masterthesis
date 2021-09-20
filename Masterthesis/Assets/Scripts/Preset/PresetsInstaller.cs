using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace SBaier.Master
{
    public class PresetsInstaller : MonoInstaller
    {
        [SerializeField]
        private PresetListEntry _entry;
        [SerializeField]
        private PresetsFileLoader _loader;

        public override void InstallBindings()
        {
            Container.BindFactory<Preset, PresetListEntry, PresetListEntry.Factory>().FromComponentInNewPrefab(_entry);
            Presets loadedPresets = _loader.Load();
            if (loadedPresets == null)
                loadedPresets = new Presets(new List<Preset>());
            Container.Bind<Presets>().FromInstance(loadedPresets).AsSingle();
        }
    }
}