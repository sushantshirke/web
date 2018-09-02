using mshtml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WebReader.DataAccessLayer
{
   internal class DbContext
    {
        public DbContext()
        {

        }

        private List<string> GetContractsAndDate(string strContracts)
        {
            List<string> lstData = new List<string>();

            try
            {
                if (strContracts != string.Empty)
                {
                    string[] SepData = strContracts.Split(new string[] { "</select>" }, StringSplitOptions.RemoveEmptyEntries);

                    Func<string, List<string>> getData = str =>
                    {
                        List<string> lst = new List<string>();

                        string[] data = str.Split(new string[] { "</option>" }, StringSplitOptions.RemoveEmptyEntries);

                        foreach (string value in data)
                        {
                            int lastIndex = 0;
                            lastIndex = value.LastIndexOf('>');

                            if (lastIndex > 0)
                            {
                                lst.Add(value.Substring(lastIndex + 1));
                            }
                        }

                        return lst;

                    };

                    lstData = getData(SepData[1]);
                }
            }
            catch (Exception ex)
            {
                Erorr = ex.StackTrace;
            }

            return lstData;


        }

        public string Erorr { get; set; }


        public string PriceTime { get; set; }

        public string GetUrlData(string url)
        {
            Erorr = string.Empty;
            string htmlCode = string.Empty;
            try
            {
                WebClient client = new WebClient();
                htmlCode = client.DownloadString(url);
            }
            catch (Exception ex)
            {
                Erorr = ex.Message;
            }
            return htmlCode;
        }


        public List<string >GetExpiryDatas(string url)
        {
            string htmlCode = string.Empty;
           htmlCode = GetUrlData(url);


            Erorr = string.Empty;
            List<string> dates = new List<string>();
            try
            {
                string a = htmlCode.Substring(htmlCode.IndexOf("<table"), htmlCode.LastIndexOf("</table") - htmlCode.IndexOf("<table"));

                string[] seperator = new string[] { "</table" };

                string[] tableArray = a.Split(seperator, StringSplitOptions.RemoveEmptyEntries); // read array of 2

                dates =  GetContractsAndDate(tableArray[1]); //Dates array 1

            }
            catch (Exception ex)
            {
                Erorr = ex.Message;
            }
            return dates;
        }


        public DataTable GetPageTableData(string url)
        {
            Erorr = string.Empty;
            DataTable dataTable = new DataTable();
            try
            {
                string htmlCode = GetUrlData(url);



                string a = htmlCode.Substring(htmlCode.IndexOf("<table"), htmlCode.LastIndexOf("</table") - htmlCode.IndexOf("<table"));

                string[] seperator = new string[] { "</table" };


                string[] tableArray = a.Split(seperator, StringSplitOptions.RemoveEmptyEntries); // read array of 2

                CurrentPrice(tableArray[0]);




                string[] tableRows = tableArray[2].Split(new string[] { "</tr" }, StringSplitOptions.RemoveEmptyEntries); //array of 1 for header

                string[] tableHeader = tableRows[1].Split(new string[] { "title" }, StringSplitOptions.RemoveEmptyEntries);

                string columPrefix = "Call_";

                // foreach (string tablcolumn in tableHeader)
                {

                    for (int i = 1; i < tableHeader.Length; i++)


                    //if (!tablcolumn.Contains("<!--"))
                    {
                        string tablcolumn = string.Empty;
                        tablcolumn = tableHeader[i];

                        string columnName = tablcolumn.Substring(2, tablcolumn.IndexOf(">") - 3).Replace(" ", "");

                        if (columnName.Contains("StrikePrice"))
                        {
                            columPrefix = "";
                        }
                        DataColumn dataColumn = new DataColumn(columPrefix + columnName, typeof(double));

                        dataTable.Columns.Add(dataColumn);

                        if (columnName.Contains("StrikePrice"))
                        {
                            columPrefix = "Put_";
                        }
                    }
                }


                for (int i = 2; i < tableRows.Length; i++)
                {
                    string[] data = tableRows[i].Split(new string[] { "</td" }, StringSplitOptions.RemoveEmptyEntries);

                    DataRow dr = dataTable.NewRow();


                    if (data[0].Contains("Total"))
                    {

                        Func<string, string> getTotal = str =>
                        {

                            string[] totalarr = str.Split(new string[] { "</b" }, StringSplitOptions.RemoveEmptyEntries);
                            return totalarr[0].Substring(totalarr[0].LastIndexOf(">") + 1);
                        };

                        double callUI = 0;
                        double callOIchange = 0;
                        double putOI = 0;
                        double putOIcahnge = 0;


                        double.TryParse(getTotal(data[1]), out callUI);  //call OI Total
                        dr[1] = callUI;

                        double.TryParse(getTotal(data[3]), out callOIchange); //call change in oi total
                        dr[3] = callOIchange;

                        double.TryParse(getTotal(data[5]), out putOI);
                        dr[19] = putOI;

                        double.TryParse(getTotal(data[7]), out putOIcahnge);
                        dr[21] = putOIcahnge;

                        dataTable.Rows.Add(dr);


                        break; ;
                    }


                    int columnIndex = 0;
                    int start = 2;// TODO
                    if(data.Length ==27)
                    {
                        start = 1;
                    }
                    else
                    {
                        start = 2;
                    }
                        
                    for (int j = start; j < dataTable.Columns.Count + 1; j++)
                    {
                        string columnWiseData = string.Empty;

                        //Last row total

                        
                        if (data[j].Contains("href"))
                        {
                            if (data[j].Contains("</b>"))
                            {
                                string[] insideAnchor = data[j].Split(new string[] { "</b" }, StringSplitOptions.RemoveEmptyEntries);
                                columnWiseData = insideAnchor[0].Substring(insideAnchor[0].LastIndexOf(">") + 1);
                            }
                            else
                            {
                                string[] insideAnchor = data[j].Split(new string[] { "</a" }, StringSplitOptions.RemoveEmptyEntries);
                                columnWiseData = insideAnchor[0].Substring(insideAnchor[0].LastIndexOf(">") + 1);
                            }


                            //columnWiseData = data[j].Substring(data[j].LastIndexOf(">") + 1);
                        }
                        else
                        {

                            columnWiseData = data[j].Substring(data[j].LastIndexOf(">") + 1);
                        }

                        double intcolumnWiseData = 0;
                        double.TryParse(columnWiseData, out intcolumnWiseData);
                        dr[columnIndex++] = intcolumnWiseData;
                    }
                    dataTable.Rows.Add(dr);

                }

            }
            catch (Exception ex)
            {
                Erorr = ex.Message;
            }

            return dataTable;
        }

        public string DataTableToCSV(DataTable datatable, char seperator = ',')
        {
            StringBuilder sb = new StringBuilder();
            Erorr = string.Empty;
            try
            {
                
                for (int i = 0; i < datatable.Columns.Count; i++)
                {
                    sb.Append(datatable.Columns[i]);
                    if (i < datatable.Columns.Count - 1)
                        sb.Append(seperator);
                }
                sb.AppendLine();
                foreach (DataRow dr in datatable.Rows)
                {
                    for (int i = 0; i < datatable.Columns.Count; i++)
                    {
                        sb.Append(dr[i].ToString());

                        if (i < datatable.Columns.Count - 1)
                            sb.Append(seperator);
                    }
                    sb.AppendLine();
                }
                

            }
            catch (Exception ex)
            {
                Erorr = ex.Message; 
            }

            return sb.ToString();
        }

        private void CurrentPrice(string html)
        {
            Erorr = string.Empty;
            try
            {
                string[] data = html.Split(new string[] { "<span>" }, StringSplitOptions.RemoveEmptyEntries);

                var a = data[0].Split(new string[] { "Underlying","</b>" }, StringSplitOptions.RemoveEmptyEntries);

                var price = a[1].Substring(a[1].LastIndexOf('>')+1);

                var time = data[1].Substring(0,data[1].IndexOf('<') - 1);

                PriceTime = price + "    " + time;
            }
            catch (Exception ex)
            {
                Erorr = ex.Message;
                
            }
            
        }




    }
}
