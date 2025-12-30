using CommunityToolkit.Mvvm.Messaging;
using RestaurantApp.Data;
using RestaurantApp.Dtos;
using RestaurantApp.Entities;
using RestaurantApp.Helper;
using RestaurantApp.Messages;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace RestaurantApp.ViewModels;

public class AllAreasViewModel : WorkspaceViewModel
{
    private readonly RestaurantDbContext _context = new();

    public ObservableCollection<AreaListItemDto> Areas { get; } = new();

    public event Action? RequestAddArea;

    public ICommand AddAreaCommand { get; }

    public AllAreasViewModel()
    {
        base.DisplayName = "Strefy";
        AddAreaCommand = new BaseCommand(() => RequestAddArea?.Invoke());

        WeakReferenceMessenger.Default.Register<AreaSavedMessage>(this, (_, msg) =>
        {
            Refresh();
        });

        LoadAreas();
    }

    private void LoadAreas()
    {
        Areas.Clear();

        var data = _context.Areas
                   .OrderBy(a => a.AreaId)
                   .Select(a => new AreaListItemDto
                   {
                       AreaId = a.AreaId,
                       RestaurantId = a.RestaurantId,
                       RestaurantName = a.Restaurant.Name,
                       Name = a.Name,
                       Description = a.Description
                   })
                   .ToList();

        foreach (var row in data)
            Areas.Add(row);
    }
    public void Refresh()
    {
        LoadAreas();
    }

}
