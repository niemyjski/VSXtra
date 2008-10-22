// ================================================================================================
// WindowPaneNotFoundException.cs
//
// Created: 2008.06.29, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using System.Runtime.Serialization;

namespace VSXtra
{
  // ==================================================================================
  /// <summary>
  /// This class defines an exception describing that a Windows pane has not been found.
  /// </summary>
  // ==================================================================================
  public sealed class WindowPaneNotFoundException : Exception
  {
    // --------------------------------------------------------------------------------
    /// <summary>
    /// Creates a new instance of the exception with the specified message.
    /// </summary>
    /// <param name="message">Exception message</param>
    // --------------------------------------------------------------------------------
    public WindowPaneNotFoundException(string message)
      : base(message)
    {
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Creates a new instance of the exception with a message related to the specified
    /// type.
    /// </summary>
    /// <param name="type">Type related to the exception</param>
    // --------------------------------------------------------------------------------
    public WindowPaneNotFoundException(Type type)
      : base(MessageString(type))
    {
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Creates a new instance of the exception with a message related to the specified
    /// type and an inner exception..
    /// </summary>
    /// <param name="type">Type related to the exception</param>
    /// <param name="innerException">Inner exception instance.</param>
    // --------------------------------------------------------------------------------
    public WindowPaneNotFoundException(Type type, Exception innerException)
      : base(MessageString(type), innerException)
    {
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Creates a new instance of the exception with the specified message and inner
    /// exception.
    /// </summary>
    /// <param name="message">Exception message</param>
    /// <param name="innerException">Inner exception instance.</param>
    // --------------------------------------------------------------------------------
    public WindowPaneNotFoundException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Deserialization constructor for the exception instance.
    /// </summary>
    /// <param name="info">Serialization information</param>
    /// <param name="context">Streaming context information</param>
    // --------------------------------------------------------------------------------
    public WindowPaneNotFoundException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Creates a message string for the specified type.
    /// </summary>
    /// <param name="type">Type to create a message for.</param>
    /// <returns>Exception message.</returns>
    // --------------------------------------------------------------------------------
    private static string MessageString(Type type)
    {
      return String.Format("Window pane based on type '{0}' cannot be found.", type);
    }
  }
}
