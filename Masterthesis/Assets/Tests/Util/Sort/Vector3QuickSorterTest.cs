using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SBaier.Master.Test
{
	public class Vector3QuickSorterTest : Vector3SorterTest
	{
		private QuickSelector<Vector3> _selector;

		private int[] _selectedIndices = new int[]
		{
			3, 0, 4, 2, 1
		};

		private int[] _invalidSelectedIndices = new int[]
		{
			-1, -4, 21, 100
		};

		protected override void BindSorter()
		{
			Func<Vector3, int, float> compareValueSelect = (p, i) => p[i];
			Container.Bind(typeof(Sorter<Vector3>), typeof(QuickSelector<Vector3>)).
				To<QuickSorter<Vector3, float>>().AsTransient().WithArguments(compareValueSelect);
		}

		[Test]
		public void QuickSelect_HasSortetValueAtSelectedIndex()
		{
			for (int i = 0; i < _testInput.Length; i++)
			{
				for (int j = 0; j < _selectedIndices.Length; j++)
				{
					GivenANewQuickSelector();
					Vector3[] points = _testInput[i].ToArray();
					WhenQuickSelectIsCalledOn(points, _selectedIndices[j]);
					ThenPointAtSelectedIndexIsAsExpected(points, _testInput[i], 0, _selectedIndices[j]);
					Teardown();
					Setup();
				}
			}
		}

		[Test]
		public void QuickSelect_ThrowsExceptionInInvalidSelectedIndex()
		{
			for (int i = 0; i < _invalidSelectedIndices.Length; i++)
			{
				GivenANewQuickSelector();
				TestDelegate test = () => WhenQuickSelectIsCalledOn(_testInput[0], _invalidSelectedIndices[i]);
				ThenThrowsArgumentOutOfRangeException(test);
				Teardown();
				Setup();
			}
		}

		private void GivenANewQuickSelector()
		{
			BindSorter();
			_selector = Container.Resolve<QuickSelector<Vector3>>();
		}

		private void WhenQuickSelectIsCalledOn(Vector3[] points, int selectedIndex)
		{
			_selector.QuickSelect(points, new int[points.Length], new Vector2Int(0, points.Length - 1), 0, selectedIndex);
		}

		private void ThenPointAtSelectedIndexIsAsExpected(Vector3[] actual, Vector3[] testInput, int compareValueIndex, int selectedIndex)
		{
			Vector3[] expected = testInput.ToArray();
			expected = expected.OrderBy(v => v[compareValueIndex]).ToArray();
			Assert.AreEqual(expected[selectedIndex][compareValueIndex], actual[selectedIndex][compareValueIndex]);
		}

		private void ThenThrowsArgumentOutOfRangeException(TestDelegate test)
		{
			Assert.Throws<ArgumentOutOfRangeException>(test);
		}
	}
}