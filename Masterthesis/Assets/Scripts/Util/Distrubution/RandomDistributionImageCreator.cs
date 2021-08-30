using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace SBaier.Master
{
	public class RandomDistributionImageCreator : ImageCreator
	{
		private Seed _seed;

		[SerializeField]
		private int _points = 100;

		private float[] _values;
		private int[] _indices;

		[Inject]
		public void Construct(Seed seed)
		{
			_seed = seed;
			_values = new float[ImageSize.x * ImageSize.y];
			_indices = new int[ImageSize.x * ImageSize.y];
		}

		protected override void Start()
		{
			CreateIndices();
			CreateValues();
			base.Start();
		}

		private void CreateIndices()
		{
			for (int i = 0; i < _indices.Length; i++)
				_indices[i] = i;
			_indices = _indices.OrderBy(x => _seed.Random.Next()).ToArray();
		}

		private void CreateValues()
		{
			for (int i = 0; i < _values.Length; i++)
				_values[i] = _indices[i] > _points ? 1 : 0;
		}

		protected override float GetValue(int y, int x)
		{
			return _values[y * ImageSize.x + x];
		}
	}
}