using Automation.Core.Controls;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Automation.Core.Infrastructure;
using Automation.Core;
using Automation.Core.Attributes;
using Automation.Core.Containers;

namespace Pages.PagesPO
{
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
