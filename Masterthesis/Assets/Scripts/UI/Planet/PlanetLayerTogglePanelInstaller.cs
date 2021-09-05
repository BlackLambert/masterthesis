using UnityEngine;
using Zenject;

namespace SBaier.Master
{
    public class PlanetLayerTogglePanelInstaller : MonoInstaller
    {
        [SerializeField]
        private PlanetLayerTogglePanel _layerTogglePanel;

        public override void InstallBindings()
        {
            Container.Bind<PlanetLayerTogglePanel>().FromInstance(_layerTogglePanel).AsSingle();
        }
    }
}