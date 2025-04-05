using System;
using System.Windows.Input;

public class RelayCommand : ICommand
{
    private readonly Action<object> _execute;
    private readonly Func<bool> _canExecute;
    private event EventHandler _canExecuteChanged;
    public RelayCommand(Action<object> execute, Func<bool> canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public bool CanExecute(object parameter) => _canExecute == null || _canExecute();
    public void Execute(object parameter) => _execute(parameter);


    public event EventHandler CanExecuteChanged
    {
        add
        {
            CommandManager.RequerySuggested += value;
            _canExecuteChanged += value;
        }
        remove
        {
            CommandManager.RequerySuggested -= value;
            _canExecuteChanged -= value;
        }
    }

    public void RaiseCanExecuteChanged()
    {
        _canExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}