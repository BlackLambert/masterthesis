using UnityEngine;
using Zenject;

namespace SBaier.Master
{
    public class Noise3DToSpheralMeshApplierInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<Noise3DToSpheralMeshApplier>().AsTransient();
        }
    }
}