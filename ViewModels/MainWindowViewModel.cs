using RestaurantApp.Helper;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Data;
using System.Windows.Forms;
using CommunityToolkit.Mvvm.Messaging;
using RestaurantApp.Messages;
using System.Windows.Threading;


namespace RestaurantApp.ViewModels;

public class MainWindowViewModel : BaseViewModel
{
    #region Fields
    private ReadOnlyCollection<CommandViewModel> _Commands;
    private ObservableCollection<WorkspaceViewModel> _Workspaces;
    #endregion

    // ===== NOTYFIKACJE (BANNER) =====
    private string _notificationText = "";
    public string NotificationText
    {
        get => _notificationText;
        set { _notificationText = value; OnPropertyChanged(() => NotificationText); }
    }

    private bool _isNotificationVisible;
    public bool IsNotificationVisible
    {
        get => _isNotificationVisible;
        set { _isNotificationVisible = value; OnPropertyChanged(() => IsNotificationVisible); }
    }



    public MainWindowViewModel()
    {
        WeakReferenceMessenger.Default.Register<UiNotificationMessage>(this, (_, msg) =>
        {
            NotificationText = msg.Value.Text;
            IsNotificationVisible = true;

            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3) };
            timer.Tick += (_, __) =>
            {
                timer.Stop();
                IsNotificationVisible = false;
            };
            timer.Start();
        });
    }




    #region Commands
    public ReadOnlyCollection<CommandViewModel> Commands
    {
        get
        {
            if (_Commands == null)
            {
                List<CommandViewModel> cmds = this.CreateCommands();
                _Commands = new ReadOnlyCollection<CommandViewModel>(cmds);
            }
            return _Commands;
        }
    }
    private List<CommandViewModel> CreateCommands()
    {
        return new List<CommandViewModel>
    {
        new CommandViewModel(
            "Zamówienia",
            new BaseCommand(() => this.ShowAllOrders())),

        new CommandViewModel(
            "Strefy",
            new BaseCommand(() => this.ShowAllAreas())),

        new CommandViewModel(
            "Stoliki",
            new BaseCommand(() => this.ShowAllTables())),

        new CommandViewModel(
            "Menu",
            new BaseCommand(() => this.ShowAllMenuItems())),

        new CommandViewModel(
            "Rezerwacje",
            new BaseCommand(() => this.ShowAllReservations())),

        new CommandViewModel(
            "Klienci",
            new BaseCommand(() => this.ShowAllCustomers())),

        new CommandViewModel(
            "Składniki",
            new BaseCommand(() => this.ShowAllIngredients())),

        new CommandViewModel(
            "Kelnerzy",
            new BaseCommand(() => this.ShowAllWaiters())),

        new CommandViewModel(
            "Płatności",
            new BaseCommand(() => this.ShowAllPayments())),

        new CommandViewModel(
            "Raport: sprzedaż dzienna",
            new BaseCommand(() => this.ShowDailySalesReport())),

        new CommandViewModel(
            "Raport: TOP menu",
            new BaseCommand(() => this.ShowTopMenuItemsReport())),

        new CommandViewModel(
            "Raport: obłożenie stolików",
            new BaseCommand(() => this.ShowTableUsageReport())),
    };
    }

    #endregion

    #region Workspaces
    public ObservableCollection<WorkspaceViewModel> Workspaces
    {
        get
        {
            if (_Workspaces == null)
            {
                _Workspaces = new ObservableCollection<WorkspaceViewModel>();
                _Workspaces.CollectionChanged += this.OnWorkspacesChanged;
            }
            return _Workspaces;
        }
    }
    private void OnWorkspacesChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems != null && e.NewItems.Count != 0)
            foreach (WorkspaceViewModel workspace in e.NewItems)
                workspace.RequestClose += this.OnWorkspaceRequestClose;

        if (e.OldItems != null && e.OldItems.Count != 0)
            foreach (WorkspaceViewModel workspace in e.OldItems)
                workspace.RequestClose -= this.OnWorkspaceRequestClose;
    }
    private void OnWorkspaceRequestClose(object sender, EventArgs e)
    {
        WorkspaceViewModel workspace = sender as WorkspaceViewModel;
        //workspace.Dispos();
        this.Workspaces.Remove(workspace);
    }

    #endregion // Workspaces

    #region Private Helpers
    //private void CreateView( WorkspaceViewModel workspace)
    //{
    //    this.Workspaces.Add(workspace);//dodajemy zakladke do kolekcji zakladek
    //    this.SetActiveWorkspace(workspace);//aktywujemy zakladke (zeby byla wlaczona)
    //}

    private void ShowAllOrders()
    {
        var workspace = this.Workspaces.OfType<AllOrdersViewModel>().FirstOrDefault();

        if (workspace == null)
        {
            workspace = new AllOrdersViewModel();
            this.Workspaces.Add(workspace);
        }

        workspace.RequestAddOrder -= this.ShowAddOrder;
        workspace.RequestAddOrder += this.ShowAddOrder;

        this.SetActiveWorkspace(workspace);
    }

    private void ShowAddOrder()
    {
        var workspace = this.Workspaces.OfType<AddOrderViewModel>().FirstOrDefault();

        if (workspace == null)
        {
            workspace = new AddOrderViewModel();
            this.Workspaces.Add(workspace);
        }

        this.SetActiveWorkspace(workspace);
    }


    private void ShowAllAreas()
    {

        var workspace = this.Workspaces.OfType<AllAreasViewModel>().FirstOrDefault();

        if (workspace == null)
        {
            workspace = new AllAreasViewModel();
            this.Workspaces.Add(workspace);
        }

        workspace.RequestAddArea -= this.ShowAddArea;
        workspace.RequestAddArea += this.ShowAddArea;

        SetActiveWorkspace(workspace);
    }


    private void ShowAllCustomers()
    {
        AllCustomersViewModel workspace =
            this.Workspaces.FirstOrDefault(vm => vm is AllCustomersViewModel)
            as AllCustomersViewModel;

        if (workspace == null)
        {
            workspace = new AllCustomersViewModel();
            this.Workspaces.Add(workspace);
        }

        this.SetActiveWorkspace(workspace);
    }

    private void ShowAllIngredients()
    {
        AllIngredientsViewModel workspace =
            this.Workspaces.FirstOrDefault(vm => vm is AllIngredientsViewModel)
            as AllIngredientsViewModel;

        if (workspace == null)
        {
            workspace = new AllIngredientsViewModel();
            this.Workspaces.Add(workspace);
        }

        this.SetActiveWorkspace(workspace);
    }


    private void ShowAllTables()
    {
        var workspace = this.Workspaces.OfType<AllTablesViewModel>().FirstOrDefault();

        if (workspace == null)
        {
            workspace = new AllTablesViewModel();
            this.Workspaces.Add(workspace);
        }

        workspace.RequestAddTable -= this.ShowAddTable;
        workspace.RequestAddTable += this.ShowAddTable;

        this.SetActiveWorkspace(workspace);
    }

    private void ShowAddTable()
    {
        var workspace = this.Workspaces.OfType<AddTableViewModel>().FirstOrDefault();

        if (workspace == null)
        {
            workspace = new AddTableViewModel();

            workspace.TableSaved += () =>
            {
                var tablesVm = this.Workspaces.OfType<AllTablesViewModel>().FirstOrDefault();
                tablesVm?.Refresh();
            };

            this.Workspaces.Add(workspace);
        }

        this.SetActiveWorkspace(workspace);
    }


    private void ShowAllMenuItems()
    {
        AllMenuItemsViewModel workspace =
            this.Workspaces.FirstOrDefault(vm => vm is AllMenuItemsViewModel)
            as AllMenuItemsViewModel;

        if (workspace == null)
        {
            workspace = new AllMenuItemsViewModel();
            this.Workspaces.Add(workspace);
        }

        this.SetActiveWorkspace(workspace);
    }

    private void ShowAllReservations()
    {
        AllReservationsViewModel workspace =
            this.Workspaces.FirstOrDefault(vm => vm is AllReservationsViewModel)
            as AllReservationsViewModel;

        if (workspace == null)
        {
            workspace = new AllReservationsViewModel();
            this.Workspaces.Add(workspace);
        }

        this.SetActiveWorkspace(workspace);
    }

    private void ShowAllWaiters()
    {
        AllWaitersViewModel workspace =
            this.Workspaces.FirstOrDefault(vm => vm is AllWaitersViewModel)
            as AllWaitersViewModel;

        if (workspace == null)
        {
            workspace = new AllWaitersViewModel();
            this.Workspaces.Add(workspace);
        }

        this.SetActiveWorkspace(workspace);
    }

    private void ShowAllPayments()
    {
        AllPaymentsViewModel workspace =
            this.Workspaces.FirstOrDefault(vm => vm is AllPaymentsViewModel)
            as AllPaymentsViewModel;

        if (workspace == null)
        {
            workspace = new AllPaymentsViewModel();
            this.Workspaces.Add(workspace);
        }

        this.SetActiveWorkspace(workspace);
    }

    private void SetActiveWorkspace(WorkspaceViewModel workspace)
    {
        Debug.Assert(this.Workspaces.Contains(workspace));

        ICollectionView collectionView = CollectionViewSource.GetDefaultView(this.Workspaces);
        if (collectionView != null)
            collectionView.MoveCurrentTo(workspace);
    }
    #endregion

    private void ShowAddArea()
    {
        var workspace = this.Workspaces.OfType<AddAreaViewModel>().FirstOrDefault();

        if (workspace == null)
        {
            workspace = new AddAreaViewModel();
            this.Workspaces.Add(workspace);
        }

        this.SetActiveWorkspace(workspace);
    }

    private void ShowDailySalesReport()
    {
        var workspace = this.Workspaces.OfType<DailySalesReportViewModel>().FirstOrDefault();

        if (workspace == null)
        {
            workspace = new DailySalesReportViewModel();
            this.Workspaces.Add(workspace);
        }

        this.SetActiveWorkspace(workspace);
    }

    private void ShowTopMenuItemsReport()
    {
        var workspace = this.Workspaces.OfType<TopMenuItemsReportViewModel>().FirstOrDefault();

        if (workspace == null)
        {
            workspace = new TopMenuItemsReportViewModel();
            this.Workspaces.Add(workspace);
        }

        this.SetActiveWorkspace(workspace);
    }

    private void ShowTableUsageReport()
    {
        var workspace = this.Workspaces.OfType<TableUsageReportViewModel>().FirstOrDefault();

        if (workspace == null)
        {
            workspace = new TableUsageReportViewModel();
            this.Workspaces.Add(workspace);
        }

        this.SetActiveWorkspace(workspace);
    }

}
