using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WebReader.DataAccessLayer;

namespace WebReader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DbContext objDbContext;
        public MainWindow()
        {
            InitializeComponent();
            objDbContext = new DbContext();
        }

        DataTable dataTable = new DataTable();
        string SelectedDate = string.Empty;

        public List<string> lstExpirayDate { get; set; }

        public string baseURL = "https://www.nseindia.com/live_market/dynaContent/live_watch/option_chain/optionKeys.jsp?segmentLink=17&instrument=OPTIDX&symbol=BANKNIFTY&date=";

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Action a = ()=>{

                    string url = baseURL+ "5Jul2018";
                    lstExpirayDate = objDbContext.GetExpiryDatas(url);

                    if (objDbContext.Erorr != string.Empty)
                    {
                        MessageBox.Show(objDbContext.Erorr);
                        return;
                    }

                    cboExpirayDate.ItemsSource = lstExpirayDate;
                    cboExpirayDate.SelectedIndex = 1;
                };

                Dispatcher.BeginInvoke(a);
                

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void cboExpirayDate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                SelectedDate = string.Empty;
                SelectedDate = Convert.ToString(cboExpirayDate.SelectedValue);

                dataTable = objDbContext.GetPageTableData(baseURL + SelectedDate);

                if(objDbContext.Erorr !=string.Empty)
                {
                    MessageBox.Show(objDbContext.Erorr);
                    return; 
                }

                grdOI.DataContext = dataTable.DefaultView;

                lblPriceTime.Content = objDbContext.PriceTime;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }

        }

        private void Export_click(object sender, RoutedEventArgs e)
        {
            try
            {
                string sbFile = string.Empty;
                sbFile = objDbContext.DataTableToCSV(dataTable, ',');

                string appPath = Environment.CurrentDirectory + @"\" + SelectedDate + ".csv";

                System.IO.File.WriteAllText(appPath, sbFile);

                MessageBox.Show("File Exported to :" + appPath);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.StackTrace);
            }


        }
    }
}
