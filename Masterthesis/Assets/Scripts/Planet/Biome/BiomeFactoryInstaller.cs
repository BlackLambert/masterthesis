using UnityEngine;
using Zenject;

namespace SBaier.Master
{
    public class BiomeFactoryInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<BiomeFactory>().AsTransient();
        }
    }
}