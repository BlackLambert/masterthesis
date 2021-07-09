using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
	public interface MeshGeneratorFactory
	{
		MeshGenerator Create(MeshGeneratorSettings settings);

	}
}