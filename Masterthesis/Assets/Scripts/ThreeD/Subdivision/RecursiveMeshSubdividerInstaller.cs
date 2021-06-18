using UnityEngine;
using Zenject;

namespace SBaier.Master
{
    public class RecursiveMeshSubdividerInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<MeshSubdivider>().To<RecursiveMeshSubdivider>().AsTransient();
        }
    }
}