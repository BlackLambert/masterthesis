using UnityEngine;
using Zenject;

namespace SBaier.Master
{
    public class MeshGeneratorFactoryInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<Vector3QuickSelector>().To<Vector3QuickSorter>().AsTransient();
            Container.Bind<MeshGeneratorFactory>().To<MeshGeneratorFactoryImpl>().AsTransient();
        }
    }
}