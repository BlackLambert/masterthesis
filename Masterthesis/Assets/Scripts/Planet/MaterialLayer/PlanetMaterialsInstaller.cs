using UnityEngine;
using Zenject;

namespace SBaier.Master
{
    public class PlanetMaterialsInstaller : MonoInstaller
    {
        [SerializeField]
        private PlanetLayerMaterialSettings[] _materials;

        public override void InstallBindings()
        {
            Container.Bind<PlanetLayerMaterialSettings[]>().FromInstance(_materials).AsSingle();
        }
    }
}