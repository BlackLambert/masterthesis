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

		public EvaluationPointDatasInitializer(Vector3BinaryKDTreeFactory treeFactory)
		{
			_treeFactory = treeFactory;
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
			for (int j = 0; j < warpedVertices.Length; j++)
				Compute(faceData.EvaluationPoints[j], warpedVertices[j]);
		}

		private void Compute(EvaluationPointData pointData, Vector3 warpedVertex)
		{
			int segmentIndex = _segmentsKDTree.GetNearestTo(warpedVertex);
			ContinentalPlateSegment segment = _plates.Segments[segmentIndex];
			pointData.ContinentalPlateSegmentIndex = segmentIndex;
			pointData.BiomeID = segment.BiomeID;
			pointData.WarpedPoint = warpedVertex;
		}

		private void Init(Parameter parameter)
		{
			_planet = parameter.Planet;
			_warpNoise = parameter.WarpNoise;
			_warpFactor = parameter.WarpFactor;
			_plates = _planet.Data.ContinentalPlates;
			_segmentsKDTree = _treeFactory.Create(_plates.SegmentSites);
		}

		public class Parameter
		{
			public Parameter(Planet planet,
				Noise3D warpNoise,
				float warpFactor)
			{
				Planet = planet;
				WarpNoise = warpNoise;
				WarpFactor = warpFactor;
			}

			public Planet Planet { get; }
			public Noise3D WarpNoise { get; }
			public float WarpFactor { get; }
		}
	}
}