using Zenject;
using System.Collections;
using UnityEngine.TestTools;
using UnityEngine;
using System;
using NUnit.Framework;

namespace SBaier.Master.Test
{
    public class MeshNormalsOnStartRecalculatorTest : ZenjectIntegrationTestFixture
    {
		private const string _prefabPath = "ThreeD/TestMeshNormalsOnStartRecalculator";
		private readonly Vector3[] _testVertices = new Vector3[]
		{
			new Vector3(0, 0 ,0),
			new Vector3(1, 2, 0),
			new Vector3(0, 2, 3),
			new Vector3(-1, 3, 2),
			new Vector3(-1, -1, -1),
		};

		private readonly Vector3[] _newVertices = new Vector3[]
		{
			Vector3.zero,
			new Vector3(1, 2, 0),
			new Vector3(0, 2, 0),
			new Vector3(-1, 3, 0),
			new Vector3(-1, -1, 0),
		};

		private readonly int[] _testTriangles = new int[]
		{
			0, 1, 2,
			1, 2, 3,
			0, 1, 4,
			2, 3, 4
		};

		private readonly Vector3 _expectedNormalValue = Vector3.forward;

		private Mesh _mesh;
		private MeshNormalsOnStartRecalculator _recalculator;

		[UnityTest]
        public IEnumerator RecalcualtesNormalsOnStart()
		{
			GivenADefaultSetup();
			WhenVerticesChange();
			yield return 0;
			ThenAllNormalsEqual(_expectedNormalValue);
		}

		private void GivenADefaultSetup()
		{
			PreInstall();
			Container.Bind<Mesh>().FromMethod(CreateTestMesh).AsSingle();
			Container.Bind<MeshNormalsOnStartRecalculator>().FromComponentInNewPrefabResource(_prefabPath).AsTransient();
			PostInstall();

			_mesh = Container.Resolve<Mesh>();
			_recalculator = Container.Resolve<MeshNormalsOnStartRecalculator>();
			MeshFilter meshFilter = _recalculator.GetComponent<MeshFilter>();
			meshFilter.sharedMesh = _mesh;
		}

		private Mesh CreateTestMesh()
		{
			Mesh result = new Mesh();
			result.vertices = _testVertices;
			result.triangles = _testTriangles;
			result.RecalculateNormals();
			return result;
		}

		private void WhenVerticesChange()
		{
			_mesh.vertices = _newVertices;
		}

		private void ThenAllNormalsEqual(Vector3 expected)
		{
			foreach(Vector3 normal in _mesh.normals)
			{
				Assert.AreEqual(expected.x, normal.x);
				Assert.AreEqual(expected.y, normal.y);
				Assert.AreEqual(expected.z, normal.z);
			}
		}
	}
}