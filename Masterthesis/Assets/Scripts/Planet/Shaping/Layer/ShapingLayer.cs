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
		private float _squaredMaxAreaOfEffect;

		public ShapingPrimitive[] _primitives;
		private bool _hasPrimitves;
		private Noise3D _noise;
		public Mode ShapingMode { get; }

		private float[] _weights;
		private float[] _evaluatedValues;
		private Vector3[] _vertices;
		private KDTree<Vector3> _verticesTree;

		public ShapingLayer(ShapingPrimitive[] primitives, Noise3D noise, Mode mode)
		{
			_primitives = primitives;
			_hasPrimitves = _primitives.Length > 0;
			_noise = noise;
			ShapingMode = mode;
			_squaredMaxAreaOfEffect = _maxAreaOfEffect * _maxAreaOfEffect;
		}

		public Result Evaluate(Vector3[] vertices, KDTree<Vector3> tree)
		{
			if (!_hasPrimitves)
				return CreateEmptyResult(vertices.Length);
			Init(vertices, tree);
			return CreateResult();
		}

		private Result CreateEmptyResult(int verticesAmount)
		{
			return new Result(new float[verticesAmount], new float[verticesAmount]);
		}

		private void Init(Vector3[] vertices, KDTree<Vector3> tree)
		{
			_weights = new float[vertices.Length];
			_vertices = vertices;
			_verticesTree = tree;
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
			for (int i = 0; i < _primitives.Length; i++)
			{
				ShapingPrimitive primitive = _primitives[i];
				int[] verticesInRange = _verticesTree.GetNearestToWithin(primitive.Position, primitive.MaxAreaOfEffect);
				
				for (int j = 0; j < verticesInRange.Length; j++)
					CalculateWeight(verticesInRange[j], primitive);
			}
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