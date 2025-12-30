using CommunityToolkit.Mvvm.Messaging;
using RestaurantApp.Messages;
using RestaurantApp.Data;
using RestaurantApp.Entities;
using RestaurantApp.Helper;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace RestaurantApp.ViewModels;

public class AddAreaViewModel : WorkspaceViewModel
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
        => SelectedRestaurant != null && !string.IsNullOrWhiteSpace(Name);

    private void Save()
    {
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
}
