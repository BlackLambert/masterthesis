using Zenject;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace SBaier.Master.Test
{
	[TestFixture]
	public class FloatBinaryHeapTest : BinaryHeapTest<float>
	{
		private readonly List<float>[] _elements = new List<float>[]
		{
			new List<float>() { -2.6f, -5.0f, 1.6f, 5.2f, 12.0f, 8.9f, 1.5f, 0 },
			new List<float>(){ 35.8f, 23.3f, 72.2f, 134.1f, 12.8f, 14.4f, 83.8f, 99.9f, 101.3f},
			new List<float>(){ -13.4f, -5.0f, -1.2f, -15.6f, -2.2f },
			new List<float>(){ 0, 0, 0, 2.0f, 2.0f, 1.2f, 0, 1.3f, 2.5f },
		};

		private readonly float[] _changeValues = new float[] { -210.6f, 0, 200.2f, -5.6f };

		protected override float[] GetChangeValues()
		{
			return _changeValues;
		}

		protected override IList<float>[] GetTestValues()
		{
			return _elements;
		}
	}
}