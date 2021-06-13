using UnityEngine;
using Zenject;

namespace SBaier.Master
{
    public class SphereMeshFormerInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<MeshFormer>().To<SphereMeshFormer>().AsTransient();
        }
    }
}