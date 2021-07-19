using Zenject;
using NUnit.Framework;
using UnityEngine;
using System;

namespace SBaier.Master.Test
{
	[TestFixture]
	public class Vector3SelectionSorterTest : Vector3SorterTest
	{
		protected override void BindSorter()
		{
			Func<Vector3, int, float> compareValueSelect = (p, i) => p[i];
			Container.Bind<Sorter<Vector3>>().To<SelectionSorter<Vector3, float>>().AsTransient().WithArguments(compareValueSelect);
		}
	}
}