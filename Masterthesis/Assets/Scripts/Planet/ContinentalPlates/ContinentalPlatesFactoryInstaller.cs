using UnityEngine;
using Zenject;

namespace SBaier.Master
{
    public class ContinentalPlatesFactoryInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<ContinentalPlatesFactory>().AsTransient();
        }
    }
}