// ================================================================================================
// ProvideOptionDialogPageAttribute.cs
//
// This source code is created by using the source code provided with the VS 2008 SDK. Many 
// patterns and implementation details are defined there. The code here is intended to be the base
// of a new Managed Package Framework for developing VSPackages.
// The code here is experimental and fully opened for community.
//
// Created: 2008.07.16, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using VSXtra.Properties;
using Microsoft.VisualStudio.Shell;

namespace VSXtra
{
  // ================================================================================================
  /// <summary>
  /// This class is intended to be the base class for all the attributes that are used to register 
  /// an option page.
  /// </summary>
  // ================================================================================================
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
  public abstract class XtraProvideOptionDialogPageAttribute : RegistrationAttribute
  {
    // --------------------------------------------------------------------------------------------
    /// <devdoc>
    /// Creates a new instance of the registration attribute with the specified page type and
    /// page name resource ID.
    /// </devdoc>
    /// <param name="pageType">Type of page.</param>
    /// <param name="pageNameResourceId">Page name resource ID.</param>
    // --------------------------------------------------------------------------------------------
    protected XtraProvideOptionDialogPageAttribute(Type pageType, string pageNameResourceId)
    {
      // --- Check the input type: as first make sure this is not null
      if (pageType == null)
      {
        throw new ArgumentNullException("pageType");
      }

      // --- Then make sure that it derives from DialogPage.
      if (!pageType.DerivesFromGenericTypeType(typeof(DialogPage<,>)))
      {
        throw new ArgumentException(string.Format(Resources.Culture, Resources.Package_PageNotDialogPage, pageType.FullName));
      }
      PageType = pageType;
      PageNameResourceId = pageNameResourceId;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the type of the option page provided with this attribute.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public Type PageType { get; private set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the id of the resource storing the localized name of the option page.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public string PageNameResourceId { get; private set; }
  }
}