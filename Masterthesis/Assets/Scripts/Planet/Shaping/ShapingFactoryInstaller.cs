using UnityEngine;
using Zenject;

namespace SBaier.Master
{
    public class ShapingFactoryInstaller : MonoInstaller
    {

        public override void InstallBindings()
        {
            Container.Bind<BaseShapingLayerFactory>().AsTransient();
            Container.Bind<SegmentShapingLayerFactory>().AsTransient();
            Container.Bind<PlatesShapingLayerFactory>().AsTransient();
            Container.Bind<ShapingFactory>().AsTransient();
        }
    }
}