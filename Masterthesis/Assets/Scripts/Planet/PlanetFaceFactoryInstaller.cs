using UnityEngine;
using Zenject;

namespace SBaier.Master
{
    public class PlanetFaceFactoryInstaller : MonoInstaller
    {
        [SerializeField]
        private PlanetFace _facePrefab;

        public override void InstallBindings()
        {
            Container.BindIFactory<PlanetFace>().FromComponentInNewPrefab(_facePrefab).AsTransient();
            Container.BindIFactory<MeshFaceSeparatorTarget>().FromComponentInNewPrefab(_facePrefab).AsTransient();
        }
    }
}