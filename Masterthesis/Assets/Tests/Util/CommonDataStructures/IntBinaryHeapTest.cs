using Zenject;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace SBaier.Master.Test
{
	[TestFixture]
	public class IntBinaryHeapTest : BinaryHeapTest<int>
	{
		private readonly List<int>[] _elements = new List<int>[]
		{
			new List<int>() { -2, -5, 1, 5, 12, 8, 1, 0 },
			new List<int>(){ 35, 23, 72, 134, 12, 14, 83, 99, 101 },
			new List<int>(){ -13, -5, -1, -15, -2 },
			new List<int>(){ 0, 0, 0, 2, 2, 1, 0, 1, 2 },
		};

		private readonly int[] _changeValues = new int[] { -210, 0, 200, -5 };

		protected override int[] GetChangeValues()
		{
			return _changeValues;
		}

		protected override IList<int>[] GetTestValues()
		{
			return _elements;
		}
	}
}