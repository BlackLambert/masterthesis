using System;
using UnityEngine;
using Zenject;

namespace SBaier.Master
{
    public class Vector3QuickSelectorInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
			Func<Vector3, int, float> compareValueSelect = CompareValueSelect;
			Container.Bind(typeof(QuickSelector<Vector3>), typeof(QuickSorter<Vector3, float>)).To<QuickSorter<Vector3, float>>().AsTransient().WithArguments(compareValueSelect);
		}

		private float CompareValueSelect(Vector3 v, int i)
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