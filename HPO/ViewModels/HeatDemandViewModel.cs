using System;
using CommunityToolkit.Mvvm.ComponentModel;
using System.IO;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using HPO.Models;

namespace HPO.ViewModels;

public partial class HeatDemandViewModel : ViewModelBase
{
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
                UpdateSummerGraph();
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
                UpdateWinterGraph();
            }
        }
    }

    private const string CsvFilePath = "Assets/HPOInfo.csv";
    public ISeries[] SummerSeries { get; set; }
    public ISeries[] WinterSeries { get; set; }

    public Axis[] YAxes { get; set; }
        = new Axis[]
        {
            new Axis
            {
                Name = "Heat Demand in MW/h",
            }
        };

    public Axis[] XAxes { get; set; }
        = new Axis[]
        {
            new Axis
            {
                Name = "Time",
                Labels = GenerateTimeLabels(),
                LabelsRotation = 45
            }
        };

    private readonly FileReader _fileReader;

    private Dictionary<int, List<double>> _winterHeatDemandData;
    private Dictionary<int, List<double>> _summerHeatDemandData;

    public HeatDemandViewModel()
    {
        _fileReader = new FileReader();

        SummerDays = new ObservableCollection<int>(Enumerable.Range(11, 14));
        WinterDays = new ObservableCollection<int>(Enumerable.Range(1, 14));

        LoadHeatDemandData();

        SelectedSummerDay = 11;
        SelectedWinterDay = 1;
    }

    private void LoadHeatDemandData()
    {
        // These methods now use LoadData<T> internally
        _winterHeatDemandData = _fileReader.GetWinterHeatDemandData();
        _summerHeatDemandData = _fileReader.GetSummerHeatDemandData();

        UpdateWinterGraph();
        UpdateSummerGraph();
    }

    private void UpdateSummerGraph()
    {
        if (_summerHeatDemandData.ContainsKey(SelectedSummerDay))
        {
            var heatDemandData = _summerHeatDemandData[SelectedSummerDay];

            SummerSeries = new ISeries[]
            {
                new ColumnSeries<double>
                {
                    Values = heatDemandData.ToArray(),
                    Padding = 0, // Defines the distance between bars
                    MaxBarWidth = double.MaxValue, // Optional: Adjust as needed
                    Fill = new SolidColorPaint(SKColors.Blue) // Set a consistent color (e.g., blue)
                }
            };

            OnPropertyChanged(nameof(SummerSeries));
        }
    }

    private void UpdateWinterGraph()
    {
        if (_winterHeatDemandData.ContainsKey(SelectedWinterDay))
        {
            var heatDemandData = _winterHeatDemandData[SelectedWinterDay];

            WinterSeries = new ISeries[]
            {
                new ColumnSeries<double>
                {
                    Values = heatDemandData.ToArray(),
                    Padding = 0, // Defines the distance between bars
                    MaxBarWidth = double.MaxValue, // Optional: Adjust as needed
                    Fill = new SolidColorPaint(SKColors.Red) // Set a consistent color (e.g., red)
                }
            };

            OnPropertyChanged(nameof(WinterSeries));
        }
    }

    private static string[] GenerateTimeLabels()
    {
        return Enumerable.Range(0, 24)
            .Select(i => $"{i}:00-{(i + 1) % 24}:00")
            .ToArray();
    }
}