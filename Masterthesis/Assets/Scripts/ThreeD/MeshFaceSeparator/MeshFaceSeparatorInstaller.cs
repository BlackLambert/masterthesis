using UnityEngine;
using Zenject;

namespace SBaier.Master
{
    public class MeshFaceSeparatorInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<MeshFaceSeparator>().AsTransient();
        }
    }
}