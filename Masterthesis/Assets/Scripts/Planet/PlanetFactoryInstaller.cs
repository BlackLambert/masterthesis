using UnityEngine;
using Zenject;


namespace SBaier.Master
{
    public class PlanetFactoryInstaller : MonoInstaller
    {
        [SerializeField]
        private Planet _planetPrefab;

        public override void InstallBindings()
        {
            Container.BindFactory<PlanetData, Planet, Planet.Factory>().FromSubContainerResolve().ByNewContextPrefab<PlanetInstaller>(_planetPrefab);
        }
    }
}