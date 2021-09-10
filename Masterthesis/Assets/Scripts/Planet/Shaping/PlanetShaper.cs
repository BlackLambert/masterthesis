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

		public PlanetShaper(ShapingLayer[] shapingLayers)
		{
			_shapingLayers = shapingLayers;
		}

		public float[] Shape(PlanetFace face)
		{
			_pointsAmount = face.Data.EvaluationPoints.Length;
			EvaluatePoints(face);
			CalculateResult();
			return _result;
		}

		private void EvaluatePoints(PlanetFace face)
		{
			_evaluationResults = new ShapingLayer.Result[ShapingLayersAmount];
			for (int i = 0; i < ShapingLayersAmount; i++)
				_evaluationResults[i] = _shapingLayers[i].Evaluate(face.WarpedVertices, face.WarpedVertexTree);
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
				case ShapingLayer.Mode.Min:
					MinLayerResult(evaluationResult);
					break;
				case ShapingLayer.Mode.Max:
					MaxLayerResult(evaluationResult);
					break;
				default:
					throw new NotImplementedException();
			}
		}

		private void MaxLayerResult(ShapingLayer.Result evaluationResult)
		{
			for (int i = 0; i < _pointsAmount; i++)
				MaxLayerResult(evaluationResult, i);
		}

		private void MinLayerResult(ShapingLayer.Result evaluationResult)
		{
			for (int i = 0; i < _pointsAmount; i++)
				MinLayerResult(evaluationResult, i);
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
			substractionValue = substractionValue - 0.5f;
			float substraction = substractionWeight * substractionValue;
			_result[pointIndex] -= substraction;
		}

		private void BlendLayerResult(ShapingLayer.Result evaluationResult, int pointIndex)
		{
			float additionWeight = evaluationResult.Weight[pointIndex];
			float additionValue = evaluationResult.EvaluatedValues[pointIndex] - 0.5f;
			float weightAddition = additionWeight * additionValue;
			float formerValue = _result[pointIndex] - 0.5f;
			float formerValueWeight = 1;
			float valueSum = formerValue + weightAddition;
			float weightSum = formerValueWeight + additionWeight;
			_result[pointIndex] = (valueSum / weightSum) + 0.5f;
		}

		private void AddLayerResult(ShapingLayer.Result evaluationResult, int pointIndex)
		{
			float additionWeight = evaluationResult.Weight[pointIndex];
			float additionValue = evaluationResult.EvaluatedValues[pointIndex];
			additionValue = additionValue - 0.5f;
			float addition = additionWeight * additionValue;
			_result[pointIndex] += addition;
		}

		private void MaxLayerResult(ShapingLayer.Result evaluationResult, int pointIndex)
		{
			float weight = evaluationResult.Weight[pointIndex];
			float value = evaluationResult.EvaluatedValues[pointIndex];
			float weightedResult = weight * value;
			_result[pointIndex] = Mathf.Max(weightedResult, _result[pointIndex]);
		}

		private void MinLayerResult(ShapingLayer.Result evaluationResult, int pointIndex)
		{
			float weight = evaluationResult.Weight[pointIndex];
			float value = evaluationResult.EvaluatedValues[pointIndex];
			float weightedResult = weight * value;
			_result[pointIndex] = Mathf.Min(weightedResult, _result[pointIndex]);
		}
	}
}