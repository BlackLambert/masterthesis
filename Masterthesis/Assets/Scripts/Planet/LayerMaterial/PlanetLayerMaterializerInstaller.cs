using UnityEngine;
using Zenject;

namespace SBaier.Master
{
    public class PlanetLayerMaterializerInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<PlanetLayerMaterializer>().AsSingle();
        }
    }
}