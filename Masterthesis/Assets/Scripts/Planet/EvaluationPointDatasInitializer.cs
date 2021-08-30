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
		private Noise3D _warpNoise;
		private ContinentalPlates _plates;
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

		private void Compute(PlanetFace face)
		{
			PlanetFaceData faceData = face.Data;
			PlanetPointsWarper warper = new PlanetPointsWarper(_warpNoise);
			Vector3[] warpedVertices = warper.Warp(face.Vertices, _warpFactor, _planet.Data.Dimensions.AtmosphereRadius);
			int[] segmentIndices = _segmentsKDTree.GetNearestTo(warpedVertices);
			for (int i = 0; i < segmentIndices.Length; i++)
				InitData(faceData.EvaluationPoints[i], segmentIndices[i], warpedVertices[i]);
		}

		private void InitData(EvaluationPointData pointData, int segmentIndex, Vector3 warpedVertex)
		{
			ContinentalPlateSegment segment = _plates.Segments[segmentIndex];
			pointData.ContinentalPlateSegmentIndex = segmentIndex;
			pointData.WarpedPoint = warpedVertex;
			pointData.Biomes = GetBiomeOccurrences(pointData, segment);
		}

		private short[] GetBiomeOccurrences(EvaluationPointData pointData, ContinentalPlateSegment segment)
		{
			List<short> result = new List<short>(1);
			result.Add(_serializer.Serialize(new BiomeOccurrence((byte) segment.BiomeID, 1f)));
			AddNeighborBiomes(pointData, segment, result);
			return result.ToArray();
		}

		private void AddNeighborBiomes(EvaluationPointData pointData, ContinentalPlateSegment segment, List<short> result)
		{
			int[] neighbors = segment.Neighbors; 
			for (int i = 0; i < neighbors.Length; i++)
				AddNeighborBiome(pointData, neighbors[i], result);
		}

		private void AddNeighborBiome(EvaluationPointData data, int neighborIndex, List<short> result)
		{
			ContinentalPlateSegment neighborSegment = _plates.Segments[neighborIndex];
			float distanceToSegment = GetDistance(data, neighborIndex);
			if (distanceToSegment > _blendDistance)
				return;
			float weight = 1 - (distanceToSegment / _blendDistance);
			short value = _serializer.Serialize(new BiomeOccurrence((byte)neighborSegment.BiomeID, weight));
			result.Add(value);
		}

		private float GetDistance(EvaluationPointData data, int segmentIndex)
		{
			Vector3 vertex = data.WarpedPoint;
			Vector3 pointNeighborOnBorder = _plates.SegmentsVoronoi.GetNearestBorderPointOf(vertex, segmentIndex);
			float distanceToSegment = _planet.GetDistanceOnSurface(vertex, pointNeighborOnBorder);
			return distanceToSegment;
		}

		private void Init(Parameter parameter)
		{
			_planet = parameter.Planet;
			_warpNoise = parameter.WarpNoise;
			_warpFactor = parameter.WarpFactor;
			_plates = _planet.Data.ContinentalPlates;
			_segmentsKDTree = _treeFactory.Create(_plates.SegmentSites);
			_biomes = parameter.Biomes;
			_blendDistance = parameter.BlendDistance;
		}

		public class Parameter
		{
			public Parameter(Planet planet,
				Noise3D warpNoise,
				float warpFactor,
				Biome[] biomes,
				float blendDistance)
			{
				Planet = planet;
				WarpNoise = warpNoise;
				WarpFactor = warpFactor;
				Biomes = biomes;
				BlendDistance = blendDistance;
			}

			public Planet Planet { get; }
			public Noise3D WarpNoise { get; }
			public float WarpFactor { get; }
			public Biome[] Biomes { get; }
			public float BlendDistance { get; }
		}
	}
}