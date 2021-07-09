using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
	[CreateAssetMenu(fileName = "CubeGeneratorSettings", menuName = "Mesh/CubeGeneratorSettings")]
	public class CubeGeneratorSettings : MeshGeneratorSettings
	{
		public override MeshGeneratorType Type => MeshGeneratorType.Cube;
	}
}