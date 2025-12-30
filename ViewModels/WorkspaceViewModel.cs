using RestaurantApp.Helper;
using System.Windows.Input;

namespace RestaurantApp.ViewModels;

public abstract class WorkspaceViewModel : BaseViewModel
{
    private BaseCommand? _closeCommand;

    private string _displayName = "";
    public string DisplayName
    {
        get => _displayName;
        set { _displayName = value; OnPropertyChanged(() => DisplayName); }
    }

    public virtual bool CanClose => true;

    public ICommand CloseCommand =>
        _closeCommand ??= new BaseCommand(() => OnRequestClose(), () => CanClose);

    public event EventHandler? RequestClose;

    protected void OnRequestClose()
        => RequestClose?.Invoke(this, EventArgs.Empty);
}
