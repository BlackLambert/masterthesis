using UnityEngine;
using Zenject;

namespace SBaier.Master
{
    public class Vector3BinaryKDTreeFactoryInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<Vector3BinaryKDTreeFactory>().AsTransient();
        }
    }
}