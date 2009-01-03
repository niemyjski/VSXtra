// ================================================================================================
// HierarchyCommandContext.cs
//
// Created: 2009.01.03, by Istvan Novak (DeepDiver)
// ================================================================================================
using System.Collections.Generic;
using System.Collections.ObjectModel;
using VSXtra.Commands;
using VSXtra.Package;

namespace VSXtra.Hierarchy
{
  // ================================================================================================
  /// <summary>
  /// This class represents command context for hierarchy commands
  /// </summary>
  // ================================================================================================
  public class HierarchyCommandContext : CommandContext
  {
    #region Private fields

    private readonly ReadOnlyCollection<HierarchyNode> _Nodes;
    private readonly CommandOrigin _Origin;
    private bool _ExplicitStatusSet;
    private bool _Supported;
    private bool _Disabled;
    private bool _Invisible;

    #endregion

    #region Lifecycle methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="HierarchyCommandContext"/> class.
    /// </summary>
    /// <param name="package">The package owning this context.</param>
    /// <param name="selection">The list of selected nodes belonging to this context.</param>
    /// <param name="origin">The command origin.</param>
    // --------------------------------------------------------------------------------------------
    public HierarchyCommandContext(PackageBase package, IList<HierarchyNode> selection,
      CommandOrigin origin) : 
      base(package)
    {
      _Nodes = new ReadOnlyCollection<HierarchyNode>(selection);
      _Origin = origin;
      Handled = true;
      _ExplicitStatusSet = false;
      _Supported = false;
      _Disabled = false;
      _Invisible = false;
    }

    #endregion

    #region Public properties

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the flag indicating that the command has been handled.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public bool Handled { get; set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the collection of selected nodes.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public ReadOnlyCollection<HierarchyNode> Nodes
    {
      get { return _Nodes; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the command origin.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public CommandOrigin Origin
    {
      get { return _Origin; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the flag if there is any node selected.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public bool HasNode
    {
      get { return _Nodes.Count > 0; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the flag indicating if a single node is selected.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public bool HasSingleNode
    {
      get { return _Nodes.Count == 1; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the flag indicating if multiple nodes are selected.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public bool HasMultipleNode
    {
      get { return _Nodes.Count > 1; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the first node from the selected ones.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public HierarchyNode Node
    {
      get { return _Nodes[0]; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the flag indicating if the command is supported in the current context or not.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public bool Supported
    {
      get { return _Supported; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the flag indicating if the command is disabled in the current context or not.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public bool Disabled
    {
      get { return _Disabled; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the flag indicating if the command is invisible in the current context or not.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public bool Invisible
    {
      get { return _Invisible; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the flag indicating if the command status is explicitly set in the current context.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public bool ExplicitStatusSet
    {
      get { return _ExplicitStatusSet; }
    }

    #endregion

    #region Public methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Explicitly sets the context status supported.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public void MarkSupported()
    {
      _ExplicitStatusSet = true;
      _Supported = true;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Explicitly sets the context status disabled.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public void MarkDisabled()
    {
      _ExplicitStatusSet = true;
      _Disabled = true;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Explicitly sets the context status invisible.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public void MarkInvisible()
    {
      _ExplicitStatusSet = true;
      _Invisible = true;
    }

    #endregion
  }
}