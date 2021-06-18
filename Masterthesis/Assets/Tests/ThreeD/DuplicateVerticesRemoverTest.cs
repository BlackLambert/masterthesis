using Zenject;
using NUnit.Framework;
using System;
using UnityEngine;

namespace SBaier.Master.Test
{
    [TestFixture]
    public class DuplicateVerticesRemoverTest : ZenjectUnitTestFixture
    {
		private Vector3[][] _testVertices = new Vector3[][]
		{
			new Vector3[]{Vector3.zero, Vector3.up, Vector3.forward, Vector3.zero, Vector3.down, Vector3.forward},
			new Vector3[]{Vector3.zero, Vector3.up, Vector3.right, Vector3.right, new Vector3(1, 1, 0), Vector3.up},
			new Vector3[]{
				new Vector3(-2.6f, -5.2f, 1.5f), 
				new Vector3(7.8f, -0.2f, 5.4f), 
				new Vector3(1.2f, 4.3f, -0.5f), 
				new Vector3(1.2f, 4.3f, -0.5f), 
				new Vector3(7.8f, -0.2f, 5.4f), 
				new Vector3(0, -6.6f, -2.5f),
				new Vector3(1.2f, 4.3f, -0.5f),
				new Vector3(0, -6.6f, -2.5f),
				new Vector3(-4.1f, 10.3f, 3.9f)}
		};

		private Vector3[][] _expectedVertices = new Vector3[][]
		{
			new Vector3[]{Vector3.zero, Vector3.up, Vector3.forward, Vector3.down},
			new Vector3[]{Vector3.zero, Vector3.up, Vector3.right, new Vector3(1, 1, 0)},
			new Vector3[]{
				new Vector3(-2.6f, -5.2f, 1.5f), 
				new Vector3(7.8f, -0.2f, 5.4f), 
				new Vector3(1.2f, 4.3f, -0.5f), 
				new Vector3(0, -6.6f, -2.5f), 
				new Vector3(-4.1f, 10.3f, 3.9f)}
		};

		private int[][] _testVertexIndices = new int[][]
		{
			new int[]{0, 1, 2, 3, 4, 5},
			new int[]{0, 1, 2, 3, 4, 5},
			new int[]{0, 1, 2, 3, 4, 5, 6, 7, 8}
		};

		private int[][] _expectedVertexIndices = new int[][]
		{
			new int[]{0, 1, 2, 0, 3, 2},
			new int[]{0, 1, 2, 2, 3, 1},
			new int[]{0, 1, 2, 2, 1, 3, 2, 3, 4,}
		};


		private DuplicateVerticesRemover _remover;
		private Mesh _testMesh;

		[Test(Description = "RunOn removes all doublicate vertices from the testMesh")]
        public void RunOn_RemovesAllDoublicateVerticesFromTestMesh()
        {
			for (int i = 0; i < _testVertices.Length; i++)
			{
				GivenADefaultSetup(_testVertices[i], _testVertexIndices[i]);
				WhenRunOnCalledOn(_testMesh);
				ThenMeshHasExpectedVertices(_expectedVertices[i]);
				Teardown();
				Setup();
			}
        }

		[Test(Description = "RunOn changes triangle vertex indices according to the vertex changes")]
        public void RunOn_ChangesTrianglesAccordingToTheVertexChanges()
        {
			for (int i = 0; i < _testVertices.Length; i++)
			{
				GivenADefaultSetup(_testVertices[i], _testVertexIndices[i]);
				WhenRunOnCalledOn(_testMesh);
				ThenMeshHasExpectedVertexIndices(_expectedVertexIndices[i]);
				Teardown();
				Setup();
			}
        }

		private void GivenADefaultSetup(Vector3[] vertices, int[] vertexIndices)
		{
			Container.Bind<Mesh>().FromMethod(() => CreateMesh(vertices, vertexIndices)).AsSingle();
            Container.Bind<DuplicateVerticesRemover>().To<DuplicateVerticesRemover>().AsTransient();

            _remover = Container.Resolve<DuplicateVerticesRemover>();
			_testMesh = Container.Resolve<Mesh>(); 
		}

		private void WhenRunOnCalledOn(Mesh testMesh)
		{
			_remover.RunOn(testMesh);
		}

		private void ThenMeshHasExpectedVertices(Vector3[] expected)
		{
			Assert.AreEqual(expected, _testMesh.vertices);
		}

		private void ThenMeshHasExpectedVertexIndices(int[] expected)
		{
			Assert.AreEqual(expected, _testMesh.triangles);
		}

		private Mesh CreateMesh(Vector3[] vertices, int[] vertexIndices)
		{
			Mesh result = new Mesh();
			result.vertices = vertices;
			result.triangles = vertexIndices;
			return result;
		}
	}
}