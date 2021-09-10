using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public class EvaluationPointDatasInitializer
    {
		Vector3BinaryKDTreeFactory _treeFactory;

		private Planet _planet;
		private float _warpFactor;
		private float _warpChaosFacor;
		private Noise3D _warpNoise;
		private ContinentalPlates _plates;
		private VoronoiDiagram _segementsVoronoi;
		private KDTree<Vector3> _segmentsKDTree;
		private Biome[] _biomes;
		private float _blendDistance;
		private BiomeOccurrenceSerializer _serializer;

		public EvaluationPointDatasInitializer(Vector3BinaryKDTreeFactory treeFactory,
			BiomeOccurrenceSerializer serializer)
		{
			_treeFactory = treeFactory;
			_serializer = serializer;
		}

		public void Compute(Parameter parameter)
		{
			Init(parameter);
			foreach (PlanetFace face in _planet.Faces)
				Compute(face);
		}

		private void Init(Parameter parameter)
		{
			_planet = parameter.Planet;
			_warpNoise = parameter.WarpNoise;
			_warpFactor = parameter.WarpFactor;
			_warpChaosFacor = parameter.WarpChaosFactor;
			_plates = _planet.Data.ContinentalPlates;
			_segementsVoronoi = _plates.SegmentsVoronoi;
			_segmentsKDTree = _treeFactory.Create(_plates.SegmentSites);
			_biomes = parameter.Biomes;
			_blendDistance = parameter.BlendDistance;
		}

		private void Compute(PlanetFace face)
		{
			PlanetFaceData faceData = face.Data;
			EvaluationPointData[] evaluationPoints = faceData.EvaluationPoints;
			PlanetPointsWarper warper = new PlanetPointsWarper(_warpNoise);
			Vector3[] warpedVertices = warper.Warp(face.Vertices, _warpFactor, _warpChaosFacor, _planet.Data.Dimensions.AtmosphereRadius);
			KDTree<Vector3> warpedVerticesTree = _treeFactory.Create(warpedVertices);
			face.SetWarpedVertices(warpedVertices, warpedVerticesTree);
			int[] segmentIndices = _segmentsKDTree.GetNearestTo(warpedVertices);
			for (int i = 0; i < segmentIndices.Length; i++)
				InitData(evaluationPoints[i], segmentIndices[i], warpedVertices[i]);
		}

		private void InitData(EvaluationPointData pointData, int segmentIndex, Vector3 vertex)
		{
			ContinentalPlateSegment segment = _plates.Segments[segmentIndex];
			pointData.ContinentalPlateSegmentIndex = segmentIndex;
			pointData.Biomes = CalculateBiomeOccurrences(segment, vertex);
		}

		private short[] CalculateBiomeOccurrences(ContinentalPlateSegment segment, Vector3 vertex)
		{
			List<short> result = new List<short>(1);
			result.Add(_serializer.Serialize(new BiomeOccurrence((byte) segment.BiomeID, 1f)));
			AddNeighborBiomes(segment, result, vertex);
			return result.ToArray();
		}

		private void AddNeighborBiomes(ContinentalPlateSegment segment, List<short> result, Vector3 vertex)
		{
			int[] neighbors = segment.Neighbors; 
			for (int i = 0; i < neighbors.Length; i++)
				AddNeighborBiome(neighbors[i], result, vertex);
		}

		private void AddNeighborBiome(int neighborIndex, List<short> result, Vector3 vertex)
		{
			ContinentalPlateSegment neighborSegment = _plates.Segments[neighborIndex];
			float distanceToSegment = GetDistance(neighborIndex, vertex);
			if (distanceToSegment > _blendDistance)
				return;
			float weight = 1 - (distanceToSegment / _blendDistance);
			short value = _serializer.Serialize(new BiomeOccurrence((byte)neighborSegment.BiomeID, weight));
			result.Add(value);
		}

		private float GetDistance(int segmentIndex, Vector3 vertex)
		{
			Vector3 pointNeighborOnBorder = _segementsVoronoi.GetNearestBorderPointOf(vertex, segmentIndex);
			float distanceToSegment = _planet.GetDistanceOnSurface(vertex, pointNeighborOnBorder);
			return distanceToSegment;
		}

		public class Parameter
		{
			public Parameter(Planet planet,
				Noise3D warpNoise,
				float warpFactor,
				float warpChaosFactor,
				Biome[] biomes,
				float blendDistance)
			{
				Planet = planet;
				WarpNoise = warpNoise;
				WarpFactor = warpFactor;
				WarpChaosFactor = warpChaosFactor;
				Biomes = biomes;
				BlendDistance = blendDistance;
			}

			public Planet Planet { get; }
			public Noise3D WarpNoise { get; }
			public float WarpFactor { get; }
			public float WarpChaosFactor { get; }
			public Biome[] Biomes { get; }
			public float BlendDistance { get; }
		}
	}
}