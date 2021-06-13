using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Master
{
	public interface MeshFormer
	{
		void Form(Mesh testMesh);
		void Form(Mesh testMesh, float size);
	}
}