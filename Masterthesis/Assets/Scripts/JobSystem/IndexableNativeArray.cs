using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace SBaier.Master
{
	[StructLayout(LayoutKind.Sequential)]
	[NativeContainer]
	unsafe public struct IndexableNativeArray<T> : IList<T>, IDisposable where T : struct
	{
		private NativeArray<T> _array;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
		// Handle to tell if operations such as reading and writing can be performed safely.
		internal AtomicSafetyHandle m_Safety;

		// Handle to tell if the container has been disposed.
		// This is a managed object. It can be passed along as the job can't dispose the container, 
		// but needs to be (re)set to null on schedule to prevent job access to a managed object.
		[NativeSetClassTypeToNullOnSchedule] internal DisposeSentinel m_DisposeSentinel;
#endif
		Allocator _allocatorLabel;

		public IndexableNativeArray(T[] array, Allocator allocator)
		{
			_array = new NativeArray<T>(array, allocator);
			_allocatorLabel = allocator;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
			// Create a dispose sentinel to track memory leaks. 
			// An atomic safety handle is also created automatically.
			DisposeSentinel.Create(out m_Safety, out m_DisposeSentinel, 1, allocator);
#endif
		}


		public T this[int index]
		{
			get
			{
#if ENABLE_UNITY_COLLECTIONS_CHECKS
				AtomicSafetyHandle.CheckReadAndThrow(m_Safety);
#endif
				return _array[index];
			}
			set
			{
#if ENABLE_UNITY_COLLECTIONS_CHECKS
				AtomicSafetyHandle.CheckWriteAndThrow(m_Safety);
#endif
				_array[index] = value;
			}
		}

		public int Count => _array.Length;

		public bool IsReadOnly => false;

		public void Add(T item)
		{
			throw new System.NotImplementedException();
		}

		public void Clear()
		{
			throw new System.NotImplementedException();
		}

		public bool Contains(T item)
		{
			throw new System.NotImplementedException();
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			throw new System.NotImplementedException();
		}

		public IEnumerator<T> GetEnumerator()
		{
			return _array.GetEnumerator();
		}

		public int IndexOf(T item)
		{
			throw new System.NotImplementedException();
		}

		public void Insert(int index, T item)
		{
			throw new System.NotImplementedException();
		}

		public bool Remove(T item)
		{
			throw new System.NotImplementedException();
		}

		public void RemoveAt(int index)
		{
			throw new System.NotImplementedException();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _array.GetEnumerator();
		}

		public void Dispose()
		{
#if ENABLE_UNITY_COLLECTIONS_CHECKS
			if (!UnsafeUtility.IsValidAllocator(_allocatorLabel))
				throw new InvalidOperationException("The NativeArray can not be Disposed because it was not allocated with a valid allocator.");

			DisposeSentinel.Dispose(ref m_Safety, ref m_DisposeSentinel);
#endif
			_array.Dispose();
		}
	}
}