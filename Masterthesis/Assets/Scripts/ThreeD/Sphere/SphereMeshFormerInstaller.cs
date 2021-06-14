using UnityEngine;
using Zenject;

namespace SBaier.Master
{
    public class SphereMeshFormerInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind(typeof(MeshFormer), typeof(SphereMeshFormer)).To<SphereMeshFormer>().AsTransient();
        }
    }
}