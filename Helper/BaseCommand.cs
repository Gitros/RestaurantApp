using System;
using System.Windows.Input;

namespace RestaurantApp.Helper
{
    internal class BaseCommand : ICommand
    {
        private readonly Action _command;
        private readonly Func<bool>? _canExecute;

        public BaseCommand(Action command, Func<bool>? canExecute = null)
        {
            _command = command ?? throw new ArgumentNullException(nameof(command));
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter)
        {
            return _canExecute?.Invoke() ?? true;
        }

        public void Execute(object? parameter)
        {
            _command();
        }

        public event EventHandler? CanExecuteChanged;

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}