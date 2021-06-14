using UnityEngine;
using Zenject;

namespace SBaier.Master
{
    public class MeshGeneratorFactoryInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<MeshGeneratorFactory>().To<MeshGeneratorFactoryImpl>().AsTransient();
        }
    }
}