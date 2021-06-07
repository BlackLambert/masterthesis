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
			Container.Bind<ICollection<OctaveNoise.Octave>>().FromMethod(CreateValidOctaves).AsSingle();
			Container.Bind(typeof(Noise3D), typeof(Noise2D)).To<OctaveNoise>().AsSingle().NonLazy();
		}

		private ICollection<OctaveNoise.Octave> CreateValidOctaves()
		{
			ICollection<OctaveNoise.Octave> result = new List<OctaveNoise.Octave>()
			{
				new OctaveNoise.Octave(Container.Resolve<PerlinNoise>(), 1, 1),
				new OctaveNoise.Octave(Container.Resolve<PerlinNoise>(), 0.5f, 0.5f),
				new OctaveNoise.Octave(Container.Resolve<PerlinNoise>(), 0.25f, 0.25f),
				new OctaveNoise.Octave(Container.Resolve<PerlinNoise>(), 0.1f, 0.1f)
			};
			return result;
		}

		private PerlinNoise CreatePerlinNoise()
		{
			return new PerlinNoise(new Seed(_seed.Random.Next(0, 10000)));
		}
	}
}