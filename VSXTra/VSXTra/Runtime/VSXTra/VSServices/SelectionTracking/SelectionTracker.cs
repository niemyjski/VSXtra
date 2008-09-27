// ================================================================================================
// SelectionTracker.cs
//
// Created: 2008.08.04, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace VSXtra
{
  // ================================================================================================
  /// <summary>
  /// This class is a wrapper around the STrackSelection service.
  /// </summary>
  // ================================================================================================
  public class SelectionTracker
  {
    #region Private fields

    private readonly ITrackSelection _TrackSelection;
    private SelectionContainer _Container;

    #endregion

    #region Lifecycle methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates a new selection tracker instance.
    /// </summary>
    /// <param name="provider">Service provider to obtain the selection tracker from.</param>
    /// <param name="selectableRO">Is the collection of selectable objects read only?</param>
    /// <param name="selectedRO">Is the collection of selected objects read only?</param>
    // --------------------------------------------------------------------------------------------
    public SelectionTracker(IServiceProvider provider, bool selectableRO, bool selectedRO)
    {
      _TrackSelection = provider.GetService<STrackSelection, ITrackSelection>();
      _Container = new SelectionContainer(selectableRO, selectedRO);
      Unselect();
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates a new selection tracker instance.
    /// </summary>
    /// <param name="provider">Service provider to obtain the selection tracker from.</param>
    // --------------------------------------------------------------------------------------------
    public SelectionTracker(IServiceProvider provider)
      : this(provider, false, false)
    {
    }

    #endregion

    #region Public properties

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the selection container.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public SelectionContainer Container
    {
      get { return _Container; }
      set { _Container = value; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the ITrackSelection object behind this instance.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public ITrackSelection TrackSelection
    {
      get { return _TrackSelection; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the selected objects.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public IEnumerable SelectedObjects
    {
      get { return _Container == null ? new ArrayList() : _Container.SelectedObjects; }
    }

    #endregion

    #region Public events

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// The current selection has been changed.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public event EventHandler SelectionChanged
    {
      add
      {
        if (_Container != null)
          _Container.SelectedObjectsChanged += value;
      }
      remove
      {
        if (_Container != null)
          _Container.SelectedObjectsChanged -= value;
      }
    }

    #endregion

    #region Public methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets all the selected objects with the specified type.
    /// </summary>
    /// <typeparam name="T">Type to select.</typeparam>
    /// <returns>A collection of selected object instances with the specified type.</returns>
    // --------------------------------------------------------------------------------------------
    public IEnumerable<T> GetSelectedObjects<T>()
    {
      return SelectedObjects.OfType<T>();
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the first selected objects with the specified type.
    /// </summary>
    /// <typeparam name="T">Type to select.</typeparam>
    /// <returns>
    /// The first selected object instance with the specified type or null if there is no selected 
    /// object with the specified type.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    public T GetSelectedObject<T>()
    {
      return GetSelectedObjects<T>().FirstOrDefault();
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Unselects any object selected previously.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public void Unselect()
    {
      SetSelection(null, null);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Sets the provided object as selected and the only selectable.
    /// </summary>
    /// <param name="selectedObject">Object to select.</param>
    // --------------------------------------------------------------------------------------------
    public void SelectObject(object selectedObject)
    {
      ICollection container = new ArrayList {selectedObject};
      SetSelection(container, container);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Sets the provided object as selected and the list of objects as selectable.
    /// </summary>
    /// <param name="selectedObject">Object to select.</param>
    /// <param name="selectableObjects">Objects that can be selected.</param>
    // --------------------------------------------------------------------------------------------
    public void SelectObject(object selectedObject, IEnumerable selectableObjects)
    {
      SetSelection(new ArrayList {selectedObject}, ToArrayList(selectableObjects));
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Sets the list of objects as selected and selectable.
    /// </summary>
    /// <param name="selectedObjects">Objects to select.</param>
    // --------------------------------------------------------------------------------------------
    public void SelectObjects(IEnumerable selectedObjects)
    {
      SetSelection(ToArrayList(selectedObjects), ToArrayList(selectedObjects));
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Sets the provided objects as selected and the list of objects as selectable.
    /// </summary>
    /// <param name="selectedObjects">Object to select.</param>
    /// <param name="selectableObjects">Objects that can be selected.</param>
    // --------------------------------------------------------------------------------------------
    public void SelectObjects(IEnumerable selectedObjects, IEnumerable selectableObjects)
    {
      SetSelection(ToArrayList(selectedObjects), ToArrayList(selectableObjects));
    }

    #endregion

    #region Private methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Converst an IList to an ArrayList
    /// </summary>
    // --------------------------------------------------------------------------------------------
    private static ArrayList ToArrayList(IEnumerable list)
    {
      var result = new ArrayList();
      foreach (var item in list) result.Add(item);
      return result;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Carries out the selection.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    private void SetSelection(ICollection selected, ICollection selectable)
    {
      if (_Container != null)
      {
        _Container.SelectableObjects = selectable;
        _Container.SelectedObjects = selected;
      }
      if (_TrackSelection != null)
      {
        _TrackSelection.OnSelectChange(_Container);
      }
    }

    #endregion
  }
}