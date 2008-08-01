// ================================================================================================
// RegistryEntry.cs
//
// Created: 2008.07.28, by Istvan Novak (DeepDiver)
// ================================================================================================
namespace VSXtra
{
  #region RegistryEntry class

  // ================================================================================================
  /// <summary>
  /// This type represents a registry entry having the specified type.
  /// </summary>
  /// <typeparam name="TValue">Type of registry entry</typeparam>
  // ================================================================================================
  public class RegistryEntry<TValue>
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates an entry with the specified key and value.
    /// </summary>
    /// <param name="key">Key of the registry entry</param>
    /// <param name="value">Value of the registry entry</param>
    // --------------------------------------------------------------------------------------------
    public RegistryEntry(string key, TValue value)
    {
      Key = key;
      Value = value;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the key of the registry entry.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public string Key { get; private set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the value of the registry entry.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public TValue Value { get; set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the string representation of the entry.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public override string ToString()
    {
      return Value.ToString();
    }
  }

  #endregion

  #region FileEntry

  // ================================================================================================
  /// <summary>
  /// This type represents a registry entry holding a file or folder name.
  /// </summary>
  // ================================================================================================
  public class FileEntry : RegistryEntry<string>
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates an entry with the specified key and value.
    /// </summary>
    /// <param name="key">Key of the registry entry</param>
    /// <param name="value">Value of the registry entry</param>
    // --------------------------------------------------------------------------------------------
    public FileEntry(string key, string value)
      : base(key, value)
    {
    }
  }

  #endregion
}