using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public class MeshGeneratorFactoryImpl : MeshGeneratorFactory
    {
		public MeshGeneratorFactoryImpl()
		{
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
				default:
					throw new NotImplementedException();
			}
		}

		private MeshGenerator CreateUVSphereGenerator(UVSphereGeneratorSettings settings)
		{
			return new UVSphereMeshGenerator(settings.RingsAmount, settings.SegmentsAmount);
		}
	}
}