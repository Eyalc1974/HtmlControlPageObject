# HtmlControlPageObject
The project is enhancement for C# developer who are using Selenium for create tests on Web and Mobile, 
The project make your code to be more simplify , all you need to do is to fetch that solution and start define page object class with the very simple coding , define controls in page which give you the robust infrastructure based on Selenuim function and other JS tricks

in addition to that project you should have the testing solution , see below an example how to create page object class with that current solution 
and also the testing (test class) phase,


see Page login as example of your pages >> using attributes and reflictions

 public class LoginPage : BasePage
    {
        [HtmlControl("txtUsername", SearchBy.Id)]
        public HtmlControlTextBox UserName { get; set; }

        [HtmlControl("txtPassword", SearchBy.Id)]
        public HtmlControlTextBox Pass { get; set; }

        [HtmlControl("btnLogin", SearchBy.Id)]
        public HtmlControlButton LoginBtn { get; set; }

        private WebRunner m_webRunner;
        public static string PageUrl = string.Empty;

        /// <summary>
        /// pass the test runner only when the current test contains more than one instance of browser and you need more than 1 selenium driver
        /// </summary>
        /// <param name="runner"></param>

        public LoginPage(WebRunner runner)
            : base(runner)
        {
            m_webRunner = runner;
            m_webRunner.WaitForPageToLoad();
        }


    }
}

// --------------------------------------

Adn this is the test itself 

 [TestMethod]
        public void TestMethod1()
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
