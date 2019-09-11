using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Automation.Core.Infrastructure;
using Pages.PagesPO;

namespace Tests_example
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestExample()
        {
            WebRunner webRunner;
            webRunner = new WebRunner(TestProfile.IEProfile);
            LoginPage loginPage = new LoginPage(webRunner);
            loginPage.UserName.Text = "User1";
            loginPage.Pass.Text = "MyPassword";
            loginPage.LoginBtn.Click();
            //Assert.IsTrue("TBD");
            webRunner.Close();

        }
    }
}
