using UnityEngine;
using Zenject;

namespace SBaier.Master
{
    public class PlainInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<MeshGenerator>().To<PlainGenerator>().AsTransient();
        }
    }
}