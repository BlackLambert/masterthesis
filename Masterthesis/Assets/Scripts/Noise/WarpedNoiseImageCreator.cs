using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Zenject;

namespace SBaier.Master
{
    public class WarpedNoiseImageCreator : ImageCreator
    {
		private Seed _seed;
		private NoiseFactory _noiseFactory;

		[SerializeField]
		private NoiseSettings _baseNoiseSettings;
		[SerializeField]
		private NoiseSettings _warpNoiseSettings;
		[SerializeField]
		private int _maxWarpDistance = 50;
		[SerializeField]
		private Vector2 _delta = new Vector2(0.01f, 0.01f); 

		private Noise3D _baseNoise;
		private Noise3D _warpNoiseX;
		private Noise3D _warpNoiseY;
		private float _xWarpDistance;
		private float _yWarpDistance;

		[Inject]
		public void Construct(Seed seed,
			NoiseFactory noiseFactory)
		{
			_seed = seed;
			_noiseFactory = noiseFactory;
		}

		protected override void Start()
		{
			_baseNoise = _noiseFactory.Create(_baseNoiseSettings, _seed);
			_noiseFactory.ClearCache();
			_warpNoiseX = _noiseFactory.Create(_warpNoiseSettings, CreateSeedBasedOn(_seed));
			_noiseFactory.ClearCache();
			_warpNoiseY = _noiseFactory.Create(_warpNoiseSettings, CreateSeedBasedOn(_seed));

			_xWarpDistance = _maxWarpDistance * _delta.x;
			_yWarpDistance = _maxWarpDistance * _delta.y;

			base.Start();
		}

		protected override float GetValue(int y, int x)
		{
			Vector2 point = new Vector2(x * _delta.x, y * _delta.y);
			float warpX = _warpNoiseX.Evaluate2D(point) * _xWarpDistance - _xWarpDistance / 2;
			float warpY = _warpNoiseY.Evaluate2D(point) * _yWarpDistance - _yWarpDistance / 2;
			Vector2 warpVector = new Vector2(warpX, warpY);
			return _baseNoise.Evaluate2D(point + warpVector);
		}

		private Seed CreateSeedBasedOn(Seed seed)
		{
			return new Seed(seed.Random.Next());
		}
	}
}