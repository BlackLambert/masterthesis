using System;

namespace SBaier.Master
{
	public interface Heap<T> : Heap
	{
		T Peek();
		int Pop();
		void ChangeElementAt(int index, T newElement);
	}

	public interface Heap
	{
		public class ElementRemovedException : Exception { }
	}
}