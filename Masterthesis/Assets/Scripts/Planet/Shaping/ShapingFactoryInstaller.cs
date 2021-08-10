using UnityEngine;
using Zenject;

namespace SBaier.Master
{
    public class ShapingFactoryInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<ShapingFactory>().AsTransient();
        }
    }
}