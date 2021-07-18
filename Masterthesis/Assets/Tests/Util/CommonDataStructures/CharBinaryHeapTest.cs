using Zenject;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace SBaier.Master.Test
{
	[TestFixture]
	public class CharBinaryHeapTest : BinaryHeapTest<char>
	{
		private readonly List<char>[] _elements = new List<char>[]
		{
			new List<char>() { '2', 'b', '0', 'v', 'r', 'a', 'e', 'q' },
			new List<char>(){ '3', 'w', 'u', 'r', 't', '4', '8', '4', '7' },
			new List<char>(){ '4', 's', 'k', 'b', 'c' },
			new List<char>(){ 'a', 'a', 'a', 'c', 'c', 'b', 'a', 'b', 'c' },
		};

		private readonly char[] _changeValues = new char[] { 'z', '0', '_', '5' };

		protected override char[] GetChangeValues()
		{
			return _changeValues;
		}

		protected override IList<char>[] GetTestValues()
		{
			return _elements;
		}
	}
}