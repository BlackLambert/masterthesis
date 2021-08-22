using System;
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

		public ShapingPrimitive[] _primitives;
		private bool _hasPrimitves;
		private Noise3D _noise;
		public Mode ShapingMode { get; }

		private float[] _weights;
		private float[] _evaluatedValues;
		private Vector3[] _vertices;

		public ShapingLayer(ShapingPrimitive[] primitives, KDTree<Vector3> primitivesTree, Noise3D noise, Mode mode)
		{
			_primitives = primitives;
			_hasPrimitves = _primitives.Length > 0;
			_noise = noise;
			_kDTree = primitivesTree;
			ShapingMode = mode;
			_maxAreaOfEffect = GetMaxAreaOfEffect();
		}

		private float GetMaxAreaOfEffect()
		{
			return _primitives.Length > 0 ? _primitives.Max(p => p.MaxAreaOfEffect) : 0;
		}

		public Result Evaluate(Vector3[] vertices)
		{
			if (!_hasPrimitves)
				return CreateEmptyResult(vertices.Length);
			Init(vertices);
			return CreateResult();
		}

		private Result CreateEmptyResult(int verticesAmount)
		{
			return new Result(new float[verticesAmount], new float[verticesAmount]);
		}

		private void Init(Vector3[] vertices)
		{
			_weights = new float[vertices.Length];
			_vertices = vertices;
		}

		private Result CreateResult()
		{
			EvaluateNoise();
			CalculateWeight();
			return new Result(_evaluatedValues, _weights);
		}

		private void EvaluateNoise()
		{
			NativeArray<Vector3> nativeVertices = new NativeArray<Vector3>(_vertices, Allocator.TempJob);
			NativeArray<float> evalPoints = _noise.Evaluate3D(nativeVertices);
			_evaluatedValues = evalPoints.ToArray();
			nativeVertices.Dispose();
			evalPoints.Dispose();
		}

		private void CalculateWeight()
		{
			int[][] primitivesInRange = _kDTree.GetNearestToWithin(_vertices, _maxAreaOfEffect);
			CalculateWeight(primitivesInRange);
			//int[] nearestPrimitive = _kDTree.GetNearestTo(_vertices);
			//for (int i = 0; i < nearestPrimitive.Length; i++)
			//	CalculateWeight(i, _primitives[nearestPrimitive[i]]);
		}

		private void CalculateWeight(int[][] primitivesInRange)
		{
			for (int i = 0; i < primitivesInRange.Length; i++)
				CalculateWeight(i, primitivesInRange[i]);
		}

		private void CalculateWeight(int vertexIndex, int[] primitivesInRange)
		{
			for (int i = 0; i < primitivesInRange.Length; i++)
				CalculateWeight(vertexIndex, _primitives[primitivesInRange[i]]);
		}

		private void CalculateWeight(int vertexIndex, ShapingPrimitive primitive)
		{
			Vector3 vertex = _vertices[vertexIndex];
			float formerWeight = _weights[vertexIndex];
			float evaluatedWeight = primitive.Evaluate(vertex);
			float value = Mathf.Max(formerWeight, evaluatedWeight);
			_weights[vertexIndex] = value;
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
			Substract = 2,
			Min = 4,
			Max = 8
		}
	}
}