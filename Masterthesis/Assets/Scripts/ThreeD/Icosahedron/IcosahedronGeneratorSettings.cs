using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
	[CreateAssetMenu(fileName = "IcosahedronGeneratorSettings", menuName = "Mesh/IcosahedronGeneratorSettings")]
	public class IcosahedronGeneratorSettings : MeshGeneratorSettings
	{
		public override MeshGeneratorType Type => MeshGeneratorType.Icosahedron;
	}
}