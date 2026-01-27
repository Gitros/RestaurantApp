using System.ComponentModel;

namespace RestaurantApp.ViewModels;

public abstract class ValidatableViewModel : WorkspaceViewModel, IDataErrorInfo
{
    private bool _showErrors;
    public bool ShowErrors
    {
        get => _showErrors;
        set
        {
            if (_showErrors == value) return;
            _showErrors = value;
            OnPropertyChanged(() => ShowErrors);
        }
    }

    public abstract string this[string columnName] { get; }

    public string Error => "";

    public abstract bool IsValid { get; }
}
