using System.ComponentModel;

namespace RestaurantApp.ViewModels;

public abstract class ValidatableViewModel : WorkspaceViewModel, IDataErrorInfo
{
    public virtual string Error => string.Empty;

    // WPF pyta o błąd konkretnej właściwości
    public abstract string this[string columnName] { get; }

    // Czy cały formularz jest poprawny? (użyjemy w CanExecute)
    public virtual bool IsValid => true;
}
