using UnityEngine;

namespace SBaier.Master
{
	public interface MeshGenerator
	{
		void GenerateMeshFor(Mesh mesh);
		void GenerateMeshFor(Mesh mesh, float size);
	}
}