// ================================================================================================
// BlogItemEditorFactory.cs
//
// Created: 2008.08.30, by Istvan Novak (DeepDiver)
// ================================================================================================
using System.Runtime.InteropServices;
using DeepDiver.BlogItemEditor;
using VSXtra;
using VSXtra.Editors;

namespace DeepDiver.BlogItemEditor
{
  // ==================================================================================
  /// <summary>
  /// This class represents the editor factory for the BlogItemEditor.
  /// </summary>
  // ==================================================================================
  [Guid(GuidList.GuidBlogEditorFactoryString)]
  public sealed class BlogItemEditorFactory: 
    EditorFactoryBase<BlogItemEditorPackage,BlogItemEditorPane>
  {
  }
}
