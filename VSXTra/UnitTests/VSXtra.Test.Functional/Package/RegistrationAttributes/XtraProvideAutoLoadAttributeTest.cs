using System;
using NUnit.Framework;
using VSXtra.Package;

namespace VSXtra.Test.Functional.Package.RegistrationAttributes
{
  [TestFixture]
  public class XtraProvideAutoLoadAttributeTest
  {
    [Test]
    public void UIContextGuidIsAccepted()
    {
      var attr = new XtraProvideAutoLoadAttribute(typeof (UIContext.NoSolution));
      Assert.AreEqual(attr.LoadGuid, typeof(UIContext.NoSolution).GUID);
    }

    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void WrongUIContextTypeIsRefused1()
    {
      new XtraProvideAutoLoadAttribute(typeof(WindowKind.CallStack));
    }

    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void WrongUIContextTypeIsRefused2()
    {
      new XtraProvideAutoLoadAttribute(typeof(int));
    }
  }
}