using Zenject;
using NUnit.Framework;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using Moq;

namespace SBaier.Master.Test
{
    [TestFixture]
    public class Vector3BinaryKDTreeTest : ZenjectUnitTestFixture
    {
        private Vector3[][] _testPoints =
        {
            new Vector3[]
            {
                new Vector3(-3.5f, 5f, 3f),
                new Vector3(2f, 4.3f, -2.9f),
                Vector3.zero,
                Vector3.left,
                new Vector3(-8.1f, 24.5f, -5f),
                new Vector3(-3.5f, 5f, 12f)
            },
            new Vector3[]
            {
                new Vector3(1.6f, 0.3f, 0.7f),
                new Vector3(-4.5f, 3.8f, 0.8f),
                Vector3.zero
            },
            new Vector3[] {
                new Vector3(7.2f, 5.6f, -1.2f),
                new Vector3(7.5f, -4.2f, 7.6f),
                Vector3.up,
                new Vector3(9.9f, -2.5f, -7.3f),
                new Vector3(-2f, -4f, -1f),
                Vector3.down,
                new Vector3(-3, 7, -2),
                new Vector3(12.5f, 11.1f, 6.8f)
            }
        };

        private Vector3[] _samples = 
        { 
            new Vector3(2f, 4.3f, -2.91f), 
            new Vector3(0.1f, 1f, 0), 
            new Vector3(0, -0.01f, 0), 
            new Vector3(12f, 7f, -4f), 
            new Vector3(-5.1f, 7.8f, 4.7f)
        };

        private float[] _testMaxDistances = new float[] { 1.5f, 2.5f, 7f, 0.3f, 0.01f };
        private float[] _invalidMaxDistance = new float[] { 0, -12f, -0.03f, -7.2f };



        [Test]
        public void Constructor_ThrowsExpectionOnEmptyVector3CollectionInput()
        {
            TestDelegate test = () => GivenAKDTree(new Vector3[0]);
            ThenThrowsArgumentException(test);
        }

        [Test]
        public void Depth_get_ReturnsExpectedValue()
		{
            for (int i = 0; i < _testPoints.Length; i++)
            {
                GivenAKDTree(_testPoints[i]);
                Vector3BinaryKDTree tree = Container.Resolve<Vector3BinaryKDTree>();
                ThenDepthReturnsExpectedValue(tree, _testPoints[i]);
                Teardown();
                Setup();
            }
        }

        [Test]
        public void GetNearestTo_ReturnsExpectedValue()
        {
            for (int i = 0; i < _testPoints.Length; i++)
            {
                for (int j = 0; j < _samples.Length; j++)
                {
                    GivenAKDTree(_testPoints[i]);
                    Vector3BinaryKDTree tree = Container.Resolve<Vector3BinaryKDTree>();
                    int result = WhenGetNearestToIsCalledOn(tree, _samples[j]);
                    ThenResultingNearestIsAsExpected(_testPoints[i], _samples[j], result);
                    Teardown();
                    Setup();
                }
            }
        }

        [Test]
        public void GetNearestToWithin_ThrowsExceptionOnInvalidRadius()
        {
            for (int i = 0; i < _invalidMaxDistance.Length; i++)
            {
                GivenAKDTree(_testPoints[0]);
                Vector3BinaryKDTree tree = Container.Resolve<Vector3BinaryKDTree>();
                TestDelegate test = () => WhenGetNearestToWithinIsCalled(tree, _samples[0], _invalidMaxDistance[i]);
                ThenThrowsArgumentOutOfRangeException(test);
                Teardown();
                Setup();
            }
        }

        [Test]
        public void GetNearestToWithin_ReturnsExpectedValues()
        {
            for (int i = 0; i < _testPoints.Length; i++)
            {
                for (int j = 0; j < _samples.Length; j++)
                {
                    GivenAKDTree(_testPoints[i]);
                    Vector3BinaryKDTree tree = Container.Resolve<Vector3BinaryKDTree>();
                    IList<int> result = WhenGetNearestToWithinIsCalled(tree, _samples[j], _testMaxDistances[j]);
                    ThenResultingNearestIsAsExpected(_testPoints[i], _samples[j], _testMaxDistances[j], result);
                    Teardown();
                    Setup();
                }
            }
        }

		private void GivenAKDTree(Vector3[] vectors)
		{
            Container.Bind<Vector3QuickSelector>().FromMethod(() => CreateMockedSelector(vectors)).AsTransient();
            Vector3BinaryKDTree tree = new Vector3BinaryKDTree(vectors, Container.Resolve<Vector3QuickSelector>());
            Container.Bind<Vector3BinaryKDTree>().FromInstance(tree).AsTransient();
        }

		private int WhenGetNearestToIsCalledOn(Vector3BinaryKDTree tree, Vector3 sample)
        {
            return tree.GetNearestTo(sample);
        }

        private IList<int> WhenGetNearestToWithinIsCalled(Vector3BinaryKDTree tree, Vector3 sample, float radius)
        {
            return tree.GetNearestToWithin(sample, radius);
        }

        private void ThenThrowsArgumentException(TestDelegate test)
		{
            Assert.Throws<ArgumentException>(test);
        }

        private void ThenThrowsArgumentOutOfRangeException(TestDelegate test)
		{
            Assert.Throws<ArgumentOutOfRangeException>(test);
        }

        private void ThenDepthReturnsExpectedValue(Vector3BinaryKDTree tree, Vector3[] vectors)
        {
            int actual = tree.Depth;
            int expected = Mathf.FloorToInt(Mathf.Log(vectors.Length, 2));

            Assert.AreEqual(expected, actual, $"The depth for a vectors input of length {vectors.Length} should be {expected} and not {actual}");
        }

        private void ThenResultingNearestIsAsExpected(Vector3[] points, Vector3 sample, int actual)
        {
            int nearest = -1;
            float nearestDistance = float.PositiveInfinity;
            for(int i = 0; i < points.Length; i++)
			{
                Vector3 point = points[i];
                float distance = (sample - point).magnitude;
                if(distance < nearestDistance)
				{
                    nearest = i;
                    nearestDistance = distance;
                }
			}

            Assert.AreEqual(nearest, actual);
        }

        private void ThenResultingNearestIsAsExpected(Vector3[] points, Vector3 sample, float maxDistance, IList<int> actual)
        {
            List<int> expected = new List<int>();

            for (int i = 0; i < points.Length; i++)
            {
                Vector3 point = points[i];
                float distance = (sample - point).magnitude;
                if (distance <= maxDistance)
                    expected.Add(i);
            }

            actual = actual.OrderBy(i => i).ToArray();
            Assert.AreEqual(expected.ToArray(), actual);
        }

        private Vector3QuickSelector CreateMockedSelector(Vector3[] points)
        {
            points = points.ToArray();
            Mock<Vector3QuickSelector> sorterMock = new Mock<Vector3QuickSelector>();
            sorterMock.Setup(s => s.QuickSelect(It.IsAny<IList<Vector3>>(), It.IsAny<IList<int>>(), It.IsAny<Vector2Int>(), It.IsAny<int>(), It.IsAny<int>())).
                Callback<IList<Vector3>, IList<int>, Vector2Int, int, int>((p, perm, r, c, sI) => BasicSort(p, perm, r, c, points));
            return sorterMock.Object;
        }

        private void BasicSort(IList<Vector3> points, IList<int> permutations, Vector2Int range, int compareValueIndex, IList<Vector3> originalPoints)
		{
            Vector3[] rangePoints = points.Where((p, i) => i >= range.x && i <= range.y).ToArray();
            Vector3[] orderedRange = rangePoints.OrderBy(p => p[compareValueIndex]).ToArray();

            for(int i = range.x; i <= range.y; i++ )
			{
                int innerIndex = i - range.x;
                permutations[i] = originalPoints.IndexOf(orderedRange[innerIndex]);
                points[i] = orderedRange[innerIndex];
            }
        }
    }
}