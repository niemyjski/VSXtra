// ================================================================================================
// EventHooker.cs
//
// Created: 2008.08.03, by Istvan Novak (DeepDiver)
// ================================================================================================
using VSXtra.Diagnostics;

namespace VSXtra
{
  // ================================================================================================
  /// <summary>
  /// This class declares a utility for hooking Visual Studio events and map them for standard .NET
  /// events.
  /// </summary>
  /// <typeparam name="TEvent">Type of object involving event handlers.</typeparam>
  // ================================================================================================
  public abstract class EventHooker<TEvent>
    where TEvent: class, new()
  {
    #region Private fields

    private uint _EventCookie;

    #endregion

    #region Public properties

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the instance providing event methods.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public TEvent Events { get; private set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the counter showing how many subscribers of this event hook has.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public int AdviseCount { get; private set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the cookie for the event handler
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public uint EventCookie
    {
      get { return _EventCookie; }
    }

    #endregion

    #region Abstract methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Calls the advise method on the related event provider class.
    /// </summary>
    /// <param name="cookie">The cookie to be used in the Unadvise method.</param>
    // --------------------------------------------------------------------------------------------
    protected abstract int Advise(out uint cookie);

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Calls the unadvise method on the related event provider class.
    /// </summary>
    /// <param name="cookie">The cookie created in the Advise method.</param>
    // --------------------------------------------------------------------------------------------
    protected abstract int Unadvise(uint cookie);

    #endregion

    #region Public methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Ensures that IVsRunningDocTableEvents is advised when required.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public void EnsureAdvise()
    {
      lock (this)
      {
        if (AdviseCount == 0)
        {
          Events = new TEvent();
          NativeMethods.ThrowOnFailure(Advise(out _EventCookie));
        }
        AdviseCount++;
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Ensures that IVsRunningDocTableEvents is unadvised when required.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public void EnsureUnadvise()
    {
      lock (this)
      {
        if (AdviseCount == 0)
        {
          VsDebug.Fail("EnsureUnadvise has been called with a zero-valued AdviceCount.");
          return;
        }
        AdviseCount--;
        if (AdviseCount == 0)
        {
          NativeMethods.ThrowOnFailure(Unadvise(_EventCookie));
          Events = null;
        }
      }
    }

    #endregion
  }
}