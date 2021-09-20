

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
	public class EvaluationPointDatasInitializer
	{
		private const int _maxWarpChaos = 3;
		private const float _maxRelativeWarpDistance = 0.3f;
		private const float _minRelativeWarpDistance = 0.1f;
		Vector3BinaryKDTreeFactory _treeFactory;

		private Planet _planet;
		private float _warpFactor;
		private float _warpChaosFacor;
		private Noise3D _warpNoise;
		private ContinentalPlates _plates;
		private ContinentalPlateSegment[] _segments;
		private KDTree<Vector3> _segmentsKDTree;
		private float _blendDistance;
		private bool _hasBlendDistance;
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
			_warpFactor = Mathf.Max(_minRelativeWarpDistance, parameter.WarpFactor * _maxRelativeWarpDistance);
			_warpFactor = parameter.WarpFactor * _maxRelativeWarpDistance;
			_warpChaosFacor = parameter.WarpFactor * _maxWarpChaos;
			_plates = _planet.Data.ContinentalPlates;
			_segments = _plates.Segments;
			_segmentsKDTree = _treeFactory.Create(_plates.SegmentSites);
			_blendDistance = parameter.BlendDistance;
			_hasBlendDistance = parameter.BlendDistance > 0;
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
			ContinentalPlateSegment segment = _segments[segmentIndex];
			pointData.ContinentalPlateSegmentIndex = segmentIndex;
			pointData.Biomes = CalculateBiomeOccurrences(segment, vertex);
		}

		private short[] CalculateBiomeOccurrences(ContinentalPlateSegment segment, Vector3 vertex)
		{
			List<short> result = new List<short>(1);
			result.Add(_serializer.Serialize(new BiomeOccurrence((byte)segment.BiomeID, 1f)));
			if (_hasBlendDistance)
				AddNeighborBiomes(segment, result, vertex);
			return result.ToArray();
		}

		private void AddNeighborBiomes(ContinentalPlateSegment segment, List<short> result, Vector3 vertex)
		{
			int[] neighbors = segment.Neighbors;
			Vector3 distanceVectorToSite = vertex.FastSubstract(segment.Site);
			VoronoiRegion segementRegion = segment.VoronoiRegion;
			float[] distanceToNeighbors = segementRegion.DistanceToNeighbors;
			Vector3[] distanceVectorToNeighbors = segementRegion.DistanceVectorToNeighbors;
			for (int i = 0; i < neighbors.Length; i++)
			{
				int neighborSegmentIndex = neighbors[i];
				ContinentalPlateSegment neighborSegment = _segments[neighborSegmentIndex];
				float distanceToNeighbor = distanceToNeighbors[i];
				Vector3 distanceVectorToNeighbor = distanceVectorToNeighbors[i];
				float projectionLength = Vector3.Dot(distanceVectorToSite, distanceVectorToNeighbor) / distanceToNeighbor;
				float vertexDistanceToBorder = distanceToNeighbor - projectionLength;
				if (vertexDistanceToBorder >= _blendDistance)
					continue;
				float weight = Mathf.Clamp01(1 - vertexDistanceToBorder / _blendDistance);
				short value = _serializer.Serialize(new BiomeOccurrence((byte)neighborSegment.BiomeID, weight));
				result.Add(value);
			}
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