using System;
using CommunityToolkit.Mvvm.ComponentModel;
using System.IO;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

namespace HPO.ViewModels;

public partial class HeatDemandViewModel : ViewModelBase
{
    public ObservableCollection<int> Days { get; }

    private int _selectedDay;
    public int SelectedDay
    {
        get => _selectedDay;
        set
        {
            if (SetProperty(ref _selectedDay, value))
            {
                UpdateGraph();
            }
        }
    }

    private const string CsvFilePath = "Assets/HPOInfo.csv"; // Updated file path
    public ISeries[] Series { get; set; }

    public Axis[] YAxes { get; set; }
        = new Axis[]
        {
            new Axis
            {
                Name = "Heat Demand in MW/h",
            }
        };

    private Dictionary<int, List<double>> _dailyHeatDemandData;

    public HeatDemandViewModel()
    {
        Days = new ObservableCollection<int>(Enumerable.Range(1, 14));
        _dailyHeatDemandData = new Dictionary<int, List<double>>();
        LoadHeatDemandData();
        SelectedDay = 1;
    }

    private void LoadHeatDemandData()
    {
        using (var reader = new StreamReader(CsvFilePath))
        {
            var header = reader.ReadLine(); // Skip the header row

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(',');

                // Assuming WTimeFrom is the date and WHeatDemand is the heat demand
                if (values.Length >= 5 &&
                    DateTime.TryParse(values[0], CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date) &&
                    double.TryParse(values[4], NumberStyles.Any, CultureInfo.InvariantCulture, out double heatDemand))
                {
                    int day = date.Day;

                    if (!_dailyHeatDemandData.ContainsKey(day))
                    {
                        _dailyHeatDemandData[day] = new List<double>();
                    }

                    _dailyHeatDemandData[day].Add(heatDemand);
                }
            }
        }

        UpdateGraph();
    }

    private void UpdateGraph()
    {
        if (_dailyHeatDemandData.TryGetValue(SelectedDay, out var heatDemandData))
        {
            Series = new ISeries[]
            {
                new LineSeries<double>
                {
                    Values = heatDemandData.ToArray(),
                    Fill = null,
                    GeometrySize = 0,
                    LineSmoothness = 0
                }
            };

            OnPropertyChanged(nameof(Series));
        }
    }
}
