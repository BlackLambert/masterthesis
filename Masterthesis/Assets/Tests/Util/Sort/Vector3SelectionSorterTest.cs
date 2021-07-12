using Zenject;
using NUnit.Framework;

namespace SBaier.Master.Test
{
	[TestFixture]
	public class Vector3SelectionSorterTest : Vector3SorterTest
	{
		protected override void BindSorter()
		{
			Container.Bind<Vector3Sorter>().To<Vector3SelectionSorter>().AsTransient();
		}
	}
}