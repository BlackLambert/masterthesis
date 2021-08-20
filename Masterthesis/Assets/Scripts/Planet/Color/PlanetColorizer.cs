using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public class PlanetColorizer
    {
        private Planet _planet;
        private Biome[] _biomes;
		private float _blendDistance;

		private BasicColorEvaluator _oceanColorEvaluator;
		private ContinentalColorEvaluator _contientalColorEvaluator;

        public PlanetColorizer(
            Planet planet,
            ContinentalPlatesParameter continentalPlatesParameter,
            Biome[] biomes
            )
		{
            _planet = planet;
            _biomes = biomes;
			_blendDistance = CalculateBlendDistance(continentalPlatesParameter);

			_oceanColorEvaluator = new BasicColorEvaluator(_blendDistance);
			_contientalColorEvaluator = new ContinentalColorEvaluator(_blendDistance);
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
			Vector3[] vertexNormals = face.MeshFilter.sharedMesh.normals;
			float evaluationPointslength = faceData.EvaluationPoints.Length;

			for (int i = 0; i < evaluationPointslength; i++)
				vertexColors[i] = GetVertexColor(face, vertexNormals, i);

			face.MeshFilter.sharedMesh.colors = vertexColors;
		}

		private Color GetVertexColor(PlanetFace face, Vector3[] vertexNormals, int vertexIndex)
		{
			ContinentalPlates plates = _planet.Data.ContinentalPlates;
			Vector3 vertex = face.Data.EvaluationPoints[vertexIndex].WarpedPoint;
			Vector3 vertexNormal = vertexNormals[vertexIndex];
			_contientalColorEvaluator.InitVertexData(vertex, vertexNormal);

			int segmentIndex = face.Data.EvaluationPoints[vertexIndex].ContinentalPlateSegmentIndex;
			ContinentalPlateSegment segment = plates.Segments[segmentIndex];
			Biome biome = _biomes[segment.BiomeID];
			Vector3 pointOnBorder = plates.SegmentsVoronoi.GetNearestBorderPointOf(vertex, segmentIndex);
			float distanceToInnerBorder = _planet.GetDistanceOnSurface(vertex, pointOnBorder);

			List<PlanetColorEvaluator.Result> colors = new List<PlanetColorEvaluator.Result>();
			AddColors(biome, -distanceToInnerBorder, colors);

			if (distanceToInnerBorder < _blendDistance)
				AddBlendColors(plates, vertex, segment, biome, colors);

			return ComputeColor(colors);
		}

		private void AddBlendColors(ContinentalPlates plates, Vector3 vertex, ContinentalPlateSegment segment, Biome biome, List<PlanetColorEvaluator.Result> colors)
		{
			for (int i = 0; i < segment.Neighbors.Length; i++)
			{
				int neighborSegmentIndex = segment.Neighbors[i];
				ContinentalPlateSegment neighborSegment = plates.Segments[neighborSegmentIndex];
				Biome neighborBiome = _biomes[neighborSegment.BiomeID];
				if (neighborBiome.RegionType != biome.RegionType)
					continue;
				Vector3 pointNeighborOnBorder = plates.SegmentsVoronoi.GetNearestBorderPointOf(vertex, neighborSegmentIndex);
				float distanceToSegment = _planet.GetDistanceOnSurface(vertex, pointNeighborOnBorder);
				AddColors(neighborBiome, distanceToSegment, colors);
			}
		}

		private Color ComputeColor(List<PlanetColorEvaluator.Result> colors)
		{
			float weightSum = 0;
			Color color = Color.black;
			foreach (PlanetColorEvaluator.Result c in colors)
			{
				weightSum += c.Weight;
				color += c.Weight * c.Color;
			}

			color /= weightSum;
			return color;
		}

		private void AddColors(Biome biome, float distanceToBorder, List<PlanetColorEvaluator.Result> colors)
		{
			if(biome is ContinentalBiome)
				EvaluateContinentalColor(biome as ContinentalBiome, distanceToBorder, colors);
			else
				EvaluateOceanColor(biome, distanceToBorder, colors);
		}

		private void EvaluateOceanColor(Biome biome, float distanceToInnerBorder, List<PlanetColorEvaluator.Result> colors)
		{
			_oceanColorEvaluator.Init(biome, distanceToInnerBorder);
			colors.Add(_oceanColorEvaluator.Evaluate());
		}

		private void EvaluateContinentalColor(ContinentalBiome biome, float distanceToBorder, List<PlanetColorEvaluator.Result> colors)
		{
			_contientalColorEvaluator.Init(biome, distanceToBorder);
			colors.Add(_contientalColorEvaluator.Evaluate());
		}

		private float CalculateBlendDistance(ContinentalPlatesParameter continentalPlatesParameter)
		{
			return continentalPlatesParameter.BlendFactor * _planet.AtmosphereRadius;
		}
	}
}