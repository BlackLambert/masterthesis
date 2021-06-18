using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace SBaier.Master.Test
{
    [TestFixture]
    public class RecursiveMeshSubdividerTest : MeshSubdividerTest
    {

        private const int _additionalTrianglesPerSubdivision = 4;
		private const int _subdivisionsPerEdge = 2;

		protected override void GivenAMeshSubdivider()
		{
			Container.Bind<MeshSubdivider>().To<RecursiveMeshSubdivider>().AsTransient();
		}

		protected override int GetExpectedTrianglesAmountFor(int amount)
		{
            return (int)(Math.Pow(_additionalTrianglesPerSubdivision, amount) * (_testVertexIndices.Length/3));
		}

		protected override int GetVerticesPerEdgeFor(int amount)
		{
			return 1 + (int)Math.Pow(_subdivisionsPerEdge, amount);
		}
	}
}