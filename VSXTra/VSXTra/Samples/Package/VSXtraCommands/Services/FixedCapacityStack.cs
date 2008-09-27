// ================================================================================================
// FixedCapacityStack.cs
//
// This file was taken from the source of PowerCommands for Visual Studio 2008. I added only some
// comments and made some refactorings, but the essence of the code has not been changed.
//
// Created: 2008, by Pablo Galiano
// Revised: 2008.08.12, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

namespace DeepDiver.VSXtraCommands
{
  // ================================================================================================
  /// <summary>
  /// Class that represents a fixed capacity stack
  /// </summary>
  /// <typeparam name="TItem">The type of the item.</typeparam>
  // ================================================================================================
  public class FixedCapacityStack<TItem> : IEnumerable<TItem>, ICollection
  {
    #region Fields

    private readonly int _Capacity;
    private TItem[] _Array;
    private int _CurrentIndex;
    [NonSerialized] private object _SyncRoot;

    #endregion

    #region Constructors

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="FixedCapacityStack&lt;TItem&gt;"/> class.
    /// </summary>
    /// <param name="capacity">The capacity.</param>
    // --------------------------------------------------------------------------------------------
    public FixedCapacityStack(int capacity)
    {
      if (capacity == 0)
        throw new ArgumentException(Resources.CapacityShouldBeGreaterThanZero, "capacity");
      _CurrentIndex = 0;
      _Capacity = capacity;
      _Array = new TItem[capacity];
    }

    #endregion

    #region ICollection Members

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Copies the elements of the collection to an array, starting at a particular index.
    /// </summary>
    /// <param name="toArray">
    /// The one-dimensional array that is the destination of the elements copied from collection. 
    /// The array must have zero-based indexing.
    /// </param>
    /// <param name="index">
    /// The zero-based index in <paramref name="toArray"/> at which copying begins.
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// 	<paramref name="toArray"/> is null. </exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    /// 	<paramref name="index"/> is less than zero. </exception>
    /// <exception cref="T:System.ArgumentException">
    /// 	<paramref name="toArray"/> is multidimensional.-or- <paramref name="index"/> is equal to
    /// or greater than the length of <paramref name="toArray"/>.-or- The number of elements in 
    /// the source <see cref="T:System.Collections.ICollection"/> is greater than the available 
    /// space from <paramref name="index"/> to the end of the destination <paramref name="toArray"/>. 
    /// </exception>
    /// <exception cref="T:System.ArgumentException">
    /// The type of the source collection cannot be cast automatically to the type of the 
    /// destination <paramref name="toArray"/>. 
    /// </exception>
    // --------------------------------------------------------------------------------------------
    public void CopyTo(Array toArray, int index)
    {
      throw new NotImplementedException();
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the number of elements contained in the collection.
    /// </summary>
    /// <value></value>
    /// <returns>The number of elements contained in the collection.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    public int Count
    {
      get { return _CurrentIndex; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets a value indicating whether access to the collection is synchronized (thread safe).
    /// </summary>
    /// <returns>
    /// true if access to the collection is synchronized (thread safe); otherwise, false.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    public bool IsSynchronized
    {
      get { return false; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets an object that can be used to synchronize access to the collection.
    /// </summary>
    /// <returns>
    /// An object that can be used to synchronize access to the collection.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    public object SyncRoot
    {
      get
      {
        if (_SyncRoot == null)
        {
          Interlocked.CompareExchange(ref _SyncRoot, new object(), null);
        }
        return _SyncRoot;
      }
    }

    #endregion

    #region IEnumerable<TItem> Members

    /// <summary>
    /// Returns an enumerator that iterates through a collection.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
    /// </returns>
    public IEnumerator GetEnumerator()
    {
      return new Enumerator<TItem>(this);
    }

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
    /// </returns>
    IEnumerator<TItem> IEnumerable<TItem>.GetEnumerator()
    {
      return new Enumerator<TItem>(this);
    }

    #endregion

    #region

    /// <summary>
    /// Pushes an item.
    /// </summary>
    /// <param name="item">The item.</param>
    public void Push(TItem item)
    {
      if (_CurrentIndex == _Array.Length)
      {
        var destinationArray = new TItem[_Capacity];
        Array.Copy(_Array, 1, destinationArray, 0, _Capacity - 1);
        _Array = destinationArray;
        _CurrentIndex = _Array.Length - 1;
      }

      _Array[_CurrentIndex++] = item;
    }

    /// <summary>
    /// Pops an item.
    /// </summary>
    /// <returns></returns>
    public TItem Pop()
    {
      if (_CurrentIndex == 0)
      {
        throw new InvalidOperationException(Resources.EmptyStack);
      }

      TItem item = _Array[--_CurrentIndex];
      _Array[_CurrentIndex] = default(TItem);

      return item;
    }

    /// <summary>
    /// Peeks the current item.
    /// </summary>
    /// <returns></returns>
    public TItem Peek()
    {
      if (_CurrentIndex == 0)
      {
        throw new InvalidOperationException(Resources.EmptyStack);
      }

      return _Array[_CurrentIndex - 1];
    }

    /// <summary>
    /// Clears the stack.
    /// </summary>
    public void Clear()
    {
      Array.Clear(_Array, 0, _Capacity);
      _CurrentIndex = 0;
    }

    #endregion

    #region Nested type: Enumerator

    /// <summary>
    /// Enumerator for the Circular Stack
    /// </summary>
    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct Enumerator<T> : IEnumerator<TItem>
    {
      #region Fields

      private int index;
      private readonly FixedCapacityStack<TItem> stack;
      private TItem currentElement;

      #endregion

      #region Constructors

      internal Enumerator(FixedCapacityStack<TItem> stack)
      {
        this.stack = stack;
        index = -2;
        currentElement = default(TItem);
      }

      #endregion

      #region Public Implementation

      /// <summary>
      /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
      /// </summary>
      public void Dispose()
      {
        index = -1;
      }

      /// <summary>
      /// Advances the enumerator to the next element of the collection.
      /// </summary>
      /// <returns>
      /// true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.
      /// </returns>
      /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
      public bool MoveNext()
      {
        bool flag;

        if (index == -2)
        {
          index = stack._CurrentIndex - 1;
          flag = index >= 0;
          if (flag)
          {
            currentElement = stack._Array[index];
          }
          return flag;
        }
        if (index == -1)
        {
          return false;
        }
        flag = --index >= 0;
        if (flag)
        {
          currentElement = stack._Array[index];
          return flag;
        }
        currentElement = default(TItem);
        return flag;
      }

      void IEnumerator.Reset()
      {
        index = -2;
        currentElement = default(TItem);
      }

      /// <summary>
      /// Gets the current.
      /// </summary>
      /// <value>The current.</value>
      public TItem Current
      {
        get
        {
          if (index == -2)
          {
            throw new InvalidOperationException(Resources.EnumNotStarted);
          }
          if (index == -1)
          {
            throw new InvalidOperationException(Resources.EnumEnded);
          }
          return currentElement;
        }
      }

      object IEnumerator.Current
      {
        get
        {
          if (index == -2)
          {
            throw new InvalidOperationException(Resources.EnumNotStarted);
          }
          if (index == -1)
          {
            throw new InvalidOperationException(Resources.EnumEnded);
          }
          return currentElement;
        }
      }

      #endregion
    }

    #endregion
  }
}