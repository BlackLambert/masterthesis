using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
	[CreateAssetMenu(fileName = "PlainGeneratorSettings", menuName = "Mesh/PlainGeneratorSettings")]
	public class PlainGeneratorSettings : MeshGeneratorSettings
	{
		public override MeshGeneratorType Type => MeshGeneratorType.Plain;
	}
}