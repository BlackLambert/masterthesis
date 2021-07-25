using UnityEngine;
using Zenject;

namespace SBaier.Master
{
    public class PlanetInstaller : MonoInstaller
    {
        [Inject]
        private PlanetData _planetData;
        [SerializeField]
        private Planet _planet;

        public override void InstallBindings()
        {
            Container.Bind<Planet>().FromInstance(_planet).AsSingle();
            Container.Bind<PlanetData>().FromInstance(_planetData).AsSingle();
            Container.Bind<PlanetAxisData>().FromInstance(_planetData.PlanetAxis).AsSingle();
            Container.Bind<PlanetDimensions>().FromInstance(_planetData.Dimensions).AsSingle();
        }
    }
}