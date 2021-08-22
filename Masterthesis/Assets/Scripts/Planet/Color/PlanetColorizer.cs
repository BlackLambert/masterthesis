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
        private Planet _planet;
		private PlanetLayerMaterialSettings[] _materials;
		private Noise3D _gradientNoise;

		private Dictionary<int, PlanetLayerMaterialSettings> _idToMaterial = new Dictionary<int, PlanetLayerMaterialSettings>();

		public PlanetColorizer(
			Planet planet,
			PlanetLayerMaterialSettings[] materials,
			Noise3D gradientNoise
            )
		{
            _planet = planet;
			_materials = materials;
			_gradientNoise = gradientNoise;
			CreateIDToMaterial();
		}

		private void CreateIDToMaterial()
		{
			foreach (PlanetLayerMaterialSettings material in _materials)
				_idToMaterial.Add(material.ID, material);
		}

		public void Compute()
		{
            for (int i = 0; i < _planet.Faces.Length; i++)
				UpdateFaceVertexColors(i);
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
				PlanetLayerData layerData = data.Layers[i];
				PlanetLayerMaterialSettings material = _idToMaterial[layerData.MaterialIndex];
				if (material.State == PlanetLayerMaterialState.Gas)
					continue;
				return GetVertexColor(layerData, material, gradientValue);
			}
			throw new InvalidOperationException();
		}

		private Color GetVertexColor(PlanetLayerData layerData, PlanetLayerMaterialSettings material, float gradientValue)
		{
			switch(material.State)
			{
				case PlanetLayerMaterialState.Liquid:
					return CalculateLiquidColor(material as LiquidPlanetLayerMaterialSettings, layerData.Height, gradientValue);
				case PlanetLayerMaterialState.Solid:
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
	}
}