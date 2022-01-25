using System;
using System.Windows.Input;

namespace WPFCalc.ViewModels
{
    public class RelayCommand : ICommand
    {

        private readonly Action<object> execute;
        private readonly Func<object, bool> canExecute;

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public RelayCommand(Action<object> exec, Func<object, bool> canExec)
        {
            execute = exec ?? throw new ArgumentException(nameof(execute));
            canExecute = canExec;
        }

        public bool CanExecute(object parameter) => canExecute?.Invoke(parameter) ?? true;

        public void Execute(object parameter) => execute(parameter);
    }
}