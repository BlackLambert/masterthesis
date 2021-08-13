using UnityEngine;
using Zenject;

namespace SBaier.Master
{
    public class BasicPlanetFactoryInstaller : MonoInstaller
    {
        [SerializeField]
        private IcosahedronGeneratorSettings _settings;
        [Inject]
        private MeshGeneratorFactory _meshGeneratorFactory;

        public override void InstallBindings()
        {
            IcosahedronGenerator generator = _meshGeneratorFactory.Create(_settings) as IcosahedronGenerator;
            Container.Bind<BasicPlanetFactory>().AsTransient().WithArguments(generator);
        }
    }
}