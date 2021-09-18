using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace SBaier.Master
{
	public class SampleEliminationDistributionImageCreator : ImageCreator
	{
		private Seed _seed;

		[SerializeField]
		private int _targetPoints = 100;
		[SerializeField]
		[Range(1, 5)]
		private float _startPointsfactor = 2;
		private SampleElimination3D _sampleElimination;

		private Vector3[] _startPoints;
		private Vector3[] _samplePoints;
		private float[] _values;

		[Inject]
		public void Construct(Seed seed,
			SampleElimination3D sampleElimination)
		{
			_seed = seed;
			_values = new float[ImageSize.x * ImageSize.y];
			_sampleElimination = sampleElimination;
		}

		protected override void Start()
		{
			CreateStartPoints();
			EliminatePoints();
			CreateValues();
			base.Start();
		}

		private void CreateStartPoints()
		{
			_startPoints = new Vector3[(int)(_targetPoints * _startPointsfactor)];
			for (int i = 0; i < _startPoints.Length; i++)
			{
				float x = _seed.Random.Next(0, ImageSize.x);
				float y = _seed.Random.Next(0, ImageSize.y);
				_startPoints[i] = new Vector3(x, y, 0);
			}
		}

		private void EliminatePoints()
		{
			_samplePoints = _sampleElimination.Eliminate(_startPoints, _targetPoints, ImageSize.x * ImageSize.y);
			_samplePoints = _samplePoints.OrderBy(p => p.x).ToArray();
		}

		private void CreateValues()
		{
			for (int i = 0; i < _values.Length; i++)
				_values[i] = 1;
			for (int i = 0; i < _samplePoints.Length; i++)
			{
				Vector3 point = _samplePoints[i];
				int valueIndex = (int)point.x + (int)point.y * ImageSize.x; 
				_values[valueIndex] = 0;
			}
			//int zeroCount = _values.Count(v => v == 0);
			//if (zeroCount != _targetPoints)
			//	throw new InvalidOperationException();
		}

		protected override float GetValue(int y, int x)
		{
			return _values[y * ImageSize.x + x];
		}
	}
}