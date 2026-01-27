using CommunityToolkit.Mvvm.Messaging;
using RestaurantApp.Messages;
using RestaurantApp.Data;
using RestaurantApp.Entities;
using RestaurantApp.Helper;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace RestaurantApp.ViewModels;

public class AddAreaViewModel : ValidatableViewModel
{
    private readonly RestaurantDbContext _context = new();

    public ObservableCollection<Restaurant> Restaurants { get; } = new();

    private Restaurant? _selectedRestaurant;
    public Restaurant? SelectedRestaurant
    {
        get => _selectedRestaurant;
        set
        {
            _selectedRestaurant = value;
            OnPropertyChanged(() => SelectedRestaurant);
            ((BaseCommand)SaveCommand).RaiseCanExecuteChanged();
        }
    }

    public override string this[string columnName]
    {
        get
        {
            // NIE pokazuj błędów dopóki user nie kliknie "Zapisz"
            if (!ShowErrors)
                return "";

            switch (columnName)
            {
                case nameof(SelectedRestaurant):
                    return SelectedRestaurant == null ? "Wybierz restaurację." : "";

                case nameof(Name):
                    if (string.IsNullOrWhiteSpace(Name))
                        return "Nazwa strefy jest wymagana.";
                    if (Name.Trim().Length < 3)
                        return "Nazwa musi mieć co najmniej 3 znaki.";
                    if (Name.Trim().Length > 50)
                        return "Nazwa może mieć max 50 znaków.";
                    return "";

                case nameof(Description):
                    if (!string.IsNullOrWhiteSpace(Description) && Description.Trim().Length > 200)
                        return "Opis może mieć max 200 znaków.";
                    return "";
            }

            return "";
        }
    }


    private string _name = "";
    public string Name
    {
        get => _name;
        set
        {
            _name = value;
            OnPropertyChanged(() => Name);
            ((BaseCommand)SaveCommand).RaiseCanExecuteChanged();
        }
    }

    private string? _description;
    public string? Description
    {
        get => _description;
        set
        {
            _description = value;
            OnPropertyChanged(() => Description);
            ((BaseCommand)SaveCommand).RaiseCanExecuteChanged();
        }
    }

    public ICommand SaveCommand { get; }

    public AddAreaViewModel()
    {
        DisplayName = "Dodaj strefę";
        SaveCommand = new BaseCommand(Save, CanSave);
        LoadRestaurants();
    }

    private void LoadRestaurants()
    {
        Restaurants.Clear();

        var data = _context.Restaurants
            .OrderBy(r => r.RestaurantId)
            .ToList();

        foreach (var r in data)
            Restaurants.Add(r);

        SelectedRestaurant = Restaurants.FirstOrDefault();
    }

    private bool CanSave()
        => true;

    private void Save()
    {
        ShowErrors = true;

        OnPropertyChanged(() => SelectedRestaurant);
        OnPropertyChanged(() => Name);
        OnPropertyChanged(() => Description);

        if (!IsValid)
            return;

        var area = new Area
        {
            Name = Name.Trim(),
            Description = string.IsNullOrWhiteSpace(Description) ? null : Description.Trim(),
            RestaurantId = SelectedRestaurant!.RestaurantId
        };

        _context.Areas.Add(area);
        _context.SaveChanges();

        WeakReferenceMessenger.Default.Send(new AreaSavedMessage(area.AreaId));
        OnRequestClose();
    }



    private bool _showErrors;
    public bool ShowErrors
    {
        get => _showErrors;
        set
        {
            _showErrors = value;
            OnPropertyChanged(() => ShowErrors);

            // odśwież UI błędów
            OnPropertyChanged(() => Name);
            OnPropertyChanged(() => Description);
            OnPropertyChanged(() => SelectedRestaurant);
        }
    }


    public override bool IsValid =>
        string.IsNullOrEmpty(this[nameof(SelectedRestaurant)]) &&
        string.IsNullOrEmpty(this[nameof(Name)]) &&
        string.IsNullOrEmpty(this[nameof(Description)]);

}
