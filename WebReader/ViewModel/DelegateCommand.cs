using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WebReader.ViewModel
{
    public class DelegateCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private Action<object> execute;
        private bool canExecute;

        public DelegateCommand(Action<object>_execute, bool _canexecute)
        {
            this.execute = _execute;
            this.canExecute = _canexecute;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if(CanExecuteChanged!=null)
            {
                execute(parameter);
            }
        }
    }
}
