using UnityEngine;
using Zenject;

namespace SBaier.Master
{
    public class BiomeOccurrenceSerializerInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<BiomeOccurrenceSerializer>().AsTransient();
        }
    }
}