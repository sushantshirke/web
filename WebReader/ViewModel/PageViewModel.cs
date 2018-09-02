using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WebReader.Model;

namespace WebReader.ViewModel
{
  public  class PageViewModel : INotifyPropertyChanged
    {
        PageModel objModel;
        public PageViewModel()
        {

            objModel = new PageModel();
           
        }

        public ObservableCollection<OptContracts> OptionContracts
        {
            get { return OptionContracts; }
            set
            {
                OptionContracts = value;
                NotifyPropertyChanged("OptionContracts");
            }


        }


        #region Interface

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private ICommand windowLoaded;
        public ICommand WindowLoaded
        {
            get
            {
                if(windowLoaded!=null)
                {
                    windowLoaded = new DelegateCommand(WinddowlodedCmd, true);
                }
                return windowLoaded;
            }
            set
            {
                windowLoaded = value;
            }
        }

        private void WinddowlodedCmd(object obj)
        {

        }



        #endregion


    }
}
