using System;
using EnvDTE;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VsSDK.UnitTestLibrary;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
using Microsoft.VisualStudio.Shell.Interop;

namespace VSXtra.Windows.Test
{


    /// <summary>
    ///This is a test class for SiteManagerTest and is intended
    ///to contain all SiteManagerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SiteManagerTest
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
        ///A test for HasGlobalServiceProvider
        ///</summary>
        [TestMethod()]
        public void HasGlobalServiceProviderTest()
        {
            using (OleServiceProvider provider = OleServiceProvider.CreateOleServiceProviderWithBasicServices())
            {
                bool actual;
                var sp = provider as IOleServiceProvider;
                SiteManager.SuggestGlobalServiceProvider(sp);

                actual = SiteManager.HasGlobalServiceProvider;
                if (!actual)
                    Assert.Inconclusive("Verify the correctness of this test method.");
            }
        }

        /// <summary>
        ///A test for GlobalServiceProvider
        ///</summary>
        [TestMethod()]
        public void GlobalServiceProviderTest()
        {
            using (OleServiceProvider provider = OleServiceProvider.CreateOleServiceProviderWithBasicServices())
            {
                SiteManager.SuggestGlobalServiceProvider(provider as IOleServiceProvider);

                System.IServiceProvider actual;
                actual = SiteManager.GlobalServiceProvider;
                if (actual == null)
                    Assert.Inconclusive("Verify the correctness of this test method.");
            }
        }

        /// <summary>
        ///A test for SuggestGlobalServiceProvider
        ///</summary>
        //[TestMethod()]
        // This method can only be tested when DTE is ready
        public void SuggestGlobalServiceProviderTestDTE()
        {
            using (OleServiceProvider provider = OleServiceProvider.CreateOleServiceProviderWithBasicServices())
            {
                DTE dte = provider.GetService<SDTE, DTE>();
                SiteManager.SuggestGlobalServiceProvider(dte);
                if (!SiteManager.HasGlobalServiceProvider)
                    Assert.Inconclusive("A method that does not return a value cannot be verified.");
            }
        }

        /// <summary>
        ///A test for SuggestGlobalServiceProvider
        ///</summary>
        [TestMethod()]
        public void SuggestGlobalServiceProviderTestSystem()
        {
            using (OleServiceProvider provider = OleServiceProvider.CreateOleServiceProviderWithBasicServices())
            {
                IServiceProvider serviceProvider = provider as IServiceProvider; // TODO: Initialize to an appropriate value
                SiteManager.SuggestGlobalServiceProvider(serviceProvider);
                if (!SiteManager.HasGlobalServiceProvider)
                    Assert.Inconclusive("A method that does not return a value cannot be verified.");
            }
        }

        /// <summary>
        ///A test for SuggestGlobalServiceProvider
        ///</summary>
        [TestMethod()]
        public void SuggestGlobalServiceProviderTestOle()
        {
            using (OleServiceProvider provider = OleServiceProvider.CreateOleServiceProviderWithBasicServices())
            {
                IOleServiceProvider serviceProvider = provider as IOleServiceProvider; // TODO: Initialize to an appropriate value
                SiteManager.SuggestGlobalServiceProvider(serviceProvider);
                if (!SiteManager.HasGlobalServiceProvider)
                    Assert.Inconclusive("A method that does not return a value cannot be verified.");
            }
        }
    }
}
