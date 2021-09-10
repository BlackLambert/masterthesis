using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

namespace SBaier.Master
{
    public class PlanetColorizer
	{
		private PlanetLayerMaterialSettings[] _materials;
		private readonly PlanetLayerMaterialSerializer _serializer;
		private const float _maxShineThrough = 0.015f;
		private const float _liquidDepthMax = 0.5f;
		private Noise3D _gradientNoise;
		private Planet _planet;
		private PlanetData _planetData;
		private float _relativeSeaLevel;

		private PlanetLayerMaterialSettings[] _idToMaterial = new PlanetLayerMaterialSettings[byte.MaxValue];

		public PlanetColorizer(
			PlanetLayerMaterialSerializer serializer,
			PlanetLayerMaterialSettings[] materials
            )
		{
			_serializer = serializer;
			_materials = materials;
			CreateIDToMaterial();
		}

		private void CreateIDToMaterial()
		{
			foreach (PlanetLayerMaterialSettings material in _materials)
				_idToMaterial[material.ID] = material;
		}

		public void Compute(Parameter parameter)
		{
			Init(parameter);
            for (int i = 0; i < _planet.Faces.Length; i++)
				UpdateFaceVertexColors(i);
		}

		private void Init(Parameter parameter)
		{
			_planet = parameter.Planet;
			_planetData = _planet.Data;
			_gradientNoise = _planet.Data.GradientNoise;
			_relativeSeaLevel = _planet.Data.Dimensions.RelativeSeaLevel;
		}

		private void UpdateFaceVertexColors(int faceIndex)
		{
			PlanetFace face = _planet.Faces[faceIndex];
			PlanetFaceData faceData = face.Data;
			Color[] vertexColors = new Color[face.VertexCount];
			Vector3[] vertices = face.WarpedVertices;
			float[] gradientNoiseEvaluation = EvaluateGradientNoise(vertices);
			EvaluationPointData[] evalPoints = faceData.EvaluationPoints;

			for (int i = 0; i < evalPoints.Length; i++)
				vertexColors[i] = GetVertexColor(gradientNoiseEvaluation[i], evalPoints[i]);

			face.MeshFilter.sharedMesh.colors = vertexColors;
		}

		private Color GetVertexColor(float gradientValue, EvaluationPointData data)
		{
			List<PlanetMaterialLayerData> layers = data.Layers;
			int count = layers.Count;
			for (int i = count - 1; i >= 0; i--)
			{
				PlanetMaterialLayerData layerData = layers[i];
				if (i == 0)
					return GetBaseColor(layerData, gradientValue);
				if (layerData.State == PlanetMaterialState.Gas)
					continue;
				if (!_planetData.IsLayerActive(layerData.MaterialType))
					continue;
				return GetNextLayerShineThroughColor(GetBaseColor(layerData, gradientValue), layerData, layers[i - 1], gradientValue);
			}
			throw new InvalidOperationException();
		}

		private Color GetNextLayerShineThroughColor(Color color, PlanetMaterialLayerData layer, PlanetMaterialLayerData nextLayer, float gradientValue)
		{
			if (layer.Height >= _maxShineThrough)
				return color;
			Color shineThroughColor = GetBaseColor(nextLayer, gradientValue);
			float t = layer.Height / _maxShineThrough;
			return Color.Lerp(shineThroughColor, color, t);
		}

		private Color GetBaseColor(PlanetMaterialLayerData layerData, float gradientValue)
		{
			int count = layerData.Materials.Count;
			Color result = new Color(0,0,0,0);
			float weightSum = 0;
			for (int i = 0; i < count; i++)
				GetVertexColor(layerData, gradientValue, ref result, ref weightSum, i);
			return result / weightSum;
		}

		private void GetVertexColor(PlanetMaterialLayerData layerData, float gradientValue, ref Color result, ref float weightSum, int materialIndex)
		{
			PlanetLayerMaterial deserializedData = _serializer.Deserialize(layerData.Materials[materialIndex]);
			PlanetLayerMaterialSettings material = _idToMaterial[deserializedData.MaterialID];
			float weight = deserializedData.Portion;
			Color col = GetVertexColor(layerData, material, gradientValue) * weight;
			result = result + col;
			weightSum += weight;
		}

		private Color GetVertexColor(PlanetMaterialLayerData layerData, PlanetLayerMaterialSettings material, float gradientValue)
		{
			switch(material.State)
			{
				case PlanetMaterialState.Liquid:
					return CalculateLiquidColor(material as LiquidPlanetLayerMaterialSettings, layerData.Height, gradientValue);
				case PlanetMaterialState.Solid:
					return CalculateSolidColor(material as SolidPlanetLayerMaterialSettings, gradientValue);
				default:
					throw new NotImplementedException();
			}
		}

		private Color CalculateSolidColor(SolidPlanetLayerMaterialSettings settings, float gradientNoiseValue)
		{
			Color baseColor = settings.BaseGradient.Evaluate(gradientNoiseValue);
			return baseColor;
		}

		private Color CalculateLiquidColor(LiquidPlanetLayerMaterialSettings settings, float height, float gradientNoiseValue)
		{
			float depthEvalValue = Mathf.Clamp01( height / _relativeSeaLevel + _liquidDepthMax);
			Color depthColor = settings.DepthColor;
			Color baseColor = settings.BaseGradient.Evaluate(gradientNoiseValue);
			return Color.Lerp(baseColor, depthColor, depthEvalValue);
		}

		private float[] EvaluateGradientNoise(Vector3[] vertices)
		{
			NativeArray<Vector3> verticesNative = new NativeArray<Vector3>(vertices, Allocator.TempJob);
			NativeArray<float> resultNative = _gradientNoise.Evaluate3D(verticesNative);
			float[] gradientNoiseEvaluation = resultNative.ToArray();
			verticesNative.Dispose();
			resultNative.Dispose();
			return gradientNoiseEvaluation;
		}

		public class Parameter
		{
			public Parameter(
				Planet planet
			)
			{
				Planet = planet;
			}

			public Planet Planet { get; }
		}
	}
}