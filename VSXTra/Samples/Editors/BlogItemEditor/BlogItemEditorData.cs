// ================================================================================================
// BlogItemEditorData.cs
//
// Created: 2008.08.30, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using System.IO;
using System.Xml.Linq;

namespace DeepDiver.BlogItemEditor
{
  // ==================================================================================
  /// <summary>
  /// This class represents the data belonging to a BlogItemEditor.
  /// </summary>
  // ==================================================================================
  public sealed class BlogItemEditorData : IXmlPersistable
  {
    #region Constant values

    public const string BlogItemLiteral = "BlogItem";
    public const string BlogItemNamespace = "http://www.codeplex.com/VSXtra/BlogItemv1.0";
    public const string BodyLiteral = "Body";
    public const string CategoriesLiteral = "Categories";
    public const string CategoryLiteral = "Category";
    public const string TitleLiteral = "Title";

    #endregion

    #region XName values

    private readonly XName BlogItemXName = XName.Get(BlogItemLiteral, BlogItemNamespace);
    private readonly XName BodyXName = XName.Get(BodyLiteral, BlogItemNamespace);
    private readonly XName CategoriesXName = XName.Get(CategoriesLiteral, BlogItemNamespace);
    private readonly XName CategoryXName = XName.Get(CategoryLiteral, BlogItemNamespace);
    private readonly XName TitleXName = XName.Get(TitleLiteral, BlogItemNamespace);

    #endregion

    #region Lifecycle methods

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Creates a new instance of BlogItemEditorData with empty property values.
    /// </summary>
    // --------------------------------------------------------------------------------
    public BlogItemEditorData()
    {
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Creates a new instance of BlogItemEditorData
    /// </summary>
    /// <param name="title">Title of the blog post</param>
    /// <param name="categories">Categories of the blog post</param>
    /// <param name="body">Body of the blogpost</param>
    // --------------------------------------------------------------------------------
    public BlogItemEditorData(string title, string categories, string body)
    {
      Title = title;
      Categories = categories;
      Body = body;
    }

    #endregion

    #region Public properties

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Gets the title of the blog post.
    /// </summary>
    // --------------------------------------------------------------------------------
    public string Title { get; set; }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Gets the categories string of the blog post.
    /// </summary>
    // --------------------------------------------------------------------------------
    public string Categories { get; set; }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Gets the body text of the blog post.
    /// </summary>
    // --------------------------------------------------------------------------------
    public string Body { get; set; }

    #endregion

    #region Persistance methods

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Saves the object data into the specified file.
    /// </summary>
    /// <param name="fileName">File to save the data into</param>
    // --------------------------------------------------------------------------------
    public void SaveTo(string fileName)
    {
      // --- Create the root document element
      var root = new XElement(BlogItemXName);
      var objectDoc = new XDocument(root);

      // --- Save document data to XElement and then tofile
      SaveTo(root);
      objectDoc.Save(fileName, SaveOptions.DisableFormatting);
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Reads the object data from the specified file.
    /// </summary>
    /// <param name="fileName">File to read the data from</param>
    // --------------------------------------------------------------------------------
    public void ReadFrom(string fileName)
    {
      string fileContent = File.ReadAllText(fileName);
      XDocument objectDoc = XDocument.Parse(fileContent, LoadOptions.PreserveWhitespace);

      // --- Check the document element
      XElement root = objectDoc.Element(BlogItemXName);
      if (root == null)
        throw new InvalidOperationException(
          "Root '" + BlogItemLiteral + "' element cannot be found.");

      // --- Read the document
      ReadFrom(root);
    }

    #endregion

    #region IXmlPersistable members

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Saves the object to an XElement.
    /// </summary>
    /// <param name="targetElement">Target XElement.</param>
    // --------------------------------------------------------------------------------
    public void SaveTo(XElement targetElement)
    {
      // --- Create title
      targetElement.Add(new XElement(TitleXName, Title));

      // --- Create category hierarchy
      var categories = new XElement(CategoriesXName);
      targetElement.Add(categories);
      string[] categoryList = Categories.Split(';');
      foreach (string category in categoryList)
      {
        string trimmed = category.Trim();
        if (trimmed.Length > 0)
        {
          categories.Add(new XElement(CategoryXName, trimmed));
        }
      }

      // --- Create the body
      targetElement.Add(
        new XElement(BodyXName,
                     new XCData(Body))
        );
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Loads an object from an XElement.
    /// </summary>
    /// <param name="sourceElement">Source XElement.</param>
    // --------------------------------------------------------------------------------
    public void ReadFrom(XElement sourceElement)
    {
      // --- Get the title
      XElement titleElement = sourceElement.Element(TitleXName);
      if (titleElement == null)
        throw new InvalidOperationException("'Title' element is missing");
      Title = titleElement.Value;

      // --- Obtain categories
      Categories = string.Empty;
      XElement categoriesElement = sourceElement.Element(CategoriesXName);
      if (categoriesElement != null)
      {
        foreach (XElement categoryElement in categoriesElement.Elements(CategoryXName))
        {
          if (Categories.Length > 0) Categories += "; ";
          Categories += categoryElement.Value;
        }
      }

      // --- Get the post body
      XElement bodyElement = sourceElement.Element(BodyXName);
      if (bodyElement == null)
        throw new InvalidOperationException("'Body' element is missing");
      Body = bodyElement.Value;
      Body = Body.Replace("\n", "\r\n");
    }

    #endregion
  }
}