using UnityEngine;
using Zenject;

namespace SBaier.Master
{
    public class PresetsInstaller : MonoInstaller
    {
        [SerializeField]
        private PresetListEntry _entry;

        public override void InstallBindings()
        {
            Container.BindFactory<Preset, PresetListEntry, PresetListEntry.Factory>().FromComponentInNewPrefab(_entry);
            Container.Bind<Presets>().AsSingle();
        }
    }
}