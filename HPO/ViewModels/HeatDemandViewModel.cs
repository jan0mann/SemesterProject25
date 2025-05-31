using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.IO;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using HPO.Models;
using HPO.Optimizer;

namespace HPO.ViewModels;

public partial class HeatDemandViewModel : ViewModelBase
{
    [ObservableProperty]
    private ISeries[] summerSeries;

    [ObservableProperty]
    private ISeries[] winterSeries;

    [ObservableProperty]
    private ISeries[] summerCostCO2Series;

    [ObservableProperty]
    private ISeries[] winterCostCO2Series;

    [ObservableProperty]
    private ISeries[] summerEPriceSeries;

    [ObservableProperty]
    private ISeries[] winterEPriceSeries;

    [ObservableProperty]
    private ISeries[] electricityProductionSeries;

    private ObservableCollection<Boiler> _boilersScenario1;
    private ObservableCollection<Boiler> _boilersScenario2;
    public ObservableCollection<int> SummerDays { get; }
    public ObservableCollection<int> WinterDays { get; }

    private int _selectedSummerDay;
    public int SelectedSummerDay
    {
        get => _selectedSummerDay;
        set
        {
            if (SetProperty(ref _selectedSummerDay, value))
            {
                UpdateSummerGraphByScenario();
            }
        }
    }

    private int _selectedWinterDay;
    public int SelectedWinterDay
    {
        get => _selectedWinterDay;
        set
        {
            if (SetProperty(ref _selectedWinterDay, value))
            {
                UpdateWinterGraphByScenario();
            }
        }
    }

    private int _currentSummerScenario = 1;
    public int CurrentSummerScenario
    {
        get => _currentSummerScenario;
        set
        {
            if (SetProperty(ref _currentSummerScenario, value))
            {
                UpdateSummerGraphByScenario();
            }
        }
    }

    private int _currentWinterScenario = 1;
    public int CurrentWinterScenario
    {
        get => _currentWinterScenario;
        set
        {
            if (SetProperty(ref _currentWinterScenario, value))
            {
                UpdateWinterGraphByScenario();
            }
        }
    }



    [RelayCommand]
    private void SaveScenario1BoilerDistribution()
    {
        SaveScenarioBoilerDistributionToCsvMerged(1);
    }

    [RelayCommand]
    private void SaveScenario2BoilerDistribution()
    {
        SaveScenarioBoilerDistributionToCsvMerged(2);
    }



    public Axis[] HeatDemandYAxes { get; set; }
        = new Axis[]
        {
            new Axis
            {
                Name = "Heat Demand in MW/h",
                MinLimit = 0,
            }
        };

    public Axis[] CostCO2YAxes { get; set; }
        = new Axis[]
        {
            new Axis
            {
                Name = "Cost and CO2 Emission",
            }
        };
    public Axis[] XAxes { get; set; }
        = new Axis[]
        {
            new Axis
            {
                Name = "Hours",
                Labels = Enumerable.Range(0, 24)
                    .Select(i => $"{i}:00-{(i + 1) % 24}:00")
                    .ToArray(),
                LabelsRotation = 45,
                UnitWidth = 0.5,
                MinStep = 1,
                ForceStepToMin = true
            }
        };

    private Dictionary<int, List<(double, double)>> _winterHeatDemandData;
    private Dictionary<int, List<(double, double)>> _summerHeatDemandData;
    private Dictionary<int, List<(double, double)>> _winterHeatDemandData2;
    private Dictionary<int, List<(double, double)>> _summerHeatDemandData2;

    private readonly Dictionary<string, SkiaSharp.SKColor> _boilerColors = new()
    {
        { "Gas Boiler 1", new SkiaSharp.SKColor(255, 204, 0) },
        { "Gas Boiler 2", new SkiaSharp.SKColor(204, 102, 0) },
        { "Oil Boiler 1", new SkiaSharp.SKColor(128, 128, 128) },
        { "Heat Pump 1", new SkiaSharp.SKColor(0, 200, 0) },
        { "Gas Motor 1", new SkiaSharp.SKColor(0, 102, 255) },
    };

    private Dictionary<int, List<Optimizer.hourData>> _winterOptimizedData;
    private Dictionary<int, List<Optimizer.hourData>> _summerOptimizedData;
    private Dictionary<int, List<Optimizer.hourData>> _winterOptimizedData2;
    private Dictionary<int, List<Optimizer.hourData>> _summerOptimizedData2;
    private ObservableCollection<Boiler> _boilers;

    public HeatDemandViewModel()
    {
        SummerDays = new ObservableCollection<int>(Enumerable.Range(11, 14));
        WinterDays = new ObservableCollection<int>(Enumerable.Range(1, 14));

        _winterOptimizedData = new Dictionary<int, List<Optimizer.hourData>>();
        _summerOptimizedData = new Dictionary<int, List<Optimizer.hourData>>();
        _winterOptimizedData2 = new Dictionary<int, List<Optimizer.hourData>>();
        _summerOptimizedData2 = new Dictionary<int, List<Optimizer.hourData>>();

        var assetManager = new AssetManager();
        _boilersScenario1 = assetManager.ShowBoilerInfo1();
        _boilersScenario2 = assetManager.ShowBoilerInfo2();

        LoadHeatDemandData();

        SelectedSummerDay = 11;
        SelectedWinterDay = 1;
    }
    private void LoadHeatDemandData()
    {
        _winterHeatDemandData = GetHeatDemandData(1, 'w');
        _summerHeatDemandData = GetHeatDemandData(1, 's');

        _winterHeatDemandData2 = GetHeatDemandData(2, 'w');
        _summerHeatDemandData2 = GetHeatDemandData(2, 's');


        UpdateWinterGraphByScenario();
        UpdateSummerGraphByScenario();
    }


    private void SaveScenarioBoilerDistributionToCsvMerged(int scenario)
    {
        RDM.SaveScenarioBoilerDistributionToCsvMerged(
            scenario,
            _summerOptimizedData,
            _winterOptimizedData,
            _summerOptimizedData2,
            _winterOptimizedData2
        );
    }
}