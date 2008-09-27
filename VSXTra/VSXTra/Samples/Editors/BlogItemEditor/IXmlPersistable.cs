using System.Xml.Linq;

namespace DeepDiver.BlogItemEditor
{
  // ====================================================================================
  /// <summary>
  /// This interface defines the behaviour of objects that can be persisted to and read 
  /// from XElement (LINQ XML elements) instances.
  /// </summary>
  // ====================================================================================
  public interface IXmlPersistable
  {
    // --------------------------------------------------------------------------------
    /// <summary>
    /// Saves the object to an XElement.
    /// </summary>
    /// <param name="targetElement">Target XElement.</param>
    // --------------------------------------------------------------------------------
    void SaveTo(XElement targetElement);

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Loads an object from an XElement.
    /// </summary>
    /// <param name="sourceElement">Source XElement.</param>
    // --------------------------------------------------------------------------------
    void ReadFrom(XElement sourceElement);
  }
}
