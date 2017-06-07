# VSXtra

#### VSXtra support for Add-Ins

VSXtra now supports using objects like OutputWindow, VsMessageBox, ActivityLog, RunningDocumentTable, etc. from Add-Ins and from other kinds of Visual Studio Extension artifacts. Feng implemented a great solution through the SiteManager static class that opens VSXtra for Add-Ins and other extensions not only for packages. You can find a sample Add-In named VSXtraMessageBox in the Samples\AddIns folder.

#### WpfToolWindowPane by Shawn Hempel added to VSXtra

Thanks to Shawn, VSXtra now supports WPF tool windows through the WpfToolWindowPane class. For details see the samples below.

### Latest articles:

*   [LearnVSXNow! Part #40 - Working with Hierarchies Part 5 - Managed Classes for Custom Hierarchies](http://dotneteers.net/blogs/divedeeper/archive/2008/12/05/LearnVSXNowPart40.aspx)
*   [LearnVSXNow! Part #39 - Working with Hierarchies Part 4 - Hierarchy Windows](http://dotneteers.net/blogs/divedeeper/archive/2008/12/03/LearnVSXNowPart39.aspx)

**View more related articles below the demonstration code extracts.**  

#### [VSXtra Feature List](http://vsxtra.codeplex.com/wikipage?title=VSXtraFeatures&referringTitle=Home)

### Demonstration code extracts

The following code extracts give you a feeling about how VSPackages written with VSXtra look like:

#### Custom Hierarchies

*   **Example 1**: [Basic Hierarchy Tool Window](http://vsxtra.codeplex.com/wikipage?title=BasicUIHierarchyToolWindow&referringTitle=Home)
*   **Example 2**: [Dynamic Hierarchy Sample](http://vsxtra.codeplex.com/wikipage?title=DynamicHierarchySample&referringTitle=Home)

#### Packages and Services

*   **Example 1**: [VSXtra-like implementation of the VS SDK's OptionsPage Sample](http://vsxtra.codeplex.com/wikipage?title=OptionsPage&referringTitle=Home)
*   **Example 2**: [VSXtra-like implementation of the VS SDK's Services Reference Sample](http://vsxtra.codeplex.com/wikipage?title=ServicesReference&referringTitle=Home)
*   **Example 3**: [PowerCommands implementation (1) - Command Registration](http://vsxtra.codeplex.com/wikipage?title=PowerCommandsPart1&referringTitle=Home)
*   **Example 4**: [PowerCommands implementation (2) - Clear Recent Lists](http://vsxtra.codeplex.com/wikipage?title=PowerCommandsPart2&referringTitle=Home)
*   **Example 5**: [Writing to the Activity Log](http://vsxtra.codeplex.com/wikipage?title=WriteToActivityLog&referringTitle=Home)
*   **Example 6**: [Listing Sited VSXtra Packages](http://vsxtra.codeplex.com/wikipage?title=DisplayVSXtraPackages&referringTitle=Home)

#### Tool Windows

*   **Example 1**: [Simple Tool Window Sample](http://vsxtra.codeplex.com/wikipage?title=SimpleToolWindow&referringTitle=Home)
*   **Example 2**: [Dynamic Tool Window Sample](http://vsxtra.codeplex.com/wikipage?title=DynamicToolWindow&referringTitle=Home)
*   **Example 3**: [Persisted Tool Window Sample](http://vsxtra.codeplex.com/wikipage?title=PersistedToolWindow&referringTitle=Home)
*   **Example 4**: [Creating Multiple Tool Windows](http://vsxtra.codeplex.com/wikipage?title=MultiToolWindows&referringTitle=Home)
*   **Example 5**: [VSXtra-like implementation of the VS SDK's RDT Event Explorer Sample](http://vsxtra.codeplex.com/wikipage?title=RDTEventExplorer&referringTitle=Home)
*   **Example 6**: [Simple WPF Tool Window Sample](http://vsxtra.codeplex.com/wikipage?title=WPFSimpleToolWindow&referringTitle=Home)

#### Menus and Commands

*   **Example 1**: [Package with simple menu command](http://vsxtra.codeplex.com/wikipage?title=SimpleMenuCommand&referringTitle=Home)
*   **Example 2**: [VSXtra-like implementation of the VS SDK's MenusAndCommands Sample](http://vsxtra.codeplex.com/wikipage?title=DynamicCommands&referringTitle=Home)
*   **Example 3**: [VSXtra-like implementation of the VS SDK's Combobox Sample](http://vsxtra.codeplex.com/wikipage?title=ComboboxCommands&referringTitle=Home)

#### Custom Editors (New!)

*   **Example 1**: [Blog Item Editor](http://vsxtra.codeplex.com/wikipage?title=BlogItemEditor&referringTitle=Home)

#### Output Windows and Panes

*   **Example 1**: [Package that loads automatically and puts messages to the output window](http://vsxtra.codeplex.com/wikipage?title=OutputWithAutoLoad&referringTitle=Home)
*   **Example 2**: [Visual Studio registry output](http://vsxtra.codeplex.com/wikipage?title=RegistryOutput&referringTitle=Home)
*   **Example 3**: [Using custom output panes](http://vsxtra.codeplex.com/wikipage?title=CustomOutputPanes&referringTitle=Home)

As the project goes on, more samples are to come.

### Related articles:

*   [Announcement: VSXtra Shifts to a New Implementation Phase](http://dotneteers.net/blogs/divedeeper/archive/2008/10/22/announcement-vsxtra-shifts-to-a-new-implementation-phase.aspx)
*   [LearnVSXNow! Part #36: Working with Hierarchies Part 3 - Properties and Hierarchy Traversal](http://dotneteers.net/blogs/divedeeper/archive/2008/10/16/LearnVSXNowPart36.aspx)
*   [LearnVSXNow #35: Working with Hierarchies Part 2 - Internal Structure of Hierarchies](http://dotneteers.net/blogs/divedeeper/archive/2008/10/09/LearnVSXNowPart35.aspx)
*   [LearnVSXNow #34: Working with Hierarchies Part 1 - Hierarchy Basics](http://dotneteers.net/blogs/divedeeper/archive/2008/10/07/LearnVSXNowPart34.aspx)
*   [LearnVSXNow #33: VSXtra at DevCon - Part 2](http://dotneteers.net/blogs/divedeeper/archive/2008/09/23/LearnVSXNowPart33.aspx)
*   [LearnVSXNow #32: VSXtra at DevCon - Part 1](http://dotneteers.net/blogs/divedeeper/archive/2008/09/18/LearnVSXNowPart32.aspx)
*   [LearnVSXNow #31: Merging Package Menus with VSCT](http://dotneteers.net/blogs/divedeeper/archive/2008/09/06/LearnVSXNowPart31.aspx)
*   [MPFProj Tales #1: Refactoring MPFProj into a library](http://dotneteers.net/blogs/divedeeper/archive/2008/09/05/RefactoringMPFProjPart1.aspx)
*   [Managed Project System with VSXtra](http://dotneteers.net/blogs/divedeeper/archive/2008/09/03/MPSVSXtra.aspx)
*   [LearnVSXNow! #30: Custom Editors in VSXtra](http://dotneteers.net/blogs/divedeeper/archive/2008/09/01/LearnVSXNowPart30.aspx)
*   [LearnVSXNow! #29: VSXtraCommands Part 2 — Commands removing recent items](http://dotneteers.net/blogs/divedeeper/archive/2008/08/06/LearnVSXNowPart29.aspx)
*   [LearnVSXNow! #28: VSXtraCommands Part 1 — Command handling patterns](http://dotneteers.net/blogs/divedeeper/archive/2008/08/01/LearnVSXNowPart28.aspx)
*   [LearnVSXNow! #27: Multiple Tool Windows](http://dotneteers.net/blogs/divedeeper/archive/2008/07/25/LearnVSXNowPart27.aspx)
*   [LearnVSXNow! #26: Services — with no-code service initialization](http://dotneteers.net/blogs/divedeeper/archive/2008/07/23/LearnVSXNowPart26.aspx)
*   [LearnVSXNow! #25: Advanced VSCT Concepts: Behind Combos](http://www.architekturaforum.hu/blogs/divedeeper/archive/2008/07/14/LearnVSXNowPart25.aspx)
*   [LearnVSXNow! #24: Introducing VSXtra](http://www.architekturaforum.hu/blogs/divedeeper/archive/2008/07/08/LearnVSXNowPart24.aspx)

### Project description

This project is a community project to create an improved Managed Package Framework for VS SDK 2008\. The main objectives of the project are:

*   Making it easier for .NET developers to start VSX development
*   Creating a layer above the VS SDK COM interop types to provide real objects to be used as easily as the .NET types of the BCL
*   Leveraging on the state-of-the-art features and tools of the .NET framework
*   Reducing the number of code lines required for basic VSX tasks with about 70%
*   Making the source code of VSPackages much more readable, straightforward and consistent
