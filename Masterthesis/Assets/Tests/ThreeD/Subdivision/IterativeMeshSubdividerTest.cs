using Zenject;
using NUnit.Framework;
using System.Collections.Generic;

namespace SBaier.Master.Test
{
	[TestFixture]
	public class IterativeMeshSubdividerTest : MeshSubdividerTest
	{
		protected override int GetExpectedTrianglesAmountFor(int amount)
		{
			int result = 0;
			for (int i = 0; i <= amount; i++)
				result += 1 + (2 * i);
			return result * (_testVertexIndices.Length / 3);
		}

		protected override int GetVerticesPerEdgeFor(int amount)
		{
			return amount + 2;
		}

		protected override void GivenAMeshSubdivider()
		{
			Container.Bind<DuplicateVerticesRemover>().AsTransient();
			Container.Bind<MeshSubdivider>().To<IterativeMeshSubdivider>().AsTransient();
		}
	}
}