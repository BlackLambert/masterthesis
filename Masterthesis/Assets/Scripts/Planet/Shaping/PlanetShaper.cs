using System;
using System.Linq;

namespace SBaier.Master
{
	public class PlanetShaper
	{
		private ShapingLayer[] _shapingLayers;

		public PlanetShaper(ShapingLayer[] shapingLayers, float oceanLevel)
		{
			_shapingLayers = shapingLayers;
		}

		public float[] Shape(EvaluationPointData[] pointsData)
		{
			ShapingLayer.Result[] evalResults = EvaluatePoints(pointsData);
			return CalculateResult(evalResults);
		}

		private ShapingLayer.Result[] EvaluatePoints(EvaluationPointData[] pointsData)
		{
			ShapingLayer.Result[] evalResult = new ShapingLayer.Result[_shapingLayers.Length];
			for (int i = 0; i < evalResult.Length; i++)
				evalResult[i] = _shapingLayers[i].Evaluate(pointsData.Select(d => d.WarpedPoint).ToArray());
			return evalResult;
		}

		private float[] CalculateResult(ShapingLayer.Result[] evalResults)
		{
			float[] result = new float[evalResults[0].EvaluatedValues.Length];
			for (int i = 0; i < _shapingLayers.Length; i++)
			{
				ShapingLayer layer = _shapingLayers[i];
				ShapingLayer.Result evalResult = evalResults[i];
				result = CombineLayerResult(result, layer, evalResult);
			}
			return result;
		}

		private float[] CombineLayerResult(float[] result, ShapingLayer layer, ShapingLayer.Result evalResult)
		{
			switch(layer.ShapingMode)
			{
				case ShapingLayer.Mode.Add:
					return AddLayerResult(result, evalResult);
				case ShapingLayer.Mode.Blend:
					return BlendLayerResult(result, evalResult);
				case ShapingLayer.Mode.Substract:
					return SubstractLayerResult(result, evalResult);
			}
			return result;
		}

		private float[] SubstractLayerResult(float[] result, ShapingLayer.Result evalResult)
		{
			for (int i = 0; i < evalResult.EvaluatedValues.Length; i++)
				result[i] -= evalResult.EvaluatedValues[i] * evalResult.Weight[i];
			return result;
		}

		private float[] BlendLayerResult(float[] result, ShapingLayer.Result evalResult)
		{
			for (int i = 0; i < evalResult.EvaluatedValues.Length; i++)
				result[i] = (result[i] + evalResult.EvaluatedValues[i] * evalResult.Weight[i]) / (1 + evalResult.Weight[i]);
			return result;
		}

		private float[] AddLayerResult(float[] result, ShapingLayer.Result evalResult)
		{
			for (int i = 0; i < evalResult.EvaluatedValues.Length; i++)
				result[i] += evalResult.EvaluatedValues[i] * evalResult.Weight[i];
			return result;
		}
	}
}