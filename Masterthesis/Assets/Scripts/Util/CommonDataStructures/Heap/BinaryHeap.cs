using System;
using System.Collections.Generic;
using System.Linq;

namespace SBaier.Master
{
	/// <summary>
	/// Source: https://de.wikipedia.org/wiki/Bin%C3%A4rer_Heap
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class BinaryHeap<T> : Heap<T> where T : IComparable<T>
	{
		private List<int> _internToOriginalIndices;
		private int[] _originalToInternalIndices;
		private IList<T> _elementes;
		private bool _decending = false;
		private Func<T, T, bool> _compareFunction;

		public BinaryHeap(IList<T> elements, bool decending = false)
		{
			_elementes = elements;
			_decending = decending;
			_compareFunction = SelectCompareFunction();
			Build();
		}

		private Func<T, T, bool> SelectCompareFunction()
		{
			if (_decending)
				return IsHigher;
			else
				return IsLower;
		}

		private void Build()
		{
			_internToOriginalIndices = CreateIndexPermutations(_elementes.Count);
			_originalToInternalIndices = _internToOriginalIndices.ToArray();

			for (int i = _internToOriginalIndices.Count / 2 - 1; i >= 0; i--)
				Heapify(i);
		}

		private void Heapify(int index)
		{
			int left = LeftOf(index);
			int right = RightOf(index);
			int count = _internToOriginalIndices.Count;

			int min = index;
			if (left < count && IsUnordered(left, min))
				min = left;
			if (right < count && IsUnordered(right, min))
				min = right;
			if(min != index)
			{
				Swap(index, min);
				Heapify(min);
			}
		}

		private void Swap(int i0, int i1)
		{
			_originalToInternalIndices[_internToOriginalIndices[i0]] = i1;
			_originalToInternalIndices[_internToOriginalIndices[i1]] = i0;

			int indexPermutation = _internToOriginalIndices[i0];
			_internToOriginalIndices[i0] = _internToOriginalIndices[i1];
			_internToOriginalIndices[i1] = indexPermutation;
		}

		private int ParentOf(int index)
		{
			return (index - 1) / 2;
		}

		private int LeftOf(int index)
		{
			return 2 * index + 1;
		}

		private int RightOf(int index)
		{
			return 2 * index + 2;
		}

		private bool IsUnordered(int element0Index, int element1Index)
		{
			T element = GetElement(element0Index);
			T minElement = GetElement(element1Index);
			return _compareFunction(element, minElement);
		}

		private bool IsHigher(T x0, T x1)
		{
			return x0.CompareTo(x1) > 0;
		}

		private bool IsLower(T x0, T x1)
		{
			return x0.CompareTo(x1) < 0;
		}

		private T GetElement(int index)
		{
			return _elementes[_internToOriginalIndices[index]];
		}

		private List<int> CreateIndexPermutations( int count)
		{
			List<int> result = new List<int>(count);
			for (int i = 0; i < count; i++)
				result.Add(i);
			return result;
		}

		public T Peek()
		{
			return _elementes[_internToOriginalIndices[0]];
		}

		public int Pop()
		{
			return RemoveAt(0);
		}

		private int RemoveAt(int internalIndex)
		{
			int removedIndex = _internToOriginalIndices[internalIndex];
			int lastIndex = _internToOriginalIndices.Count - 1;
			Swap(internalIndex, lastIndex);
			_originalToInternalIndices[removedIndex] = -1;
			_internToOriginalIndices.RemoveAt(lastIndex);

			if (internalIndex == lastIndex)
				return removedIndex;
			if (internalIndex == 0 || !IsUnordered(internalIndex, ParentOf(internalIndex)))
				Heapify(internalIndex);
			else
				MoveUp(internalIndex);

			return removedIndex;
		}

		private void MoveUp(int index)
		{
			while (index > 0 && IsUnordered(index, ParentOf(index)))
			{
				Swap(index, ParentOf(index));
				index = ParentOf(index);
			}
		}

		public void ChangeElementAt(int index, T newElement)
		{
			ValidateIndex(index);
			int internalIndex = _originalToInternalIndices[index];
			ValidateInternalIndex(internalIndex);
			_elementes[index] = newElement;
			if (internalIndex == 0 || !IsUnordered(internalIndex, ParentOf(internalIndex)))
				Heapify(internalIndex);
			else
				MoveUp(internalIndex);
		}

		private void ValidateIndex(int index)
		{
			if (index < 0 || index >= _elementes.Count)
				throw new ArgumentOutOfRangeException();
		}

		private static void ValidateInternalIndex(int internalIndex)
		{
			if (internalIndex < 0)
				throw new Heap.ElementRemovedException();
		}

		public T GetElementAt(int index)
		{
			return _elementes[index];
		}

		public bool HasElementBeenRemoved(int index)
		{
			return _originalToInternalIndices[index] < 0;
		}
	}
}