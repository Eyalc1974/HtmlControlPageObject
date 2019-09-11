using NG.Automation.Core.Attributes;
using NG.Automation.Core.Containers;
using NG.Automation.Core.Infrastructure;
using NG.Automation.Core.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NG.Automation.Core.Controls
{
    public class HtmlArrayControl<T> : HtmlControlBase where T : class
    {
        private T[] _arrT;
        PropertyInfo m_pi;
        bool IsFirstTimeInit;
        private Attributes.HtmlControlAttribute _attribute;
        //private BasePage _page;
        //private BaseFrame _frame;        
        Type arrayType;

        public HtmlArrayControl(HtmlControlBase[] arr, PropertyInfo pi, HtmlControlAttribute attribute, IBaseContainer container)
            : base(null, "", attribute, container)
        {
            Log.WriteMessage("HtmlArrayControl: ctor() - init array", true);
            _attribute = attribute;
            m_pi = pi;
            List<T> list = new List<T>();
            
            m_pi = pi;
            IsElementInArrayList = true;

            arrayType = arr.GetType();

            arr.ToList().ForEach(html => list.Add(html as T));
            _arrT = list.ToArray();
            IsFirstTimeInit = true;
            SetSpecificLocatorForChildsInArray();
            
        }

        private void SetSpecificLocatorForChildsInArray()
        {
            //IWebElement  l = null;
            if (!IsFirstTimeInit)
            {
                    try 
	                {	        
		              IWebElement searchEml = Parent.WebRunner.m_driver.WaitForElement(By.CssSelector(HtmlControlAttribute.DomAddressLocator));
                      if (searchEml.Enabled || searchEml.Displayed)
                       {
                         string location = _attribute.DomAddressLocator;
                         IList<IWebElement> elms = Parent.WebRunner.m_driver.FindElements(By.CssSelector(location));
                         if (elms.Count > 0)
                          {
                           if (_arrT.Length != 0)
                             {
                               for (int i = 0; i < elms.Count; i++)
                                 {
                                   (_arrT[i] as HtmlControlBase).IsElementInArrayList = true;
                                   (_arrT[i] as HtmlControlBase)._baseElement = elms[i];
                                 }
                              }
                               else
                                {
                                  HtmlControlBase[] arr = NG.Automation.Core.Factory.HtmlControlFactory.CreateElements(m_pi, Parent);
                                  List<T> list = new List<T>();
                                  arr.ToList().ForEach(html => list.Add(html as T));
                                  _arrT = list.ToArray();
                                  for (int i = 0; i < elms.Count; i++)
                                   {
                                     (_arrT[i] as HtmlControlBase).IsElementInArrayList = true;
                                     (_arrT[i] as HtmlControlBase)._baseElement = elms[i];
                                   }
                                }
                            }
                        }
	                }
	            catch (Exception)
	            {
				   Log.WriteMessage("HtmlArrayControl: lenght of array is 0 ...", true);
   	            }
            }
            else
            {
                if (_arrT.Length > 0)
                {
                    Log.WriteMessage("HtmlArrayControl: lenght of array is :" + _arrT.Length, true);
                    try
                    {
                        string location = (_arrT[0] as HtmlControlBase).HtmlControlAttribute.DomAddressLocator;
                        IList<IWebElement> elms = (_arrT[0] as HtmlControlBase).Parent.WebRunner.m_driver.FindElements(By.CssSelector(location));
                        if (elms.Count > 0)
                        {
                            for (int i = 0; i <= _arrT.Length - 1; i++)
                            {
                                (_arrT[i] as HtmlControlBase).IsElementInArrayList = true;
                                (_arrT[i] as HtmlControlBase)._baseElement = elms[i];
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
                else
                    Log.WriteMessage("HtmlArrayControl: lenght of array is 0 ...", true);
            }
        }

        public T this[int index]
        {
            get
            {
                SwitchToControl();
                if(_attribute.RefreshArrayControl)
                    Refresh();
                if (_arrT.Length > 0)
                {
                    Log.WriteMessage("HtmlArrayControl: indexer[" + index + " ] of array is :" + _arrT[index].ToString(), true);
                    return _arrT[index];
                }
                else
                {                   
                    return null;
                }
            }
        }

        public override void Refresh(bool force = false) //bool force = true
        {           

            if (!force && (_arrT.Length > 0) && 
                //(!HtmlControlAttribute.AjaxCall) && 
                HtmlControlAttribute.RefreshMethod != RefreshMethod.Always &&
                 m_pi.PropertyType.GetGenericArguments()[0].IsSubclassOf(typeof(HtmlGroupControl)))
                return;

            Log.WriteMessage("HtmlArrayControl: Refresh() array ", true);
            List<T> list = new List<T>();
            HtmlControlBase[] array = Factory.HtmlControlFactory.CreateElements(m_pi, Parent);
            array.ToList().ForEach(html => list.Add(html as T));
            _arrT = list.ToArray();
            IsFirstTimeInit = false;            
            SetSpecificLocatorForChildsInArray();

        }

        public int Lenght
        {
            get
            {
                if (_attribute.RefreshArrayControl)
                    Refresh(true);
                Log.WriteMessage("HtmlArrayControl: Lenght prop of array is:" + _arrT.Length, true);
                return _arrT.Length;
            }
        }

    
    }

}
