using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ResizeMe.Command
{
    public class ActionCommand : ICommand
    {
        private Action _executeAction;
        private Func<bool> _canExecute;
        public ActionCommand(Action executeAction, Func<bool> canExecuteAction)
        {
            _executeAction = executeAction;
            _canExecute = canExecuteAction;
        }


        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public ActionCommand(Action executeAction)
            : this(executeAction, () => true)
        {
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute();
        }

        public void Execute(object parameter)
        {
            _executeAction();
        }

        protected void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }

    }
}
