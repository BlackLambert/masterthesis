using UnityEngine;
using Zenject;

namespace SBaier.Master
{
    public class IcosahedronInstaller : MonoInstaller
    {
        

        public override void InstallBindings()
        {
            Container.Bind<MeshGenerator>().To<IcosahedronGenerator>().AsTransient();
            
        }
    }
}