using UnityEngine;
using Zenject;

namespace SBaier.Master
{
    public class MeshSubdividerInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<MeshSubdivider>().To<RecursiveMeshSubdivider>().AsTransient();
        }
    }
}