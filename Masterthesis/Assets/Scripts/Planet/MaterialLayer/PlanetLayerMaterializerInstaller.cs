using UnityEngine;
using Zenject;

namespace SBaier.Master
{
    public class PlanetLayerMaterializerInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<PlanetRockLayerAdder>().AsTransient();
            Container.Bind<PlanetGroundLayerAdder>().AsTransient();
            Container.Bind<PlanetGroundVegetationLayerAdder>().AsTransient();
            Container.Bind<PlanetLiquidLayerAdder>().AsTransient();
            Container.Bind<PlanetAirLayerAdder>().AsTransient();
            Container.Bind<PlanetLayerMaterializer>().AsTransient();
        }
    }
}