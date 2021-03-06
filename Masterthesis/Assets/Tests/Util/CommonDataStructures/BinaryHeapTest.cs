using Zenject;
using NUnit.Framework;
using System;
using System.Linq;
using System.Collections.Generic;

namespace SBaier.Master.Test
{
    [TestFixture]
    public abstract class BinaryHeapTest<T> : ZenjectUnitTestFixture where T : IComparable<T>
    {
        private Heap<T> _heap;

        protected abstract IList<T>[] GetTestValues();
        protected abstract T[] GetChangeValues();

        private int[] _invalidIndices = new int[]
        {
            -1, 200, -3
        };

        private int[] _testGetIndices = new int[]
        {
            0, 32, 45, 2, 6
        };


        [Test]
        public void Peek_ReturnsExpectedValue()
		{
            IList<T>[] testValues = GetTestValues();
            bool[] decendingOption = new bool[] { false, true };
			for (int i = 0; i < testValues.Length; i++)
			{
				for (int j = 0; j < decendingOption.Length; j++)
				{
                    GivenANewHeap(testValues[i].ToList(), decendingOption[j]);
                    T value = WhenPeekIsCalledOnHeap();
                    ThenPeekValueIsAsExpected(testValues[i].ToList(), value, decendingOption[j]);
                    Teardown();
                    Setup();
                }
			}
		}

        [Test]
        public void Pop_ReturnsExpectedValue()
		{
            IList<T>[] testValues = GetTestValues();
            bool[] decendingOption = new bool[] { false, true };
            for (int i = 0; i < testValues.Length; i++)
            {
                for (int j = 0; j < decendingOption.Length; j++)
                {
                    GivenANewHeap(testValues[i].ToList(), decendingOption[j]);
                    int popedIndex = WhenPopIsCalledOnHeap();
                    ThenPopedIndexValueIsAsExpected(testValues[i].ToList(), popedIndex, decendingOption[j]);
                    Teardown();
                    Setup();
                }
            }
        }

        [Test]
        public void Pop_NextElementIsAsExpected()
        {
            IList<T>[] testValues = GetTestValues();
            bool[] decendingOption = new bool[] { false, true };
            for (int i = 0; i < testValues.Length; i++)
            {
                for (int j = 0; j < decendingOption.Length; j++)
                {
                    IList<T> testValuesCopy = testValues[i].ToList();
                    GivenANewHeap(testValuesCopy, decendingOption[j]);
                    WhenPopIsCalledOnHeap();
                    T value = WhenPeekIsCalledOnHeap();
                    ThenPeekValueIsAsExpected(testValuesCopy, value, decendingOption[j], 1);
                    Teardown();
                    Setup();
                }
            }
        }

        [Test]
        public void HasElementBeenRemoved_ReturnsTrueForPopedElement()
        {
            IList<T>[] testValues = GetTestValues();
            bool[] decendingOption = new bool[] { false, true };
            for (int i = 0; i < testValues.Length; i++)
            {
                for (int j = 0; j < decendingOption.Length; j++)
                {
                    IList<T> testValuesCopy = testValues[i].ToList();
                    GivenANewHeap(testValuesCopy, decendingOption[j]);
                    int removedIndex = WhenPopIsCalledOnHeap();
                    bool actual = WhenHasElementBeenRemovedIsCalledOnHeap(removedIndex);
                    Assert.AreEqual(true, actual);
                    Teardown();
                    Setup();
                }
            }
        }

		[Test]
        public void ChangeElementAt_PeekReturnsExpectedValue()
        {
            IList<T>[] testValues = GetTestValues();
            bool[] decendingOption = new bool[] { false, true };

            // Iterate test values
            for (int i = 0; i < testValues.Length; i++)
            {
                T[] changeValues = GetChangeValues().ToArray();
                // Iterate change values
                for (int j = 0; j < changeValues.Length; j++)
                {
                    T changeValue = changeValues[j];
                    // Iterate all values of test values
                    for (int l = 0; l < testValues[i].Count; l++)
					{
                        // Iterate heap modes
                        for (int k = 0; k < decendingOption.Length; k++)
                        {
                            IList<T> testValuesCopy = testValues[i].ToList();
                            GivenANewHeap(testValuesCopy, decendingOption[k]);
                            WhenChangeElementAtIsCalledOn(l, changeValue);
                            T value = WhenPeekIsCalledOnHeap();
                            ThenPeekValueIsAsExpected(testValuesCopy, value, decendingOption[k]);
                            Teardown();
                            Setup();
                        }
                    }
                }
            }
        }


        [Test]
        public void ChangeElementAt_ThrowsExceptionOnInvalidIndex()
        {
            IList<T>[] testValues = GetTestValues();
            bool[] decendingOption = new bool[] { false, true };

            // Iterate test values
            for (int i = 0; i < testValues.Length; i++)
            {
				for (int j = 0; j < _invalidIndices.Length; j++)
				{
					for (int k = 0; k < decendingOption.Length; k++)
					{
                        IList<T> testValuesCopy = testValues[i].ToList();
                        GivenANewHeap(testValuesCopy, decendingOption[k]);
                        TestDelegate test = () => WhenChangeElementAtIsCalledOn(_invalidIndices[j], testValuesCopy[0]);
                        ThenThrowsArgumentOutOfRangeException(test);
                        Teardown();
                        Setup();
                    }
				}
            }
        }

        [Test]
        public void GetElementAt_ReturnsExpectedValue()
        {
            IList<T>[] testValues = GetTestValues();
            bool[] decendingOption = new bool[] { false, true };

            // Iterate test values
            for (int i = 0; i < testValues.Length; i++)
            {
                for (int j = 0; j < decendingOption.Length; j++)
                {
                    for (int k = 0; k < _testGetIndices.Length; k++)
                    {
                        IList<T> testValuesCopy = testValues[i].ToList();
                        GivenANewHeap(testValuesCopy, decendingOption[j]);
                        int index = _testGetIndices[k] % testValuesCopy.Count;
                        T value = WhenGetElementAtIsCalled(index);
                        Assert.AreEqual(testValuesCopy[index], value);
                        Teardown();
                        Setup();
                    }
                }
            }
        }


        [Test]
        public void GetElementAt_ThrowsExceptionOnInvalidIndex()
        {
            IList<T>[] testValues = GetTestValues();
            bool[] decendingOption = new bool[] { false, true };

            // Iterate test values
            for (int i = 0; i < testValues.Length; i++)
            {
                for (int j = 0; j < _invalidIndices.Length; j++)
                {
                    for (int k = 0; k < decendingOption.Length; k++)
                    {
                        IList<T> testValuesCopy = testValues[i].ToList();
                        GivenANewHeap(testValuesCopy, decendingOption[k]);
                        TestDelegate test = () => WhenGetElementAtIsCalled(_invalidIndices[j]);
                        ThenThrowsArgumentOutOfRangeException(test);
                        Teardown();
                        Setup();
                    }
                }
            }
        }

        private void GivenANewHeap(IList<T> values, bool decending = false)
		{
            Container.Bind<Heap<T>>().To<BinaryHeap<T>>().FromMethod(() => CreateBinaryHeap(values, decending)).AsTransient();
            _heap = Container.Resolve<Heap<T>>();
        }

		private BinaryHeap<T> CreateBinaryHeap(IList<T> values, bool decending)
		{
            return new BinaryHeap<T>(values, decending);
        }

        private T WhenPeekIsCalledOnHeap()
        {
            return _heap.Peek();
        }

        private int WhenPopIsCalledOnHeap()
        {
            return _heap.Pop();
        }

        private T WhenGetElementAtIsCalled(int index)
        {
            return _heap.GetElementAt(index);
        }

        private void WhenChangeElementAtIsCalledOn(int index, T changeValue)
        {
            _heap.ChangeElementAt(index, changeValue);
        }

        private bool WhenHasElementBeenRemovedIsCalledOnHeap(int removedIndex)
        {
            return _heap.HasElementBeenRemoved(removedIndex);
        }

        private void ThenPeekValueIsAsExpected(IList<T> elements, T actual, bool decending, int index = 0)
        {
            elements = Order(elements, decending);
            Assert.AreEqual(elements[index], actual);
        }

        private void ThenPopedIndexValueIsAsExpected(IList<T> elements, int popedIndex, bool decending)
        {
            IList<T> orderedElements = Order(elements, decending);
            Assert.AreEqual(orderedElements[0], elements[popedIndex]);
        }

        private void ThenThrowsArgumentOutOfRangeException(TestDelegate test)
        {
            Assert.Throws<ArgumentOutOfRangeException>(test);
        }

        private IList<T> Order(IList<T> elements, bool decending)
		{
            if (decending)
                return elements.OrderByDescending(v => v).ToArray();
            else
                return elements.OrderBy(v => v).ToArray();
        }
    }
}