using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace SBaier.Master.Test
{
    public abstract class Vector3SorterTest : ZenjectUnitTestFixture
    {
        protected readonly Vector3[][] _testInput =
        {
            new Vector3[]
            {
                new Vector3(0, 1, 2),
                new Vector3(-1, 0, 1),
                new Vector3(1, 2, 3),
                new Vector3(-2, -1, 0),
                new Vector3(2, 1, 0),
                new Vector3(2, 1, 0),
                Vector3.zero
            },
            new Vector3[]
            {
                Vector3.up,
                Vector3.left,
                Vector3.down,
                Vector3.right,
                Vector3.forward,
                Vector3.back
            },
            new Vector3[]
            {
                new Vector3(10, 9, 8),
                new Vector3(7, 8, 9),
                new Vector3(8, 7, 6),
                new Vector3(5, 6, 7),
                new Vector3(6, 5, 4),
                new Vector3(3, 4, 5),
                new Vector3(4, 3, 2),
                new Vector3(1, 2, 3),
                new Vector3(2, 1, 0),
                new Vector3(-1, 0, 1),
                new Vector3(0, -1, -2),
                new Vector3(-3, -2, -1)
            },
        };

        private readonly int[] _invalidCompareValues = new int[] { -2, 3, 6, -1, 10 };
        private readonly Vector2Int[] _ranges = new Vector2Int[]
        {
            new Vector2Int(1, 3),
            new Vector2Int(0, 5),
            new Vector2Int(2, 5),
            new Vector2Int(4, 4),
        };
        private readonly Vector2Int[] _invalidRanges = new Vector2Int[]
        {
            new Vector2Int(-1, 3),
            new Vector2Int(3, 15),
            new Vector2Int(-4, -2),
            new Vector2Int(3, 1),
            new Vector2Int(5, 0),
        };

        private Sorter<Vector3> _sorter;


        [Test]
        public void Sort_SortsInput()
        {
            for (int i = 0; i < _testInput.Length; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    GivenANewSorter();
                    Vector3[] testInput = _testInput[i].ToArray();
                    WhenSortIsCalledOn(testInput, j);
                    ThenInputIsSorted(testInput, _testInput[i].ToArray(), j);
                    Teardown();
                    Setup();
                }
            }
        }

        [Test]
        public void Sort_InvalidCompareValueIndexThrowsExpection()
        {
            for (int i = 0; i < _invalidCompareValues.Length; i++)
            {
                GivenANewSorter();
                Vector3[] testInput = _testInput[0].ToArray();
                TestDelegate test = () => WhenSortIsCalledOn(testInput, _invalidCompareValues[i]);
                ThenThrowsArgumentOutOfRangeException(test);
                Teardown();
                Setup();
            }
        }

        [Test]
        public void Sort_WithRange_SortsInputAtRange()
        {

            for (int i = 0; i < _testInput.Length; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    for (int h = 0; h < _ranges.Length; h++)
                    {
                        GivenANewSorter();
                        Vector3[] testInput = _testInput[i].ToArray();
                        WhenSortIsCalledOn(testInput, _ranges[h], j);
                        ThenInputIsSorted(testInput, _testInput[i].ToArray(), _ranges[h], j);
                        Teardown();
                        Setup();
                    }
                }
            }
        }

        [Test]
        public void Sort_InvalidIndexRangeThrowsExpection()
        {
            for (int i = 0; i < _invalidRanges.Length; i++)
            {
                GivenANewSorter();
                Vector3[] testInput = _testInput[0].ToArray();
                TestDelegate test = () => WhenSortIsCalledOn(testInput, _invalidRanges[i], 0);
                ThenThrowsArgumentOutOfRangeException(test);
                Teardown();
                Setup();
            }
        }

        [Test]
        public void Sort_WithIndexPermuations_ReturnsCorrectPermutations()
		{
            for (int i = 0; i < _testInput.Length; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    GivenANewSorter();
                    Vector3[] testInput = _testInput[i].ToArray();
                    int[] permutations = CreateInitalIndexPermuations(testInput.Length);
                    WhenSortIsCalledOn(testInput, permutations, j);
                    ThenIndexPermutationsAreAsExpected(testInput, _testInput[i], permutations, j);
                    Teardown();
                    Setup();
                }
            }
        }

		private void GivenANewSorter()
        {
            BindSorter();
            _sorter = Container.Resolve<Sorter<Vector3>>();
        }
        protected abstract void BindSorter();

        private void WhenSortIsCalledOn(Vector3[] points, int compareValueIndex)
        {
            _sorter.Sort(points, compareValueIndex);
        }

        private void WhenSortIsCalledOn(Vector3[] points, Vector2Int indexRange, int compareValueIndex)
        {
            _sorter.Sort(points, indexRange, compareValueIndex);
        }
        private void WhenSortIsCalledOn(Vector3[] points, int[] permutations, int compareValueIndex)
        {
            _sorter.Sort(points, permutations, compareValueIndex);
        }

        private void ThenInputIsSorted(Vector3[] actual, Vector3[] points, int compareValueIndex)
        {
            Vector3[] expected = points.OrderBy(p => p[compareValueIndex]).ToArray();
            for (int i = 0; i < expected.Length; i++)
                Assert.AreEqual(expected[i][compareValueIndex], actual[i][compareValueIndex]);
        }

        private void ThenInputIsSorted(Vector3[] actual, Vector3[] points, Vector2Int range, int compareValueIndex)
        {
            Vector3[] rangePoints = points.Where((p, i) => i >= range.x && i <= range.y).ToArray();
            Vector3[] expected = rangePoints.OrderBy(p => p[compareValueIndex]).ToArray();
            for (int i = 0; i < expected.Length; i++)
            {
                if (i < range.x || i > range.y)
                    Assert.AreEqual(points[i][compareValueIndex], actual[i][compareValueIndex]);
                else
                    Assert.AreEqual(expected[i - range.x][compareValueIndex], actual[i][compareValueIndex]);
            }
        }

        private void ThenIndexPermutationsAreAsExpected(Vector3[] actual, Vector3[] points, int[] permutations, int compareValueIndex)
        {
            for (int i = 0; i < points.Length; i++)
                Assert.AreEqual(actual[i][compareValueIndex], points[permutations[i]][compareValueIndex]);
        }

        private void ThenThrowsArgumentOutOfRangeException(TestDelegate test)
        {
            Assert.Throws<ArgumentOutOfRangeException>(test);
        }

        private int[] CreateInitalIndexPermuations(int count)
        {
            int[] result = new int[count];
            for (int i = 0; i < count; i++)
                result[i] = i;
            return result;
        }
    }
}