// ================================================================================================
// ItemMap.cs
//
// Created: 2008.09.09, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using System.Collections;
using System.Collections.Generic;
using VSXtra.Hierarchy;

namespace VSXtra
{
  // ================================================================================================
  /// <summary>
  /// Maps objects to and from integer "cookies".  This helps in the implementation
  /// of VS interfaces that have Advise/Unadvise methods, for example, IVsHierarchy,
  /// IVsCfgProvider2, IVsBuildableProjectCfg and so on.
  /// </summary>
  // ================================================================================================
  [CLSCompliant(false)]
  public class ItemMap<T> : IEnumerable<T>
    where T: class
  {
    #region Private fields

    List<T> _Map;

    #endregion

    #region Public members

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the total number of sinks in the collection.
    /// </summary>
    /// <remarks>
    /// Some of these might be null though.
    /// </remarks>
    // --------------------------------------------------------------------------------------------
    public int Count
    {
      get { return (_Map == null) ? 0 : _Map.Count; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Add an event sink and return it's cookie.
    /// </summary>
    /// <param name="item">Item to add to the collection.</param>
    // --------------------------------------------------------------------------------------------
    public uint Add(T item)
    {
      if (item == null)
        throw new ArgumentNullException("item");

      // --- Re-use empty slots so the List doesn't grow infinitely.
      for (int i = 0, n = GetMap().Count; i < n; i++)
      {
        if (_Map[i] == null)
        {
          _Map[i] = item;
          return (uint)i + 1; // --- Cookie must be one based else VS doesn't call Unadvise
        }
      }
      _Map.Add(item);
      return (uint)_Map.Count;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Remove the specified event sink from the collection
    /// </summary>
    /// <param name="item">Item to remove from the collextion.</param>
    // --------------------------------------------------------------------------------------------
    public void Remove(T item)
    {
      if (item == null)
        throw new ArgumentNullException("item");

      if (_Map != null)
      {
        for (int i = 0, n = _Map.Count; i < n; i++)
        {
          if (_Map[i] == item)
          {
            _Map[i] = null; // --- This gap will be reused.
            if (i == n - 1)
            {
              // --- Compact the list whenever possible.
              while (i > 0 && _Map[i - 1] == null)
              {
                i--;
              }
              _Map.RemoveRange(i, n - i);
            }
            return;
          }
        }
      }
      throw new ArgumentOutOfRangeException("item");
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Remove the specified event sink by the cookie integer returned from the Add method.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public void RemoveAt(uint cookie)
    {
      if (_Map != null)
      {
        _Map[(int)cookie - 1] = null;  // --- Cookie is 1-based
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Update the event sink associated with the given cookie.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public void SetAt(uint cookie, T value)
    {
      GetMap()[(int)cookie - 1] = value;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Update the event sink associated with the given cookie.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public void SetAt(HierarchyId id, T value)
    {
      SetAt((uint)id, value);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Indexer access to the item.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public T this[uint cookie]
    {
      get
      {
        return (_Map != null && cookie > 0 && cookie <= _Map.Count) 
          ? _Map[(int)cookie - 1] 
          : null;
      }
      set
      {
        GetMap()[(int)cookie - 1] = value;
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Indexer access to the item.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public T this[HierarchyId id]
    {
      get { return this[(uint) id]; }
      set { this[(uint) id] = value; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Remove all event sinks.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public void Clear()
    {
      if (_Map != null) _Map.Clear();
    }

    #endregion

    #region Private methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the map instance.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    private List<T> GetMap()
    {
      if (_Map == null) _Map = new List<T>();
      return _Map;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Get all non-null elements for enumeration.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    private IEnumerable<T> GetNonNullItems()
    {
      foreach(var item in _Map)
        if (item !=null) yield return item;
    }

    #endregion

    #region IEnumerable<T> implementation

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
      return GetNonNullItems().GetEnumerator();
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Returns an enumerator that iterates through a collection.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate 
    /// through the collection.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetNonNullItems().GetEnumerator();
    }

    #endregion
  }
}