using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public class MeshGeneratorFactoryImpl : MeshGeneratorFactory
    {
		private Seed _seed;
		private QuickSelector<Vector3> _selector;

		public MeshGeneratorFactoryImpl(Seed seed,
			QuickSelector<Vector3> selector)
		{
			_seed = seed;
			_selector = selector;
		}

		public MeshGenerator Create(MeshGeneratorSettings settings)
		{
			switch (settings.Type)
			{
				case MeshGeneratorType.Icosahedron:
					return new IcosahedronGenerator();
				case MeshGeneratorType.Plain:
					return new PlainGenerator();
				case MeshGeneratorType.Cube:
					return new CubeMeshGenerator();
				case MeshGeneratorType.UVSphere:
					return CreateUVSphereGenerator(settings as UVSphereGeneratorSettings);
				case MeshGeneratorType.SampleEliminationSphere:
					return CreateSampleEliminationSphereGenerator(settings as SampleEliminationSphereGeneratorSettings);
				default:
					throw new NotImplementedException();
			}
		}

		private MeshGenerator CreateUVSphereGenerator(UVSphereGeneratorSettings settings)
		{
			return new UVSphereMeshGenerator(settings.RingsAmount, settings.SegmentsAmount);
		}

		private MeshGenerator CreateSampleEliminationSphereGenerator(SampleEliminationSphereGeneratorSettings settings)
		{
			return new SampleEliminationSphereGenerator(settings.TargetSampleCount, settings.BaseSamplesFactor, new Seed(_seed.Random.Next()), _selector);
		}
	}
}