using Automation.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automation.Core.Containers
{
    public abstract class BaseFrame : BaseContainer
    {
        public string FrameName { get; set; }
        public int FrameIndex { get; set; }

        public BaseFrame(WebRunner runner, string frameName = WebRunner.DEFAULT_FRAME,IBaseContainer parent = null)
            : base(runner)
        {
            Parent = parent;
            if (string.IsNullOrEmpty(frameName))
                throw new ArgumentNullException("frameName");
            FrameName = frameName;
            FrameIndex = -1;
            Factory.HtmlControlFactory.GenerateProperties(this);
        }

        public BaseFrame(WebRunner runner, int findex,IBaseContainer parent = null)
            : base(runner)
        {
            Parent = parent;
            FrameIndex = findex;
            FrameName = string.Empty;
            Factory.HtmlControlFactory.GenerateProperties(this);
        }

        public override void Focus()
        {// if there is no frame name or index define
            if ((!string.IsNullOrEmpty(FrameName)) || FrameIndex != -1) 
            {
                // if that frame has frame parent and FrameIndex was not set and equal to -1 - only FrameName should define
                if ((Parent != null) && ((Parent) is BaseFrame) && (FrameIndex == -1))
                {
                    try
                    {
                        WebRunner.m_driver.SwitchTo().DefaultContent();
                    }
                    catch (Exception)
                    {

                    }
                    if (!string.IsNullOrEmpty((Parent as BaseFrame).FrameName))
                        WebRunner.m_driver.SwitchToFrameName((Parent as BaseFrame).FrameName);
                    else
                        WebRunner.m_driver.SwitchToFrameIndex((Parent as BaseFrame).FrameIndex);

                    WebRunner.m_driver.SwitchToFrameName(FrameName);
                }
                //if that frame has frame parent and FrameIndex was set with val 1 or 2 , then first switch to parent frame and after that switch to frame child
                if ((Parent != null) && ((Parent) is BaseFrame) && (FrameIndex != -1))
                {
                    WebRunner.m_driver.SwitchTo().DefaultContent();
                    if (!string.IsNullOrEmpty((Parent as BaseFrame).FrameName))
                        WebRunner.m_driver.SwitchToFrameName((Parent as BaseFrame).FrameName);
                    else
                        WebRunner.m_driver.SwitchToFrameIndex((Parent as BaseFrame).FrameIndex);

                    WebRunner.m_driver.SwitchToFrameIndex(FrameIndex);
                }
                else
                { // if no Parent but FrameIndex or FrameName was define
                    if (FrameIndex != -1)
                    {
                        WebRunner.m_driver.SwitchTo().DefaultContent();
                        WebRunner.m_driver.SwitchTo().Frame(FrameIndex);
                    }
                    else
                    {
                        WebRunner.m_driver.SwitchTo().DefaultContent();
                        WebRunner.m_driver.SwitchTo().Frame(FrameName);
                      //  WebRunner.CurrentFrame = FrameName;
                    }
                }
            }
            
        }

        public override void WaitToLoad()
        {
            WebRunner.WaitForPageToLoad();
        }

        public override void WaitToLoadAjax()
        {

            WebRunner.WaitForPageToLoadAjax();
        }
    }
}
