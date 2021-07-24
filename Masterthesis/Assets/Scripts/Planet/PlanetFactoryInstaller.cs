using UnityEngine;
using Zenject;


namespace SBaier.Master
{
    public class PlanetFactoryInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindFactory<UnityEngine.Object, Planet, Planet.Factory>().FromFactory<PrefabFactory<Planet>>();
        }
    }
}