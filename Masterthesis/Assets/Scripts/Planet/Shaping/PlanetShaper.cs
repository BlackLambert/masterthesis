using System.Linq;

namespace SBaier.Master
{
    public class PlanetShaper
    {
        private ShapingLayer[] _shapingLayers;
		private readonly float _oceanLevel;

		public PlanetShaper(ShapingLayer[] shapingLayers, float oceanLevel)
		{
            _shapingLayers = shapingLayers;
			_oceanLevel = oceanLevel;
		}

        public float[] Shape(EvaluationPointData[] pointsData)
		{
			float[] result = new float[pointsData.Length];
			int[] appliedBlendValues = new int[pointsData.Length];
			Init(result, appliedBlendValues);
			SumShapingValues(pointsData, result, appliedBlendValues);
			CalculateAverage(result, appliedBlendValues);
			return result;
		}

		private void Init(float[] result, int[] blendValues)
		{
			for (int i = 0; i < result.Length; i++)
				Init(result, blendValues, i);
		}

		private void Init(float[] result, int[] blendValues, int index)
		{
			result[index] = 0;
			blendValues[index] = 0;
		}

		private void SumShapingValues(EvaluationPointData[] pointsData, float[] result, int[] blendValues)
		{
			for (int i = 0; i < _shapingLayers.Length; i++)
				SumLayerShapingValues(pointsData, result, blendValues, _shapingLayers[i]);
		}

		private void SumLayerShapingValues(EvaluationPointData[] pointsData, float[] result, int[] blendValues, ShapingLayer layer)
		{
			float[] evalValues = layer.Evaluate(pointsData.Select(d => d.WarpedPoint).ToArray());
			for (int j = 0; j < evalValues.Length; j++)
				SumShapingValues(result, blendValues, evalValues[j], j);
		}

		private void SumShapingValues(float[] result, int[] blendValues, float evalValue, int index)
		{
			if (evalValue == 0)
				return;
			result[index] += evalValue;
			blendValues[index]++;
		}

		private void CalculateAverage(float[] result, int[] blendValues)
		{
			for (int i = 0; i < result.Length; i++)
				CalcualteAverage(result, blendValues, i);
		}

		private void CalcualteAverage(float[] result, int[] blendValues, int index)
		{
			float value = result[index];
			int blends = blendValues[index];
			if (value == 0)
				result[index] = _oceanLevel;
			if (blends == 0)
				return;
			result[index] /= blends;
		}
	}
}