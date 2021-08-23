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

		private Noise3D _gradientNoise;
		private Planet _planet;

		private Dictionary<byte, PlanetLayerMaterialSettings> _idToMaterial = new Dictionary<byte, PlanetLayerMaterialSettings>();

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
				_idToMaterial.Add(material.ID, material);
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
			_gradientNoise = parameter.GradientNoise;
		}

		private void UpdateFaceVertexColors(int faceIndex)
		{
			PlanetFace face = _planet.Faces[faceIndex];
			PlanetFaceData faceData = face.Data;
			Color[] vertexColors = new Color[face.MeshFilter.sharedMesh.vertexCount];
			Vector3[] vertices = face.Data.EvaluationPoints.Select(p => p.WarpedPoint).ToArray();
			float[] gradientNoiseEvaluation = EvaluateGradientNoise(vertices);
			EvaluationPointData[] evalPoints = faceData.EvaluationPoints;

			for (int i = 0; i < evalPoints.Length; i++)
				vertexColors[i] = GetVertexColor(gradientNoiseEvaluation[i], evalPoints[i]);

			face.MeshFilter.sharedMesh.colors = vertexColors;
		}

		private Color GetVertexColor(float gradientValue, EvaluationPointData data)
		{
			for(int i = data.Layers.Count - 1; i >= 0; i--)
			{
				PlanetMaterialLayerData layerData = data.Layers[i];
				if (layerData.State == PlanetMaterialState.Gas)
					continue;
				return GetVertexColor(layerData, gradientValue);
			}
			throw new InvalidOperationException();
		}

		private Color GetVertexColor(PlanetMaterialLayerData layerData, float gradientValue)
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
			result += col;
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
			float depthEvalValue = height / _planet.Data.Dimensions.RelativeSeaLevel;
			Color depthColor = settings.DepthGradient.Evaluate(depthEvalValue);
			Color baseColor = settings.BaseGradient.Evaluate(gradientNoiseValue);
			//return depthColor;
			return (depthColor + baseColor) / 2;
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
				Planet planet,
				Noise3D gradientNoise
			)
			{
				Planet = planet;
				GradientNoise = gradientNoise;
			}

			public Planet Planet { get; }
			public Noise3D GradientNoise { get; }
		}
	}
}