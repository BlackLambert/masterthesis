using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace SBaier.Master
{
    public class PerlinOctaveNoiseInstaller : MonoInstaller
    {
		[Inject]
		private Seed _seed;

		public override void InstallBindings()
		{
			Container.Bind<PerlinNoise>().FromMethod(CreatePerlinNoise).AsTransient();
			Container.Bind<ICollection<LayeredNoise.NoiseLayer>>().FromMethod(CreateValidOctaves).AsSingle();
			Container.Bind(typeof(Noise3D), typeof(Noise2D)).To<LayeredNoise>().AsSingle().NonLazy();
		}

		private ICollection<LayeredNoise.NoiseLayer> CreateValidOctaves()
		{
			ICollection<LayeredNoise.NoiseLayer> result = new List<LayeredNoise.NoiseLayer>()
			{
				new LayeredNoise.NoiseLayer(Container.Resolve<PerlinNoise>(), 1, 1),
				new LayeredNoise.NoiseLayer(Container.Resolve<PerlinNoise>(), 0.5f, 0.5f),
				new LayeredNoise.NoiseLayer(Container.Resolve<PerlinNoise>(), 0.25f, 0.25f),
				new LayeredNoise.NoiseLayer(Container.Resolve<PerlinNoise>(), 0.1f, 0.1f)
			};
			return result;
		}

		private PerlinNoise CreatePerlinNoise()
		{
			return new PerlinNoise(new Seed(_seed.Random.Next(0, 10000)));
		}
	}
}