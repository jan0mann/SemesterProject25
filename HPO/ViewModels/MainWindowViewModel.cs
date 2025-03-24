using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Avalonia;
using Avalonia.Interactivity;
using HPO.Views;
using Avalonia.Controls;

namespace HPO.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private UserControl currentView;

    [ObservableProperty]
    private bool isPaneOpen = false;

    private FirstView _firstView = new FirstView { DataContext = new FirstViewModel() };
    private BoilerInfo _secondView = new BoilerInfo { DataContext = new BoilerInfoViewModel() };
    private HeatDemand _thirdView = new HeatDemand { DataContext = new HeatDemandViewModel() };

    public MainWindowViewModel()
    {
        CurrentView = _firstView;
        HeatDemandViewModel = new HeatDemandViewModel();
    }

    [RelayCommand]
    public void NavigateToFirstView()
    {
        CurrentView = _firstView;
    }

    [RelayCommand]
    public void NavigateToBoilerInfo()
    {
        CurrentView = _secondView;
    }

    [RelayCommand]
    public void NavigateToHeatDemand()
    {
        CurrentView = _thirdView;
    }

    public HeatDemandViewModel HeatDemandViewModel { get; }
}

