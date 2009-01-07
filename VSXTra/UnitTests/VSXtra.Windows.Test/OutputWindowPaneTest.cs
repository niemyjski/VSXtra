using VSXtra.Windows;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VsSDK.UnitTestLibrary;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace VSXtra.Windows.Test
{
    /// <summary>
    ///This is a test class for OutputWindowPaneTest and is intended
    ///to contain all OutputWindowPaneTest Unit Tests
    ///</summary>
    [TestClass()]
    public class OutputWindowPaneTest
    {
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        /// <summary>
        ///A test for Write with function of "Output a message with the associated file name and position of the file"
        ///</summary>
        [TestMethod()]
        public void WriteTest()
        {
            OleServiceProvider provider = OleServiceProvider.CreateOleServiceProviderWithBasicServices();
            VSXtra.SiteManager.SuggestGlobalServiceProvider(provider as IOleServiceProvider);
            var pane = OutputWindow.General;

            string path = "C:\test.txt";
            int line = 2;
            int column = 0;
            string message = "test message"; // TODO: Initialize to an appropriate value
            pane.Write(path, line, column, message);
        }
    }
}
