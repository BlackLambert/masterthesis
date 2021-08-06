using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

namespace SBaier.Master
{
    public class ShapingLayer
    {
		private float _maxAreaOfEffect;
		private KDTree<Vector3> _kDTree;

		public ShapingPrimitive[] Primitives { get; }
		public Noise3D Noise { get; }

		public ShapingLayer(ShapingPrimitive[] primitives, KDTree<Vector3> primitivesTree, Noise3D noise)
		{
			Primitives = primitives;
			Noise = noise;
			_kDTree = primitivesTree;
			_maxAreaOfEffect = Primitives.Max(p => p.MaxAreaOfEffect);
		}

		public float[] Evaluate(Vector3[] vertices)
		{
			float[] result = new float[vertices.Length];
			NativeArray<Vector3> nativeVertices = new NativeArray<Vector3>(vertices, Allocator.TempJob);
			NativeArray<float> evalPoints = Noise.Evaluate3D(nativeVertices);
			nativeVertices.Dispose();
			for (int i = 0; i < vertices.Length; i++)
			{
				Vector3 vertex = vertices[i];
				IList<int> nearestPrimitives = _kDTree.GetNearestToWithin(vertex, _maxAreaOfEffect);
				int count = nearestPrimitives.Count;
				for (int j = 0; j < count; j++)
				{
					ShapingPrimitive primitive = Primitives[nearestPrimitives[j]];
					result[i] = Mathf.Max(result[i], primitive.Evaluate(vertex));
				}
				result[i] *= evalPoints[i];
			}
			evalPoints.Dispose();
			return result;
		}
	}
}