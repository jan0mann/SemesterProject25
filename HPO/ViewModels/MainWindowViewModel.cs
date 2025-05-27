using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Avalonia;
using Avalonia.Interactivity;
using HPO.Views;
using Avalonia.Controls;
using System;

namespace HPO.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private UserControl currentView;

    [ObservableProperty]
    private bool isPaneOpen = false;

    public HeatDemandViewModel HeatDemandViewModel { get; }
    public BoilerInfoViewModel BoilerInfoViewModel { get; }

    private FirstView _firstView;
    private BoilerInfo _secondView;
    private HeatDemand _thirdView;

    public MainWindowViewModel()
    {
        HeatDemandViewModel = new HeatDemandViewModel();
        BoilerInfoViewModel = new BoilerInfoViewModel();

        _firstView = new FirstView { DataContext = HeatDemandViewModel };
        _secondView = new BoilerInfo { DataContext = BoilerInfoViewModel };
        _thirdView = new HeatDemand { DataContext = HeatDemandViewModel };

        CurrentView = _firstView;
    }

    [RelayCommand]
    public void NavigateToFirstView() => CurrentView = _firstView;

    [RelayCommand]
    public void NavigateToBoilerInfo() => CurrentView = _secondView;

    [RelayCommand]
    public void NavigateToHeatDemand() => CurrentView = _thirdView;

    // SCENARIO COMMANDS
    [RelayCommand]
    public void ShowScenario1()
    {
        HeatDemandViewModel.CurrentSummerScenario = 1;
        HeatDemandViewModel.CurrentWinterScenario = 1;
        BoilerInfoViewModel.ShowScenario1Command.Execute(null);
        Console.WriteLine(HeatDemandViewModel.CurrentSummerScenario);
    }

    [RelayCommand]
    public void ShowScenario2()
    {
        HeatDemandViewModel.CurrentSummerScenario = 2;
        HeatDemandViewModel.CurrentWinterScenario = 2;
        BoilerInfoViewModel.ShowScenario2Command.Execute(null);
        Console.WriteLine(HeatDemandViewModel.CurrentSummerScenario);
    }
}