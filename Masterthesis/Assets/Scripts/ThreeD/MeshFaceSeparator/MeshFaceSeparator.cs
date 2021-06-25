using SBaier.Master;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace SBaier.Master
{
	public class MeshFaceSeparator
	{
		private IFactory<MeshFaceSeparatorTarget> _targetFactory;

		public MeshFaceSeparator(IFactory<MeshFaceSeparatorTarget> targetFactory)
		{
			_targetFactory = targetFactory;
		}

		public MeshFaceSeparatorTarget[] Separate(Mesh mesh)
		{
			Vector3[] meshVertices = mesh.vertices;
			int[] meshTriangles = mesh.triangles;

			int trianglesAmount = mesh.triangles.Length / 3;
			MeshFaceSeparatorTarget[] result = new MeshFaceSeparatorTarget[trianglesAmount];

			for (int i = 0; i < result.Length; i++)
				result[i] = CreateTarget(i, meshVertices, meshTriangles);

			return result;
		}

		private MeshFaceSeparatorTarget CreateTarget(int triangleIndex, Vector3[] meshVertices, int[] meshTriangles)
		{
			MeshFaceSeparatorTarget target = _targetFactory.Create();
			Mesh targetMesh = InitTargetMesh(target);
			targetMesh.vertices = CreateVertices(triangleIndex, meshTriangles, meshVertices);
			targetMesh.triangles = CreateVertexIndices();
			return target;
		}

		private Mesh InitTargetMesh(MeshFaceSeparatorTarget target)
		{
			if (target.MeshFilter.sharedMesh == null)
				target.MeshFilter.sharedMesh = new Mesh();
			return target.MeshFilter.sharedMesh;
		}

		private Vector3[] CreateVertices(int triangleIndex, int[] meshTriangles, Vector3[] meshVertices)
		{
			Vector3 v0 = meshVertices[meshTriangles[triangleIndex * 3]];
			Vector3 v1 = meshVertices[meshTriangles[triangleIndex * 3 + 1]];
			Vector3 v2 = meshVertices[meshTriangles[triangleIndex * 3 + 2]];
			return new Vector3[] { v0, v1, v2 };
		}

		private int[] CreateVertexIndices()
		{
			return new int[] { 0, 1, 2 };
		}
	}
}