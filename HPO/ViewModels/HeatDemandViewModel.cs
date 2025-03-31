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

    private const string CsvFilePath = "Assets/HeatDemand.csv";
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

            reader.ReadLine();

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(',');

                if (values.Length >= 3 &&
                    DateTime.TryParse(values[0], CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date) &&
                    double.TryParse(values[2], NumberStyles.Any, CultureInfo.InvariantCulture, out double demand))
                {
                    int day = date.Day;

                    if (!_dailyHeatDemandData.ContainsKey(day))
                    {
                        _dailyHeatDemandData[day] = new List<double>();
                    }

                    _dailyHeatDemandData[day].Add(demand);
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


