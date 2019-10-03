# HtmlControlPageObject
The project is enhancement for C# developer who are using Selenium for create tests on Web and Mobile, 
The project make your code to be more simplify , all you need to do is to fetch that solution and start define page object class with the very simple coding , define controls in page which give you the robust infrastructure based on Selenuim function and other JS tricks

first steps is to create two project aside to the core project:
the first porject - is the pages classes
the second project - is the tests classes




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

        
        public LoginPage(WebRunner runner)
            : base(runner)
        {
            m_webRunner = runner;
            m_webRunner.WaitForPageToLoad();
        }


    }
}

// --------------------------------------

And this is the test itself 


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







