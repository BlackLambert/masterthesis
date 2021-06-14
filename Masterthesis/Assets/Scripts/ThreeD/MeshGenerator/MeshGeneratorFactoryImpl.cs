using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
    public class MeshGeneratorFactoryImpl : MeshGeneratorFactory
    {
		public MeshGenerator Create(MeshGeneratorSettings settings)
		{
			switch (settings.Type)
			{
				case MeshGeneratorType.Icosahedron:
					return new IcosahedronGenerator();
				case MeshGeneratorType.Plain:
					return new PlainGenerator();
				default:
					throw new NotImplementedException();
			}
		}
	}
}