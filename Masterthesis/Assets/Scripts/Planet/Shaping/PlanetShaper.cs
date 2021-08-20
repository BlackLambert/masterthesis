using System;
using System.Linq;
using UnityEngine;

namespace SBaier.Master
{
	public class PlanetShaper
	{
		private ShapingLayer[] _shapingLayers;
		private ShapingLayer.Result[] _evaluationResults;
		private int _pointsAmount;
		private float[] _result;

		private int ShapingLayersAmount => _shapingLayers.Length;

		public PlanetShaper(ShapingLayer[] shapingLayers, float oceanLevel)
		{
			_shapingLayers = shapingLayers;
		}

		public float[] Shape(EvaluationPointData[] pointsData)
		{
			_pointsAmount = pointsData.Length;
			EvaluatePoints(pointsData);
			CalculateResult();
			return _result;
		}

		private void EvaluatePoints(EvaluationPointData[] pointsData)
		{
			_evaluationResults = new ShapingLayer.Result[ShapingLayersAmount];
			for (int i = 0; i < ShapingLayersAmount; i++)
				_evaluationResults[i] = EvaluatePoints(pointsData, _shapingLayers[i]);
		}

		private ShapingLayer.Result EvaluatePoints(EvaluationPointData[] pointsData, ShapingLayer layer)
		{
			Vector3[] warpedVertices = pointsData.Select(d => d.WarpedPoint).ToArray();
			return layer.Evaluate(warpedVertices);
		}

		private void CalculateResult()
		{
			_result = new float[_pointsAmount];
			for (int i = 0; i < ShapingLayersAmount; i++)
				CombineLayerResult(i);
		}

		private void CombineLayerResult(int layerIndex)
		{
			ShapingLayer layer = _shapingLayers[layerIndex];
			ShapingLayer.Result evaluationResult = _evaluationResults[layerIndex];
			switch (layer.ShapingMode)
			{
				case ShapingLayer.Mode.Add:
					AddLayerResult(evaluationResult);
					break;
				case ShapingLayer.Mode.Blend:
					BlendLayerResult(evaluationResult);
					break;
				case ShapingLayer.Mode.Substract:
					SubstractLayerResult(evaluationResult);
					break;
				default:
					throw new NotImplementedException();
			}
		}

		private void AddLayerResult(ShapingLayer.Result evaluationResult)
		{
			for (int i = 0; i < _pointsAmount; i++)
				AddLayerResult(evaluationResult, i);
		}

		private void BlendLayerResult(ShapingLayer.Result evaluationResult)
		{
			for (int i = 0; i < _pointsAmount; i++)
				BlendLayerResult(evaluationResult, i);
		}

		private void SubstractLayerResult(ShapingLayer.Result evaluationResult)
		{
			for (int i = 0; i < _pointsAmount; i++)
				SubstractLayerResult(evaluationResult, i);
		}

		private void SubstractLayerResult(ShapingLayer.Result evaluationResult, int pointIndex)
		{
			float substractionWeight = evaluationResult.Weight[pointIndex];
			float substractionValue = evaluationResult.EvaluatedValues[pointIndex];
			float substraction = substractionWeight * substractionValue;
			_result[pointIndex] -= substraction;
		}

		private void BlendLayerResult(ShapingLayer.Result evaluationResult, int pointIndex)
		{
			float additionWeight = evaluationResult.Weight[pointIndex];
			float additionValue = evaluationResult.EvaluatedValues[pointIndex];
			float weightAddition = additionWeight * additionValue;
			float formerValue = _result[pointIndex];
			float formerValueWeight = 1;
			float valueSum = formerValue + weightAddition;
			float weightSum = formerValueWeight + additionWeight;
			_result[pointIndex] = valueSum / weightSum;
		}

		private void AddLayerResult(ShapingLayer.Result evaluationResult, int pointIndex)
		{
			float additionWeight = evaluationResult.Weight[pointIndex];
			float additionValue = evaluationResult.EvaluatedValues[pointIndex];
			float addition = additionWeight * additionValue;
			_result[pointIndex] += addition;
		}
	}
}