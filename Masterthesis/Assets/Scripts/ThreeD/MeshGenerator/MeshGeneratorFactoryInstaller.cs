using System;
using UnityEngine;
using Zenject;

namespace SBaier.Master
{
    public class MeshGeneratorFactoryInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Func<Vector3, int, float> compareValueSelect = CompareValueSelect;
            Container.Bind<QuickSelector<Vector3>>().To<QuickSorter<Vector3, float>>().AsTransient().WithArguments(compareValueSelect);
            Container.Bind<MeshGeneratorFactory>().To<MeshGeneratorFactoryImpl>().AsTransient();
        }

        private static float CompareValueSelect(Vector3 v, int i)
		{
			switch (i)
			{
				case 0:
					return v.x;
				case 1:
					return v.y;
				case 2:
					return v.z;
			}
			return v[i];
		}
    }
}