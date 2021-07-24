using UnityEngine;

namespace SBaier.Master
{
	public class Icosahedron
	{
		private const float _e0 = 0.525731112119133606f;
		private const float _e1 = 0.850650808352039932f;
		private const float _e2 = 0;

		public static readonly Vector3[] Vertices =
		{
			new Vector3(-_e0, _e2, _e1),
			new Vector3(_e0, _e2, _e1),
			new Vector3(-_e0, _e2, -_e1),
			new Vector3(_e0, _e2, -_e1),

			new Vector3(_e2, _e1, _e0),
			new Vector3(_e2, _e1, -_e0),
			new Vector3(_e2, -_e1, _e0),
			new Vector3(_e2, -_e1, -_e0),

			new Vector3(_e1, _e0, _e2),
			new Vector3(-_e1, _e0, _e2),
			new Vector3(_e1, -_e0, _e2),
			new Vector3(-_e1, -_e0, _e2),
		};

		public static readonly int[] Triangles =
		{
			0, 1, 4, //0
			0, 4, 9, //1
			9, 4, 5, //2
			4, 8, 5, //3
			4, 1, 8, //4
			8, 1, 10, //5
			8, 10, 3, //6
			5, 8, 3, //7
			5, 3, 2, //8
			2, 3, 7, //9
			7, 3, 10, //10
			7, 10, 6, //11
			7, 6, 11, //12
			11, 6, 0, //13
			0, 6, 1, //14
			6, 10, 1, //15
			9, 11, 0, //16
			9, 2, 11, //17
			9, 5, 2, //18
			7, 11, 2 //19
		};

		public static readonly ConnectedEdge[][] ConnectedEdges =
		{
			new ConnectedEdge[] {new ConnectedEdge(14, 2),			new ConnectedEdge(4, 0, true),		new ConnectedEdge(1, 0) }, //0
			new ConnectedEdge[] {new ConnectedEdge(0, 2),			new ConnectedEdge(2, 0, true),		new ConnectedEdge(16, 2) }, //1
			new ConnectedEdge[] {new ConnectedEdge(1, 1, true),		new ConnectedEdge(3, 2, true),		new ConnectedEdge(18, 0) }, //2
			new ConnectedEdge[] {new ConnectedEdge(4, 2),			new ConnectedEdge(7, 0, true),		new ConnectedEdge(2, 1, true) }, //3
			new ConnectedEdge[] {new ConnectedEdge(0, 1, true),		new ConnectedEdge(5, 0, true),		new ConnectedEdge(3, 0) }, //4
			new ConnectedEdge[] {new ConnectedEdge(4, 1, true),		new ConnectedEdge(15, 1),			new ConnectedEdge(6, 0) }, //5
			new ConnectedEdge[] {new ConnectedEdge(5, 2),			new ConnectedEdge(10, 1),			new ConnectedEdge(7, 1, true) }, //6
			new ConnectedEdge[] {new ConnectedEdge(3, 1, true),		new ConnectedEdge(6, 2, true),		new ConnectedEdge(8, 0) }, //7
			new ConnectedEdge[] {new ConnectedEdge(7, 2),			new ConnectedEdge(9, 0, true),		new ConnectedEdge(18, 1, true) }, //8
			new ConnectedEdge[] {new ConnectedEdge(8, 1, true),		new ConnectedEdge(10, 0, true),		new ConnectedEdge(19, 2) }, //9
			new ConnectedEdge[] {new ConnectedEdge(9, 1, true),		new ConnectedEdge(6, 1),			new ConnectedEdge(11, 0) }, //10
			new ConnectedEdge[] {new ConnectedEdge(10, 2),			new ConnectedEdge(15, 0, true),		new ConnectedEdge(12, 0) }, //11
			new ConnectedEdge[] {new ConnectedEdge(11, 2),			new ConnectedEdge(13, 0, true),		new ConnectedEdge(19, 0) }, //12
			new ConnectedEdge[] {new ConnectedEdge(12, 1, true),	new ConnectedEdge(14, 0, true),		new ConnectedEdge(16, 1, true) }, //13
			new ConnectedEdge[] {new ConnectedEdge(13, 1, true),	new ConnectedEdge(15, 2, true),		new ConnectedEdge(0, 0) }, //14
			new ConnectedEdge[] {new ConnectedEdge(11, 1, true),	new ConnectedEdge(5, 1),			new ConnectedEdge(14, 1, true) }, //15
			new ConnectedEdge[] {new ConnectedEdge(17, 2),			new ConnectedEdge(13, 2, true),		new ConnectedEdge(1, 2) }, //16
			new ConnectedEdge[] {new ConnectedEdge(18, 2),			new ConnectedEdge(19, 1),			new ConnectedEdge(16, 0) }, //17
			new ConnectedEdge[] {new ConnectedEdge(2, 2),			new ConnectedEdge(8, 2, true),		new ConnectedEdge(17, 0) }, //18
			new ConnectedEdge[] {new ConnectedEdge(12, 2),			new ConnectedEdge(17, 1),			new ConnectedEdge(9, 2) } //19
		};

		public struct ConnectedEdge
		{
			public short FaceIndex { get; }
			public short EdgeIndex { get; }
			public bool Aligned { get; }

			public ConnectedEdge(
				short faceIndex,
				short edgeIndex)
			{
				FaceIndex = faceIndex;
				EdgeIndex = edgeIndex;
				Aligned = false;
			}

			public ConnectedEdge(
				short faceIndex,
				short edgeIndex,
				bool aligned)
			{
				FaceIndex = faceIndex;
				EdgeIndex = edgeIndex;
				Aligned = aligned;
			}
		}
	}
}