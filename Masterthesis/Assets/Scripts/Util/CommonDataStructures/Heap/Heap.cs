using System;

namespace SBaier.Master
{
	public interface Heap<T> : Heap where T : IComparable<T>
	{
		T Peek();
		int Pop();
		void ChangeElementAt(int index, T newElement);
		T GetElementAt(int index);
		bool HasElementBeenRemoved(int removedIndex);
	}

	public interface Heap
	{
		public class ElementRemovedException : Exception { }
	}
}