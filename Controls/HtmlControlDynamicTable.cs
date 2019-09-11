using Automation.Core.Attributes;
using Automation.Core.Containers;
using Automation.Core.Infrastructure;
using Automation.Core.Logging;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Automation.Core.Controls
{
    public class HtmlControlDynamicTable : HtmlControlBase
    {        
        public string this[int row, int col]
        {
            get
            {
                WaitForPageContainerToLoad();
                Refresh();
                return GetSubWebElement(row, col).Text;
            }          
        }
        /// <summary>
        /// row is starting from 1 
        /// </summary>
        /// <param name="colname"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        public string this[string colname,int row]
        {
            get
            {
                WaitForPageContainerToLoad();
                Refresh();
              
                return GetSubWebElement(row, GetColumnIndexByName(colname)).Text;
            }
        }
        private string temporaryCssSelector = string.Empty;
      
        public int TableTotalRows
        {
            get
            {
                Log.WriteMessage("HtmlControlDynamicTable: TableTotalRows, ", true);
                WaitForPageContainerToLoad();
                if (!ControlFound)
                    Refresh();
                if(HtmlControlAttribute.SearchBy == SearchBy.ClassName)
                return _baseElement.FindElements(By.CssSelector(HtmlControlAttribute.DomAddressLocator + " tr")).Count;
                else
                    return _baseElement.FindElements(By.TagName("tr")).Count;
            }
        }

        public int TableTotalColumns
        {
            get
            {
                Log.WriteMessage("HtmlControlDynamicTable: TableTotalColumns, ", true);
                WaitForPageContainerToLoad();
                if (!ControlFound)
                    Refresh();
                if (HtmlControlAttribute.SearchBy == SearchBy.ClassName)
                {
                    IList<IWebElement> elems = GetHeadersElements();
                    return elems.Count;
                }
                else
                {
                    int totalcol = _baseElement.FindElements(By.TagName("th")).Count;
                    Log.WriteMessage("HtmlControlDynamicTable: TableTotalColumns, number of col is :, " + totalcol, true);
                    return totalcol;
                }
            }
        }

        private int GetColumnIndexByName(string colname)
        {
            Log.WriteMessage("HtmlControlDynamicTable: GetColumnIndexByName, ", true);
            IList<IWebElement> elems = GetHeadersElements();
            int counter = 1;
            foreach (var item in elems)
            {
                if (item.Text.Equals(colname)) return counter;
                counter++;
            }
            throw new Exception("The column name: " + colname + " was not found");
        }
        
        public IList<string> TableHeaders
        {
            get
            {
                Log.WriteMessage("HtmlControlDynamicTable: TableHeaders, ", true);
                WaitForPageContainerToLoad();
                IList<string> headers =new List<string>();
                
                if (!ControlFound)
                    Refresh();
                IList<IWebElement> elems = GetHeadersElements();
                foreach (var item in elems)
                    headers.Add(item.Text);

                Log.WriteMessage("HtmlControlDynamicTable: TableHeaders, elems are:" + headers.ToString(), true);
                return headers;
                
            }
        }

        private IList<IWebElement> GetHeadersElements()
        {
            Log.WriteMessage("HtmlControlDynamicTable: GetHeadersElements, ", true);
            IList<IWebElement> elems = _baseElement.FindElements(By.TagName("th"));
            Log.WriteMessage("HtmlControlDynamicTable: GetHeadersElements, num of elems is:" + elems.Count, true);
            if (elems.Count == 0)
                elems = _baseElement.FindElements(By.CssSelector(HtmlControlAttribute.DomAddressLocator + " tr:nth-child(1)"));
            return elems;
        }

        /// <summary>
        /// Func return string based on the #cell in line# (row.col), 
        /// if the Css has tr tags then  :nth-child(x) is used otherwise TR tages is lookup
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        private IWebElement GetSubWebElement(int row, int col)
        {
            Log.WriteMessage("HtmlControlDynamicTable : GetSubWebElement using nth-child() css selector, stated", true);
            WaitForPageContainerToLoad();
            Refresh();
            IWebElement line = null;
           // IList<IWebElement> elemRows;
            if (col == 0) col++;
            if (row == 0) row++;
            if (GetHeadersElements().Count > 0) row++;

               switch (HtmlControlAttribute.SearchBy)
                {
                    case SearchBy.Id:
                        Refresh();
                        temporaryCssSelector = "#" + HtmlControlAttribute.DomAddressLocator + " tr:nth-child(" + row + ")" + " td:nth-child(" + col + ")";
                        line = _baseElement.FindElement(By.CssSelector("#" + HtmlControlAttribute.DomAddressLocator + " tr:nth-child(" + row + ")" + " td:nth-child(" + col + ")"));
                        return line;
                    case SearchBy.CssSelector:
                        Refresh();
                        temporaryCssSelector = HtmlControlAttribute.DomAddressLocator + " tr:nth-child(" + row + ")" + " td:nth-child(" + col + ")";
                        line = _baseElement.FindElement(By.CssSelector(HtmlControlAttribute.DomAddressLocator + " tr:nth-child(" + row + ")" + " td:nth-child(" + col + ")"));
                        break;                    
                    default:
                        throw new Exception("you must use CSSSelector or ID for theat smart table");                        
                }
               return line;                                                      
        }

        public HtmlControlDynamicTable(OpenQA.Selenium.IWebElement elem, string displayName, HtmlControlAttribute attribute, IBaseContainer container)
            : base(elem, displayName, attribute, container)
        {
            IsElementInArrayList = false;
        }

        public T GetSubControl<T>(int i, int j) where T : class
        {
            try
            {
                Log.WriteMessage("HtmlControlDynamicTable : GetSubControl T in table, val [i,j] :" + i + j, true);
                IWebElement subCntrl = GetSubWebElement(i, j);
                Attributes.HtmlControlAttribute html = new HtmlControlAttribute(temporaryCssSelector, HtmlControlAttribute.SearchBy);
                return (T)(object)Factory.HtmlControlFactory.CreateElement(typeof(T), html, Parent);

            }
            catch (NoSuchElementException ex) { Logging.Log.WriteError("failed to return generic control",ex); }
            return default(T);
        }              

    }
}
