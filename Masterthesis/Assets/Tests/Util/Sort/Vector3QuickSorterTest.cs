using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master.Test
{
	public class Vector3QuickSorterTest : Vector3SorterTest
	{
		protected override void BindSorter()
		{
			Container.Bind<Vector3Sorter>().To<Vector3QuickSorter>().AsTransient();
		}
	}
}