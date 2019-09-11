using Automation.Core.Infrastructure;
using Automation.Core.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Automation.Core.Attributes;
using System.Threading;
using Automation.Core.Containers;


namespace Automation.Core.Controls
{
    public class HtmlControlTable : HtmlControlBase
    {

        private IWebElement m_GridElement = null;
        private List<List<String>> m_MatrixResults;
        private List<String> m_MatrixHeader;
        private List<string> rowDataInGrid;
        private int m_totalColInTable;
        private int m_totalRowsInTable;
        private int titleIndex;
        HtmlControlAttribute m_attribute = null;
        bool m_IsfirstTimeInit = true;
        private HtmlTableAttribute m_atrrTable;
        /// <summary>
        /// cell data table;
        /// It filled once
        /// </summary>
        public List<List<String>> TableDataSnapshotResults
        {
            get
            {
                if (m_MatrixResults != null)
                    return m_MatrixResults;
                else
                {
                    TableInit();
                    return m_MatrixResults;
                }
            }

        }
        public int TableTotalRows
        {
            get
            {
                WaitForPageContainerToLoad();
                if (!(ControlFound && m_MatrixResults != null && m_attribute.RefreshMethod != RefreshMethod.Always))
                    TableInit();
                
                Logging.Log.WriteMessage("HtmlControlTable: TableTotalRows() prop, num of row is :" + m_totalRowsInTable, true);
                return m_totalRowsInTable;
            }
        }



        public int TableTotalColumns
        {
            get
            {
                WaitForPageContainerToLoad();
                if (!(ControlFound && m_MatrixResults != null && m_attribute.RefreshMethod != RefreshMethod.Always))
                    TableInit();

                Logging.Log.WriteMessage("HtmlControlTable: TableTotalColumns() prop, num of row is :" + m_totalColInTable, true);
                return m_totalColInTable;
            }
        }

        public List<String> TableHeaders
        {
            get
            {

                WaitForPageContainerToLoad();
                Thread.Sleep(5000);
                if (!(ControlFound && m_MatrixResults != null && m_attribute.RefreshMethod != RefreshMethod.Always))
                    TableInit();
                if (m_MatrixHeader == null)
                    throw new Exception("Table is not exist!!!");

                Logging.Log.WriteMessage("HtmlControlTable: TableHeaders() prop, num of headers is :" + m_MatrixHeader.Count, true);
                return m_MatrixHeader;
            }
        }

        public string this[int row, int col]
        {
            get
            {
                WaitForPageContainerToLoad();
                if (!(ControlFound && m_MatrixResults != null && m_attribute.RefreshMethod != RefreshMethod.Always))
                {
                    TableInit();
                    Logging.Log.WriteMessage("HtmlControlTable: indexer[] prop, cell in table value is :" + m_MatrixResults[row][col - 1].ToString(), true);
                    return m_MatrixResults[row][col - 1].ToString();
                }
                else return m_MatrixResults[row][col].ToString();
            }
        }

        public int GetIndexByColumnNameAndStringValue(string columValue, string strValue)
        {
            var elInList = GetValuesFromTableRowByColTitle(columValue);
            int numberOfEl = elInList.Count;
            for (int i = 0; i < numberOfEl; i++)
            {
                if (elInList[i].Contains(strValue))
                    return i;
            }

            return -1;

        }


        //}

        /// <summary>
        /// Get list of column values by column index from 0;
        /// </summary>
        /// <param name="columnTitle"></param>
        /// <returns>List of column values</returns>
        public List<String> GetValuesFromColByColIndex(int columnIndex)
        {
            WaitForPageContainerToLoad();
            List<String> columnData = new List<String>();

            if (!(ControlFound && m_MatrixResults != null && m_attribute.RefreshMethod != RefreshMethod.Always))
            {
                TableInit();
                for (int i = 0; i < TableTotalRows; i++)
                {
                    if (this.m_MatrixResults[i][columnIndex].ToString().Length > 11)
                        columnData.Add(this.m_MatrixResults[i][columnIndex].ToString());
                }

                return columnData;
            }
            else return null;
        }

        /// <summary>
        /// Get list of column values by column title
        /// </summary>
        /// <param name="columnTitle"></param>
        /// <returns>list of column values</returns>
        public List<String> GetValuesFromTableRowByColTitle(string columnTitle)
        {
            WaitForPageContainerToLoad();
            List<String> columnData = new List<String>();
            int tableRowsCounter;
            if (!(ControlFound && m_MatrixResults != null && m_attribute.RefreshMethod != RefreshMethod.Always))
            {
                TableInit();
                int colIndex = GetColumnIndex(columnTitle);
                tableRowsCounter = TableTotalRows;
                for (int i = 0; i < tableRowsCounter; i++)
                {
                    columnData.Add(this.m_MatrixResults[i][colIndex].ToString());
                }

                return columnData;
            }
            else
            {
                int colIndex = GetColumnIndex(columnTitle);
                tableRowsCounter = TableTotalRows;
                for (int i = 0; i < tableRowsCounter; i++)
                {
                    columnData.Add(this.m_MatrixResults[i][colIndex].ToString());
                }
                return columnData;
            }

        }


        public HtmlControlTable(IWebElement elem, string displayName, HtmlControlAttribute attribute, IBaseContainer container)
            : base(elem, displayName, attribute, container)
        {
            HtmlTableAttribute atrrTable = attribute as HtmlTableAttribute;
            //ToDo: we cannot init the table automatically only by demand , see playermanagmenet filter
            m_GridElement = elem;
            IsElementInArrayList = false;
            m_attribute = attribute;
            if (atrrTable != null) m_atrrTable = atrrTable;
            if (_baseElement != null)
            {
                TableInit();
                m_IsfirstTimeInit = false;
            }
            else m_IsfirstTimeInit = true;

        }


        private IList<string> GetAndUpdateHeaders()
        {
            IList<IWebElement> allHeaders = null;
            m_MatrixHeader = new List<string>();
            SwitchToControl();
            Refresh();
            // if atrr exist then use the TableHeaderTageName 
            if (m_atrrTable == null)
                allHeaders = _baseElement.FindElements(By.TagName("th"));
            else
                allHeaders = _baseElement.FindElements(By.CssSelector(m_atrrTable.TableHeaderTageName));

            // allHeaders = m_GridElement.FindElements(By.TagName("th"));
            // fetch headers to List >> m_MatrixHeader
            foreach (IWebElement elmnt in allHeaders)
            {
                m_MatrixHeader.Add(elmnt.Text);
            }
            Logging.Log.WriteMessage("HtmlControlTable: GetAndUpdateHeaders() , headers are :" + m_MatrixHeader.ToString(), true);

            return m_MatrixHeader;
        }

        private bool CheckDataInList(IList<string> list)
        {
            int counter = 0;
            for (int i = 0; i < list.Count; i++)
            {
                if ((list[i].Trim().Equals("") || list[i].Equals(string.Empty) || list[i].Equals("")))
                    counter++;
            }
            if (counter == list.Count)
                return false;
            else return true;
        }

        private void TableInit()
        {

            // if control is asked to refresh once and it is not the first time init then exist func
            if (m_attribute != null)
            {
                if ((ControlFound && m_MatrixResults != null && m_attribute.RefreshMethod != RefreshMethod.Always))
                    return;
            }

            try
            {
                Logging.Log.WriteMessage("HtmlControlTable: TableInit() - start point", true);

                // init vars
                int numberofColinGrid;
                int colmnPointerInCurrentTable;
                // Some table are unique with headers or without then the indicator of #col 
                InitRowsAndCols(out numberofColinGrid, out colmnPointerInCurrentTable);
                SwitchToControl();
                Refresh();

                m_MatrixHeader = new List<string>();
                m_MatrixResults = new List<List<string>>();
                IList<IWebElement> cells = null;

                GetAndUpdateHeaders();

                // if there are no headers in table , th
                if (m_MatrixHeader.Count != 0)
                    numberofColinGrid = m_MatrixHeader.Count;
                colmnPointerInCurrentTable = 0;
                // fetch data in table...in cells     


                m_GridElement = _baseElement;
                // if atrr exist ...
                if (m_atrrTable == null)
                    cells = m_GridElement.FindElements(By.TagName("td"));
                else
                    cells = m_GridElement.FindElements(By.CssSelector(m_atrrTable.TableDateTageName));

                rowDataInGrid = new List<string>();

                // foreach (IWebElement cell in cells)
                for (int i = 0; i < cells.Count; i++)
                { // go down to the next line of table
                    rowDataInGrid.Add(cells[i].Text);
                    colmnPointerInCurrentTable++;
                    // add new LIST as second,third rows and so one...
                    if (colmnPointerInCurrentTable == numberofColinGrid)
                    {
                        m_MatrixResults.Add(rowDataInGrid);
                        m_totalRowsInTable++;
                        if (!(CheckDataInList(rowDataInGrid)))
                            m_totalRowsInTable--;

                        colmnPointerInCurrentTable = 0;
                        rowDataInGrid = new List<string>();
                    }
                }

                //if col = 0 then the rowDataInGrid
                if (numberofColinGrid == 0) m_MatrixResults.Add(rowDataInGrid);

                // if there are no headers in table , then take the first raw with no mre 10 col and define it as hd
                if (m_MatrixHeader.Count == 0)
                {
                    for (int i = 0; i < 10; i++)
                        m_MatrixHeader.Add(m_MatrixResults[0][i].ToString());
                }

                m_totalColInTable = m_MatrixHeader.Count;
                System.Threading.Thread.Sleep(3000);
                m_IsfirstTimeInit = false;
            }
            catch (Exception e) { Log.WriteError("failed to create new table with content TableInit()", e); }

            Logging.Log.WriteMessage("HtmlControlTable: TableInit() - ended", true);

        }

        private void InitRowsAndCols(out int numberofColinGrid, out int colmnPointerInCurrentTable)
        {
            numberofColinGrid = 0;
            m_totalRowsInTable = 0;

            colmnPointerInCurrentTable = 0;
        }

        private int GetColumnIndex(string title)
        {

            // find the cloumn index based on title of col
            GetAndUpdateHeaders();
            titleIndex = m_MatrixHeader.IndexOf(title);
            if (titleIndex < 0)
                Log.WriteMessage("The following HEADER Name: " + title + " wasn't found in table at col number : " + m_totalColInTable);
            return titleIndex;
        }

        /// <summary>
        /// Return how many value in col x
        /// </summary>
        /// <param name="headerName"></param>
        /// <param name="keyVar"></param>
        /// <returns></returns>
        public int GetHowManyValueInCol(string headerName, string keyVar, bool result = true)
        {
            TableInit();

            // return the number of instance that repeat itself in specific coloumn
            int counter = 0;

             GetColumnIndex(headerName);
             

            if (result)
                for (int i = 0; i < m_MatrixResults.Count; i++)
                {
                    if (m_MatrixResults[i].Count == 0) break;
                    if (m_MatrixResults[i][titleIndex].Contains(keyVar))
                        counter++;
                }
            else
                for (int i = 0; i < m_MatrixResults.Count; i++)
                {
                    if (Double.Parse(m_MatrixResults[i][titleIndex].ToString().Substring(1)) >= Double.Parse(keyVar))
                        counter++;
                }

            Log.WriteMessage("Table [headername] is" + headerName + " , Value :" + keyVar + " are displated : " + counter);
            return counter;
        }

        public void CheckTableHeadersExistence(string[] parameters)
        {
            TableInit();
            for (int i = 0; i < parameters.Length; i++)
            {
                if ((m_MatrixHeader.IndexOf(parameters[i]) == -1))
                    Assert.Fail("The following col are not in the examine Table, " + parameters[i]);
            }
        }

        public IWebElement GetHeaderName(string name)
        {
            TableInit();

            IList<IWebElement> allHeaders = null;

            allHeaders = _baseElement.FindElements(By.TagName("th"));
            // allHeadersInners = allHeaders[0].FindElements(By.TagName("a"));
            foreach (IWebElement elmnt in allHeaders)
            {
                if (elmnt.Text.Equals(name))
                    return elmnt.FindElement(By.TagName("a"));
            }
            return null;
        }

        public bool ValueInColumn(string col, string val)
        {
            TableInit();
            int index = m_MatrixHeader.IndexOf(col);
            if (index != -1)
            {
                for (int i = 0; i < m_totalRowsInTable; i++)
                {
                    if (m_MatrixResults[i][index].Equals(val))
                        return true;
                }
            }
            return false;
        }


    }
}
