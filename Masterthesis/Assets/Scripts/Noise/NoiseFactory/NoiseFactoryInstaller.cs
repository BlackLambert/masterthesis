using UnityEngine;
using Zenject;

namespace SBaier.Master
{
    public class NoiseFactoryInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<NoiseFactoryImpl>().To<NoiseFactoryImpl>().AsTransient();
        }
    }
}