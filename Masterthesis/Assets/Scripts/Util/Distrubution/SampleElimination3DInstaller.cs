using UnityEngine;
using Zenject;

namespace SBaier.Master
{
    public class SampleElimination3DInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<SampleElimination3D>().AsTransient();
        }
    }
}