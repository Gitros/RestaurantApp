using RestaurantApp.Data;
using RestaurantApp.Entities;
using RestaurantApp.Helper;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace RestaurantApp.ViewModels;

public class AddTableViewModel : WorkspaceViewModel
{
    private readonly RestaurantDbContext _context = new();

    public event Action? TableSaved;

    public ObservableCollection<Area> Areas { get; } = new();

    private Area? _selectedArea;
    public Area? SelectedArea
    {
        get => _selectedArea;
        set
        {
            _selectedArea = value;
            OnPropertyChanged(() => SelectedArea);
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

    private int _seatsCount = 2;
    public int SeatsCount
    {
        get => _seatsCount;
        set
        {
            _seatsCount = value;
            OnPropertyChanged(() => SeatsCount);
            ((BaseCommand)SaveCommand).RaiseCanExecuteChanged();
        }
    }

    private bool _isActive = true;
    public bool IsActive
    {
        get => _isActive;
        set
        {
            _isActive = value;
            OnPropertyChanged(() => IsActive);
        }
    }

    public ICommand SaveCommand { get; }

    public AddTableViewModel()
    {
        DisplayName = "Dodaj stolik";
        SaveCommand = new BaseCommand(Save, CanSave);

        LoadAreas();
    }

    private void LoadAreas()
    {
        Areas.Clear();

        var data = _context.Areas
            .OrderBy(a => a.AreaId)
            .ToList();

        foreach (var a in data)
            Areas.Add(a);

        SelectedArea = Areas.FirstOrDefault();
    }

    private bool CanSave()
        => SelectedArea != null
           && !string.IsNullOrWhiteSpace(Name)
           && SeatsCount > 0;

    private void Save()
    {
        var table = new RestaurantTable
        {
            AreaId = SelectedArea!.AreaId,     // FK
            Name = Name.Trim(),
            SeatsCount = SeatsCount,
            IsActive = IsActive
        };

        _context.Tables.Add(table);
        _context.SaveChanges();

        TableSaved?.Invoke();
        OnRequestClose();
    }
}
