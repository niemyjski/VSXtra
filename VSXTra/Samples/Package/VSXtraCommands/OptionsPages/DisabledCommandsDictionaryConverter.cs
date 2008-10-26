// ================================================================================================
// DisabledCommandsDictionaryConverter.cs
//
// This file was taken from the source of PowerCommands for Visual Studio 2008. I added only some
// comments and made some refactorings, but the essence of the code has not been changed.
//
// Created: 2008, by Pablo Galiano
// Revised: 2008.07.19, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Globalization;
using System.Linq;
using System.Text;
using VSXtra;

namespace DeepDiver.VSXtraCommands
{
  // ================================================================================================
  /// <summary>
  /// TypeConverter for Disabled Command collection
  /// </summary>
  // ================================================================================================
  public class DisabledCommandsDictionaryConverter : StringConverter
  {
    #region Fields

    private IList<CommandID> _DisabledCommands;

    #endregion

    #region Public Implementation

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets a value indicating whether this converter can convert an object in the given source 
    /// type to a string using the specified context.
    /// </summary>
    /// <param name="context">An ITypeDescriptorContext" that provides a format context.</param>
    /// <param name="sourceType">A Type that represents the type you wish to convert from.</param>
    /// <returns>
    /// true if this converter can perform the conversion; otherwise, false.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
      return true;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Returns whether this converter can convert the object to the specified type, using the 
    /// specified context.
    /// </summary>
    /// <param name="context">An ITypeDescriptorContext that provides a format context.</param>
    /// <param name="destinationType">A Type that represents the type you want to convert to.</param>
    /// <returns>
    /// true if this converter can perform the conversion; otherwise, false.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
    {
      return true;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Converts the given value object to the specified type, using the specified context and 
    /// culture information.
    /// </summary>
    /// <param name="context">An ITypeDescriptorContext that provides a format context.</param>
    /// <param name="culture">
    /// A CultureInfo instance. If null is passed, the current culture is assumed.
    /// </param>
    /// <param name="value">The object to convert.</param>
    /// <param name="destinationType">The Type to convert the value parameter to.</param>
    /// <returns>
    /// An object that represents the converted value.
    /// </returns>
    /// <exception cref="ArgumentNullException">The destinationType parameter is null.</exception>
    /// <exception cref="NotSupportedException">The conversion cannot be performed.</exception>
    // --------------------------------------------------------------------------------------------
    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
                                     Type destinationType)
    {
      _DisabledCommands = value as IList<CommandID>;
      var builder = new StringBuilder();
      _DisabledCommands.ForEach(
        cmdId => builder.Append(string.Format("{0},{1};", cmdId.Guid, cmdId.ID)));
      return builder.ToString();
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Converts the specified value object to a String object.
    /// </summary>
    /// <param name="context">An ITypeDescriptorContext that provides a format context.</param>
    /// <param name="culture">The <see cref="T:System.Globalization.CultureInfo"/> to use.</param>
    /// <param name="value">The <see cref="T:System.Object"/> to convert.</param>
    /// <returns>
    /// An Object that represents the converted value.
    /// </returns>
    /// <exception cref="NotSupportedException">The conversion could not be performed.</exception>
    // --------------------------------------------------------------------------------------------
    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
      _DisabledCommands = new List<CommandID>();
      try
      {
        if (!string.IsNullOrEmpty(value.ToString()))
        {
          value.ToString().Split(';').ForEach(
            item =>
              {
                string[] subItems = item.Split(',');
                if (subItems.Count() == 2)
                {
                  var cmdGuid = new Guid(subItems[0]);
                  int cmdId;
                  int.TryParse(subItems[1], out cmdId);
                  _DisabledCommands.Add(new CommandID(cmdGuid, cmdId));
                }
              });
        }
      }
      catch (SystemException)
      {
        // --- This exception is intentionaly caught
      }
      return _DisabledCommands;
    }

    #endregion
  }
}