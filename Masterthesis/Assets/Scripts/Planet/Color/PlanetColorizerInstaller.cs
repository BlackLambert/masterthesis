using UnityEngine;
using Zenject;

namespace SBaier.Master
{
    public class PlanetColorizerInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<PlanetColorizer>().AsTransient();
        }
    }
}