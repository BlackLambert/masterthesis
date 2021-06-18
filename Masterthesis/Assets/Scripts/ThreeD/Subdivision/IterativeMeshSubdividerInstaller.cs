using UnityEngine;
using Zenject;

namespace SBaier.Master
{
    public class IterativeMeshSubdividerInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<DuplicateVerticesRemover>().AsTransient();
            Container.Bind<MeshSubdivider>().To<IterativeMeshSubdivider>().AsTransient();
        }
    }
}