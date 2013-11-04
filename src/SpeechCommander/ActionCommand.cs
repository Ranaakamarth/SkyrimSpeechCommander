using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SpeechCommander
{
    public class ActionCommand : ICommand
    {
        private readonly Action execute;

        private readonly Action<object> executeParam;

        private readonly Func<bool> canExecute;

        public ActionCommand(Action executeAction, Func<bool> canExecuteFunc)
        {
            this.execute = executeAction;
            this.canExecute = canExecuteFunc;
        }

        public ActionCommand(Action<object> executeAction, Func<bool> canExecuteFunc)
        {
            this.executeParam = executeAction;
            this.canExecute = canExecuteFunc;
        }

        bool ICommand.CanExecute(object parameter)
        {
            if (this.canExecute != null)
            {
                return this.canExecute();
            }
            return true;
        }

        event EventHandler ICommand.CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

         void ICommand.Execute(object parameter)
        {
            if (this.execute != null)
            {
                this.execute();
            }
            else if (this.executeParam != null)
            {
                this.executeParam(parameter);
            }
        }
    }
}
