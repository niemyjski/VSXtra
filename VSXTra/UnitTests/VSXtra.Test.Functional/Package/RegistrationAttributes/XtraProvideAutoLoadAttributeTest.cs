using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VSXtra.Package;

namespace VSXtra.Test.Functional.Package.RegistrationAttributes
{
  [TestClass]
  public class XtraProvideAutoLoadAttributeTest
  {
    [TestMethod]
    public void UIContextGuidIsAccepted()
    {
      var attr = new XtraProvideAutoLoadAttribute(typeof (UIContext.NoSolution));
      Assert.AreEqual(attr.LoadGuid, typeof(UIContext.NoSolution).GUID);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void WrongUIContextTypeIsRefused1()
    {
      new XtraProvideAutoLoadAttribute(typeof(WindowKind.CallStack));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void WrongUIContextTypeIsRefused2()
    {
      new XtraProvideAutoLoadAttribute(typeof(int));
    }
  }
}