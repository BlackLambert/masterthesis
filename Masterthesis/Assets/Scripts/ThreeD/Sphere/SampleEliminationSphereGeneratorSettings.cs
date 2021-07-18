using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
	[CreateAssetMenu(fileName = "SampleEliminationSphereGeneratorSettings", menuName = "Mesh/SampleEliminationSphereGeneratorSettings")]
	public class SampleEliminationSphereGeneratorSettings : MeshGeneratorSettings
	{
		public override MeshGeneratorType Type => MeshGeneratorType.SampleEliminationSphere;

		[SerializeField]
		private int _targetSampleCount = 1024;
		public int TargetSampleCount => _targetSampleCount;
		[SerializeField]
		private float _baseSamplesFactor = 3.0f;
		public float BaseSamplesFactor => _baseSamplesFactor;
	}
}