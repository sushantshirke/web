using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebReader.Model
{
   public class PageModel
    {

        public PageModel()
        {
            DicContracts = new ObservableCollection<OptContracts>();
            //DicContracts.Add(10003, "NIFTY");
            //DicContracts.Add(-9999, "BANKNIFTY");
        }

        public ObservableCollection<OptContracts>DicContracts
        {
            get;set;
        }


        public ObservableCollection<string> ExpiryDates { get; set; }

    }

    public class OptContracts
    {
        int key { get; set; }
        string value { get; set; }
    }

   


}
