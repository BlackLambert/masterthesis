using UnityEngine;
using Zenject;

namespace SBaier.Master
{
    public class PlanetLayerMaterialSerializerInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<PlanetLayerMaterialSerializer>().AsTransient();
        }
    }
}