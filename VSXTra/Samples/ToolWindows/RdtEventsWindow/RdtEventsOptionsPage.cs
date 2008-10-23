// ================================================================================================
// RdtEventsOptionsPage.cs
//
// Created: 2008.08.03, by Istvan Novak (DeepDiver)
// ================================================================================================
using System.ComponentModel;
using System.Runtime.InteropServices;
using VSXtra.Windows;

namespace DeepDiver.RdtEventsWindow
{
  // ================================================================================================
  /// <summary>
  /// This class implements an options page to tunr on/off RDT event notifications.
  /// </summary>
  // ================================================================================================
  [Guid(GuidList.guidOptionsPageString)]
  public class RdtEventsOptionsPage: DialogPage<RdtEventsWindowPackage>
  {
    #region Lifecycle methods

    public RdtEventsOptionsPage()
    {
      OptAfterAttributeChange = true;
      OptAfterDocumentWindowHide = true;
      OptAfterFirstDocumentLock = true;
      OptAfterSave = true;
      OptBeforeDocumentWindowShow = true;
      OptBeforeLastDocumentUnlock = true;
      OptAfterAttributeChangeEx = true;
      OptBeforeSave = true;
      OptAfterLastDocumentUnlock = true;
      OptAfterSaveAll = true;
      OptBeforeFirstDocumentLock = true;
    }

    #endregion

    #region Public properties

    [DisplayName("AfterAttributeChange")]
    [Category("Enabled events")]
    [DefaultValue(true)]
    public bool OptAfterAttributeChange { get; set; }

    [DisplayName("AfterDocumentWindowHide")]
    [Category("Enabled events")]
    [DefaultValue(true)]
    public bool OptAfterDocumentWindowHide { get; set; }

    [DisplayName("AfterFirstDocumentLock")]
    [Category("Enabled events")]
    [DefaultValue(true)]
    public bool OptAfterFirstDocumentLock { get; set; }

    [DisplayName("AfterSave")]
    [Category("Enabled events")]
    [DefaultValue(true)]
    public bool OptAfterSave { get; set; }

    [DisplayName("BeforeDocumentWindowShow")]
    [Category("Enabled events")]
    [DefaultValue(true)]
    public bool OptBeforeDocumentWindowShow { get; set; }

    [DisplayName("BeforeLastDocumentUnlock")]
    [Category("Enabled events")]
    [DefaultValue(true)]
    public bool OptBeforeLastDocumentUnlock { get; set; }

    [DisplayName("AfterAttributeChangeEx")]
    [Category("Enabled events")]
    [DefaultValue(true)]
    public bool OptAfterAttributeChangeEx { get; set; }

    [DisplayName("BeforeSave")]
    [Category("Enabled events")]
    [DefaultValue(true)]
    public bool OptBeforeSave { get; set; }

    [DisplayName("AfterLastDocumentUnlock")]
    [Category("Enabled events")]
    [DefaultValue(true)]
    public bool OptAfterLastDocumentUnlock { get; set; }

    [DisplayName("AfterSaveAll")]
    [Category("Enabled events")]
    [DefaultValue(true)]
    public bool OptAfterSaveAll { get; set; }

    [DisplayName("BeforeFirstDocumentLock")]
    [Category("Enabled events")]
    [DefaultValue(true)]
    public bool OptBeforeFirstDocumentLock { get; set; }

    #endregion
  }
}