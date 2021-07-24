using Zenject;
using NUnit.Framework;

namespace SBaier.Master.Test
{
    [TestFixture]
    public class IcosahedronTest : ZenjectUnitTestFixture
    {
        [Test]
        public void ConnectedEdges_RightEdgesAreConnected()
        {
            for(int i = 0; i < Icosahedron.ConnectedEdges.Length; i++)
			{
                Icosahedron.ConnectedEdge[] edges = Icosahedron.ConnectedEdges[i];
                for (int j = 0; j < edges.Length; j++)
				{
                    Icosahedron.ConnectedEdge connectedEdge = edges[j];
                    Icosahedron.ConnectedEdge otherEdge = Icosahedron.ConnectedEdges[connectedEdge.FaceIndex][connectedEdge.EdgeIndex];
                    Assert.AreEqual(i, otherEdge.FaceIndex, $"FaceIndex: {i} | EdgeIndex {j}");
                    Assert.AreEqual(j, otherEdge.EdgeIndex, $"FaceIndex: {i} | EdgeIndex {j}");
				}
			}
        }

        [Test]
        public void ConnectedEdges_CorrectEdgesAreFlaggedWithAligned()
        {
            for (int i = 0; i < Icosahedron.ConnectedEdges.Length; i++)
            {
                Icosahedron.ConnectedEdge[] edges = Icosahedron.ConnectedEdges[i];
                for (int j = 0; j < edges.Length; j++)
                {
                    Icosahedron.ConnectedEdge connectedEdge = edges[j];
                    bool expected = j != connectedEdge.EdgeIndex && (j == 1 || connectedEdge.EdgeIndex == 1);
                    Assert.AreEqual(expected, connectedEdge.Aligned, $"FaceIndex: {i} | EdgeIndex {j} | EdgeIndex other: {connectedEdge.EdgeIndex}");
                }
            }
        }
    }
}