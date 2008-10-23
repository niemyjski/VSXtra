// ================================================================================================
// ActivityLog.cs
//
// Created: 2008.10.22, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using Microsoft.VisualStudio.Shell.Interop;
using VSXtra.Package;

namespace VSXtra.Diagnostics
{
  // ================================================================================================
  /// <summary>
  /// This enumeration declares the types of log entries that can be used for activity
  /// log. We use this types instead of __ACTIVITYLOG_ENTRYTYPE constants.
  /// </summary>
  // ================================================================================================
  public enum ActivityLogEntryType
  {
    /// <summary>Information entry</summary>
    Information,
    /// <summary>Warning entry</summary>
    Warning,
    /// <summary>Error entry</summary>
    Error
  }

  // ================================================================================================
  /// <summary>
  /// This class defines an activity log entry with its all possible user defined
  /// properties.
  /// </summary>
  // ================================================================================================
  public sealed class ActivityLogEntry
  {
    #region Lifecyle methods

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Creates an empty log entry using default property values.
    /// </summary>
    // --------------------------------------------------------------------------------
    public ActivityLogEntry()
    {
      Type = ActivityLogEntryType.Information;
      Source = "<source not declared>";
      Message = "<no message specified>";
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Creates a log entry with the specified message.
    /// </summary>
    /// <param name="message">Message string</param>
    // --------------------------------------------------------------------------------
    public ActivityLogEntry(string message)
    {
      Hr = null;
      Guid = null;
      Message = message;
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Creates a log entry with the specified source and message.
    /// </summary>
    /// <param name="source">Source string</param>
    /// <param name="message">Message string</param>
    // --------------------------------------------------------------------------------
    public ActivityLogEntry(string source, string message)
    {
      Hr = null;
      Guid = null;
      Source = source;
      Message = message;
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Creates a log entry with the specified type and message.
    /// </summary>
    /// <param name="type">Type of the log entry</param>
    /// <param name="message">Message string</param>
    // --------------------------------------------------------------------------------
    public ActivityLogEntry(ActivityLogEntryType type, string message)
    {
      Hr = null;
      Guid = null;
      Type = type;
      Message = message;
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Creates a log entry with the specified type, source and message.
    /// </summary>
    /// <param name="type">Type of the log entry</param>
    /// <param name="source">Source string</param>
    /// <param name="message">Message string</param>
    // --------------------------------------------------------------------------------
    public ActivityLogEntry(ActivityLogEntryType type, string source, string message)
    {
      Hr = null;
      Guid = null;
      Type = type;
      Source = source;
      Message = message;
    }

    #endregion

    #region Public properties

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the type of the log entry.
    /// </summary>
    // --------------------------------------------------------------------------------
    public ActivityLogEntryType Type { get; set; }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the source of the log entry.
    /// </summary>
    // --------------------------------------------------------------------------------
    public string Source { get; set; }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the message of the log entry.
    /// </summary>
    // --------------------------------------------------------------------------------
    public string Message { get; set; }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the GUID of the log entry.
    /// </summary>
    // --------------------------------------------------------------------------------
    public Guid? Guid { get; set; }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the HRESULT of the log entry.
    /// </summary>
    // --------------------------------------------------------------------------------
    public int? Hr { get; set; }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the path of the log entry.
    /// </summary>
    // --------------------------------------------------------------------------------
    public string Path { get; set; }

    #endregion
  }

  // ================================================================================================
  /// <summary>
  /// This static class provides methods to create VS activity log entries.
  /// </summary>
  // ================================================================================================
  public static class ActivityLog
  {
    #region Public log methods

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Creates an activity logentry by the specified ActivityLogEntry instance.
    /// </summary>
    /// <param name="entry">Instance describing the log enrty properties</param>
    // --------------------------------------------------------------------------------
    public static void Write(ActivityLogEntry entry)
    {
      if (entry.Guid == null)
      {
        if (entry.Hr == null)
        {
          if (entry.Path == null)
            LogEntry(entry.Type, entry.Source, entry.Message);
          else
            LogEntryPath(entry.Type, entry.Source, entry.Message, entry.Path);
        }
        else
        {
          if (entry.Path == null)
            LogEntryHr(entry.Type, entry.Source, entry.Message, entry.Hr.Value);
          else
            LogEntryHrPath(entry.Type, entry.Source, entry.Message, entry.Hr.Value, entry.Path);
        }
      }
      else
      {
        if (entry.Hr == null)
        {
          if (entry.Path == null)
            LogEntryGuid(entry.Type, entry.Source, entry.Message, entry.Guid.Value);
          else
            LogEntryGuidPath(entry.Type, entry.Source, entry.Message, entry.Guid.Value, entry.Path);
        }
        else
        {
          if (entry.Path == null)
            LogEntryGuidHr(entry.Type, entry.Source, entry.Message, entry.Guid.Value, entry.Hr.Value);
          else
            LogEntryGuidHrPath(entry.Type, entry.Source, entry.Message, entry.Guid.Value, entry.Hr.Value, entry.Path);
        }
      }
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Creates an activity log entry with the specified source and message.
    /// </summary>
    /// <param name="source">Event source</param>
    /// <param name="message">Event message</param>
    // --------------------------------------------------------------------------------
    public static void Write(string source, string message)
    {
      Write(ActivityLogEntryType.Information, source, message);
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Creates an activity log entry with the specified source, message and GUID.
    /// </summary>
    /// <param name="source">Event source</param>
    /// <param name="message">Event message</param>
    /// <param name="guid">GUID of the entry</param>
    // --------------------------------------------------------------------------------
    public static void Write(string source, string message, Guid guid)
    {
      Write(ActivityLogEntryType.Information, source, message, guid);
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Creates an activity log entry with the specified source, message, GUID 
    /// and HRESULT.
    /// </summary>
    /// <param name="source">Event source</param>
    /// <param name="message">Event message</param>
    /// <param name="guid">GUID of the entry</param>
    /// <param name="hr">HRESULT of the entry</param>
    // --------------------------------------------------------------------------------
    public static void Write(string source, string message, Guid guid, int hr)
    {
      Write(ActivityLogEntryType.Information, source, message, guid, hr);
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Creates an activity log entry with the specified source, message and HRESULT.
    /// </summary>
    /// <param name="source">Event source</param>
    /// <param name="message">Event message</param>
    /// <param name="hr">HRESULT of the entry</param>
    // --------------------------------------------------------------------------------
    public static void Write(string source, string message, int hr)
    {
      Write(ActivityLogEntryType.Information, source, message, hr);
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Creates an activity log entry with the specified source, message, GUID, 
    /// HRESULT and path.
    /// </summary>
    /// <param name="source">Event source</param>
    /// <param name="message">Event message</param>
    /// <param name="guid">GUID of the entry</param>
    /// <param name="hr">HRESULT of the entry</param>
    /// <param name="path">Path of the message</param>
    // --------------------------------------------------------------------------------
    public static void Write(string source, string message, Guid guid, int hr, string path)
    {
      Write(ActivityLogEntryType.Information, source, message, guid, hr, path);
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Creates an activity log entry with the specified source, message, GUID
    /// and path.
    /// </summary>
    /// <param name="source">Event source</param>
    /// <param name="message">Event message</param>
    /// <param name="guid">GUID of the entry</param>
    /// <param name="path">Path of the message</param>
    // --------------------------------------------------------------------------------
    public static void Write(string source, string message, Guid guid, string path)
    {
      Write(ActivityLogEntryType.Information, source, message, guid, path);
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Creates an activity log entry with the specified source, message, HRESULT 
    /// and path.
    /// </summary>
    /// <param name="source">Event source</param>
    /// <param name="message">Event message</param>
    /// <param name="hr">HRESULT of the entry</param>
    /// <param name="path">Path of the message</param>
    // --------------------------------------------------------------------------------
    public static void Write(string source, string message, int hr, string path)
    {
      Write(ActivityLogEntryType.Information, source, message, hr, path);
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Creates an activity log entry with the specified source, message and path.
    /// </summary>
    /// <param name="source">Event source</param>
    /// <param name="message">Event message</param>
    /// <param name="path">Path of the message</param>
    // --------------------------------------------------------------------------------
    public static void Write(string source, string message, string path)
    {
      Write(ActivityLogEntryType.Information, source, message, path);
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Creates an activity log entry with the specified type, source and message.
    /// </summary>
    /// <param name="type">Entry type</param>
    /// <param name="source">Event source</param>
    /// <param name="message">Event message</param>
    // --------------------------------------------------------------------------------
    public static void Write(ActivityLogEntryType type, string source, string message)
    {
      LogEntry(type, source, message);
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Creates an activity log entry with the specified type, source, message 
    /// and GUID.
    /// </summary>
    /// <param name="type">Entry type</param>
    /// <param name="source">Event source</param>
    /// <param name="message">Event message</param>
    /// <param name="guid">GUID of the entry</param>
    // --------------------------------------------------------------------------------
    public static void Write(ActivityLogEntryType type, string source, string message, Guid guid)
    {
      LogEntryGuid(type, source, message, guid);
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Creates an activity log entry with the specified type, source, message, GUID 
    /// and HRESULT.
    /// </summary>
    /// <param name="type">Entry type</param>
    /// <param name="source">Event source</param>
    /// <param name="message">Event message</param>
    /// <param name="guid">GUID of the entry</param>
    /// <param name="hr">HRESULT of the entry</param>
    // --------------------------------------------------------------------------------
    public static void Write(ActivityLogEntryType type, string source, string message, Guid guid, int hr)
    {
      LogEntryGuidHr(type, source, message, guid, hr);
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Creates an activity log entry with the specified type, source, message 
    /// and HRESULT.
    /// </summary>
    /// <param name="type">Entry type</param>
    /// <param name="source">Event source</param>
    /// <param name="message">Event message</param>
    /// <param name="hr">HRESULT of the entry</param>
    // --------------------------------------------------------------------------------
    public static void Write(ActivityLogEntryType type, string source, string message, int hr)
    {
      LogEntryHr(type, source, message, hr);
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Creates an activity log entry with the specified type, source, message, GUID, 
    /// HRESULT and path.
    /// </summary>
    /// <param name="type">Entry type</param>
    /// <param name="source">Event source</param>
    /// <param name="message">Event message</param>
    /// <param name="guid">GUID of the entry</param>
    /// <param name="hr">HRESULT of the entry</param>
    /// <param name="path">Path of the message</param>
    // --------------------------------------------------------------------------------
    public static void Write(ActivityLogEntryType type, string source, string message, Guid guid, int hr, string path)
    {
      LogEntryGuidHrPath(type, source, message, guid, hr, path);
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Creates an activity log entry with the specified type, source, message, GUID
    /// and path.
    /// </summary>
    /// <param name="type">Entry type</param>
    /// <param name="source">Event source</param>
    /// <param name="message">Event message</param>
    /// <param name="guid">GUID of the entry</param>
    /// <param name="path">Path of the message</param>
    // --------------------------------------------------------------------------------
    public static void Write(ActivityLogEntryType type, string source, string message, Guid guid, string path)
    {
      LogEntryGuidPath(type, source, message, guid, path);
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Creates an activity log entry with the specified type, source, message, HRESULT 
    /// and path.
    /// </summary>
    /// <param name="type">Entry type</param>
    /// <param name="source">Event source</param>
    /// <param name="message">Event message</param>
    /// <param name="hr">HRESULT of the entry</param>
    /// <param name="path">Path of the message</param>
    // --------------------------------------------------------------------------------
    public static void Write(ActivityLogEntryType type, string source, string message, int hr, string path)
    {
      LogEntryHrPath(type, source, message, hr, path);
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Creates an activity log entry with the specified type, source, message 
    /// and path.
    /// </summary>
    /// <param name="type">Entry type</param>
    /// <param name="source">Event source</param>
    /// <param name="message">Event message</param>
    /// <param name="path">Path of the message</param>
    // --------------------------------------------------------------------------------
    public static void Write(ActivityLogEntryType type, string source, string message, string path)
    {
      LogEntryPath(type, source, message, path);
    }

    #endregion

    #region Private log methods

    // All log methods here call the corresponding IVsActivityLog method by their arguments.
    // The Log property obtains the reference to the IVsActivityLog interface.
    // The ActivityLogEntryType values are mapped to __ACTIVITYLOG_ENTRYTYPE values.

    private static IVsActivityLog Log
    {
      get { return PackageBase.GetGlobalService<SVsActivityLog, IVsActivityLog>(); }
    }

    private static void LogEntry(ActivityLogEntryType type, string source, string message)
    {
      IVsActivityLog log = Log;
      if (log  != null)
      {
        log.LogEntry(VsxConverter.MapLogTypeToAle(type), source, message);
      }
    }

    private static void LogEntryGuid(ActivityLogEntryType type, string source, string message,
                                     Guid guid)
    {
      IVsActivityLog log = Log;
      if (log != null)
      {
        log.LogEntryGuid(VsxConverter.MapLogTypeToAle(type), source, message, guid);
      }
    }

    private static void LogEntryGuidHr(ActivityLogEntryType type, string source, string message,
                                       Guid guid, int hr)
    {
      IVsActivityLog log = Log;
      if (log != null)
      {
        log.LogEntryGuidHr(VsxConverter.MapLogTypeToAle(type), source, message, guid, hr);
      }
    }

    private static void LogEntryGuidHrPath(ActivityLogEntryType type, string source, string message,
                                           Guid guid, int hr, string path)
    {
      IVsActivityLog log = Log;
      if (log != null)
      {
        log.LogEntryGuidHrPath(VsxConverter.MapLogTypeToAle(type), source, message, guid, hr, path);
      }
    }

    private static void LogEntryGuidPath(ActivityLogEntryType type, string source, string message,
                                         Guid guid, string path)
    {
      IVsActivityLog log = Log;
      if (log != null)
      {
        log.LogEntryGuidPath(VsxConverter.MapLogTypeToAle(type), source, message, guid, path);
      }
    }

    private static void LogEntryHr(ActivityLogEntryType type, string source, string message,
                                   int hr)
    {
      IVsActivityLog log = Log;
      if (log != null)
      {
        log.LogEntryHr(VsxConverter.MapLogTypeToAle(type), source, message, hr);
      }
    }

    private static void LogEntryHrPath(ActivityLogEntryType type, string source, string message,
                                       int hr, string path)
    {
      IVsActivityLog log = Log;
      if (log != null)
      {
        log.LogEntryHrPath(VsxConverter.MapLogTypeToAle(type), source, message, hr, path);
      }
    }

    private static void LogEntryPath(ActivityLogEntryType type, string source, string message,
                                     string path)
    {
      IVsActivityLog log = Log;
      if (log != null)
      {
        log.LogEntryPath(VsxConverter.MapLogTypeToAle(type), source, message, path);
      }
    }

    #endregion
  }
}