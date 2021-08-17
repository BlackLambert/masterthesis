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
		public Mode ShapingMode { get; }


		public ShapingLayer(ShapingPrimitive[] primitives, KDTree<Vector3> primitivesTree, Noise3D noise, Mode mode)
		{
			Primitives = primitives;
			Noise = noise;
			_kDTree = primitivesTree;
			_maxAreaOfEffect = Primitives.Length > 0 ? Primitives.Max(p => p.MaxAreaOfEffect) : 0;
			ShapingMode = mode;
		}

		public Result Evaluate(Vector3[] vertices)
		{
			float[] weights = new float[vertices.Length];
			float[] evaluatedValues = new float[vertices.Length];

			if (Primitives.Length == 0)
				return new Result(evaluatedValues, weights);

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
					float formerWeight = weights[i];
					float evaluatedWeight = primitive.Evaluate(vertex);
					float value = Mathf.Max(formerWeight, evaluatedWeight);
					weights[i] = value;
				}

				evaluatedValues[i] = evalPoints[i];
			}
			evalPoints.Dispose();
			return new Result(evaluatedValues, weights);
		}

		public class Result
		{
			public Result(float[] evaluatedValues,
				float[] weight)
			{
				EvaluatedValues = evaluatedValues;
				Weight = weight;
			}

			public float[] EvaluatedValues { get; }
			public float[] Weight { get; }
		}

		public enum Mode
		{
			Blend = 0,
			Add = 1,
			Substract = 2
		}
	}
}