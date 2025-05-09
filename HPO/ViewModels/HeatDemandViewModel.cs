using System;
using CommunityToolkit.Mvvm.ComponentModel;
using System.IO;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using HPO.Models;
using HPO.Optimizer;
using System.Configuration.Assemblies;

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

    public ISeries[] SummerSeries { get; set; }
    public ISeries[] WinterSeries { get; set; }
    public ISeries[] SummerCostCO2Series { get; set; }
    public ISeries[] WinterCostCO2Series { get; set; }

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
            //MinLimit = 0,
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
    private readonly Dictionary<string, SkiaSharp.SKColor> _boilerColors = new()
    {
        { "Gas Boiler 1", new SkiaSharp.SKColor(255, 204, 0) },
        { "Gas Boiler 2", new SkiaSharp.SKColor(204, 102, 0) },
        { "Oil Boiler 1", new SkiaSharp.SKColor(128, 128, 128) },
    };

    private Dictionary<int, List<Optimizer.hourData>> _winterOptimizedData;
    private Dictionary<int, List<Optimizer.hourData>> _summerOptimizedData;
    private ObservableCollection<Boiler> _boilers;

    public HeatDemandViewModel()
    {
        SummerDays = new ObservableCollection<int>(Enumerable.Range(11, 14));
        WinterDays = new ObservableCollection<int>(Enumerable.Range(1, 14));

        _winterOptimizedData = new Dictionary<int, List<Optimizer.hourData>>();
        _summerOptimizedData = new Dictionary<int, List<Optimizer.hourData>>();
        var assetManager = new AssetManager();
        _boilers = assetManager.ShowInfo();

        LoadHeatDemandData();

        SelectedSummerDay = 11;
        SelectedWinterDay = 1;
    }

    private void LoadHeatDemandData()
    {
        _winterHeatDemandData = GetWinterHeatDemandData();
        _summerHeatDemandData = GetSummerHeatDemandData();

        UpdateWinterGraph();
        UpdateSummerGraph();

        ResultDataManager(_winterOptimizedData);
        ResultDataManager(_summerOptimizedData);
    }

    public Dictionary<int, List<(double, double)>> GetWinterHeatDemandData()
    {
        var optimizer = new Optimizer.Optimizer();
        var winterList = new FileReader().WriteList<Winter>();
        var winterHeatDemandData = new Dictionary<int, List<(double, double)>>();

        foreach (var winter in winterList)
        {
            if (DateTime.TryParse(winter.WTimeFrom, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime wTimeFrom) &&
                winter.WHeatDemand.HasValue)
            {
                int day = wTimeFrom.Day;

                if (!winterHeatDemandData.ContainsKey(day))
                {
                    winterHeatDemandData[day] = new List<(double, double)>();
                }

                winterHeatDemandData[day].Add((winter.WHeatDemand.Value, winter.WPrice.Value));
            }
        }
        _winterHeatDemandData = winterHeatDemandData;

        foreach (var day in WinterDays)
        {
            _winterOptimizedData[day] = new List<Optimizer.hourData>();
        }

        foreach (var day in _winterHeatDemandData.Keys)
        {
            var data = winterHeatDemandData[day];
            _winterOptimizedData[day] = new List<Optimizer.hourData>();

            foreach (var hour in data)
            {
                var hourdata = optimizer.OptimizeHour(_boilers.Select(x => x.Deepcopy()).ToList(), hour.Item1, hour.Item2);
                _winterOptimizedData[day].Add(hourdata);
            }
        }

        return winterHeatDemandData;
    }

    public Dictionary<int, List<(double, double)>> GetSummerHeatDemandData()
    {
        var optimizer = new Optimizer.Optimizer();
        var summerList = new FileReader().WriteList<Summer>();

        var summerHeatDemandData = new Dictionary<int, List<(double, double)>>();

        foreach (var summer in summerList)
        {
            if (DateTime.TryParse(summer.STimeFrom, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime sTimeFrom) &&
                summer.SHeatDemand.HasValue)
            {
                int day = sTimeFrom.Day;

                if (!summerHeatDemandData.ContainsKey(day))
                {
                    summerHeatDemandData[day] = new List<(double, double)>();
                }

                summerHeatDemandData[day].Add((summer.SHeatDemand.Value, summer.SPrice.Value));
            }
        }

        _summerHeatDemandData = summerHeatDemandData;

        foreach (var day in SummerDays)
        {
            _summerOptimizedData[day] = new List<Optimizer.hourData>();
        }

        foreach (var day in _summerHeatDemandData.Keys)
        {
            var data = summerHeatDemandData[day];
            _summerOptimizedData[day] = new List<Optimizer.hourData>();

            foreach (var hour in data)
            {
                var hourdata = optimizer.OptimizeHour(_boilers.Select(x => x.Deepcopy()).ToList(), hour.Item1, hour.Item2);
                _summerOptimizedData[day].Add(hourdata);
            }
        }

        return summerHeatDemandData;
    }

    private void UpdateSummerGraph()
    {
        if (_summerHeatDemandData.ContainsKey(SelectedSummerDay))
        {
            var heatDemandData = _summerHeatDemandData[SelectedSummerDay];
            var hoursData = _summerOptimizedData[SelectedSummerDay];
            Dictionary<string, List<double>> boilersData = new();

            foreach (var boiler in _boilers)
            {
                boilersData[boiler.Name] = new List<double>();
            }

            foreach (var hour in hoursData)
            {
                foreach (var boiler in hour.Boilers)
                {
                    boilersData[boiler.Name].Add(boiler.HeatProduced);
                }
            }

            var filteredBoilersData = boilersData
                //.Where(b => b.Value.Any(v => v > 0))
                .OrderByDescending(b => b.Value.Sum())
                .ToList();

            var summerSeriesList = new List<ISeries>();
            var summerCostCO2SeriesList = new List<ISeries>();

            foreach (var boiler in filteredBoilersData)
            {
                summerSeriesList.Add(
                    new StackedAreaSeries<double>
                    {
                        Values = boiler.Value.ToArray(),
                        GeometrySize = 0,
                        LineSmoothness = 0,
                        Name = boiler.Key,
                        Fill = new LiveChartsCore.SkiaSharpView.Painting.SolidColorPaint
                        {
                            Color = _boilerColors.ContainsKey(boiler.Key)
                                ? _boilerColors[boiler.Key]
                                : new SkiaSharp.SKColor(128, 128, 128)
                        }
                    }
                );
            }
            List<double> heatDemandData2 = new();
            foreach(var hour in heatDemandData){
                heatDemandData2.Add(hour.Item1);
            }
            summerSeriesList.Add(
                new LineSeries<double>
                {
                    Values = heatDemandData2.ToArray(),
                    GeometrySize = 0,
                    LineSmoothness = 0,
                    Stroke = new LiveChartsCore.SkiaSharpView.Painting.SolidColorPaint
                    {
                        Color = new SkiaSharp.SKColor(0, 0, 0),
                        StrokeThickness = 6
                    },
                    Fill = null,
                    Name = "Heat Demand"
                }
            );

            List<double> summedCO2 = new();
            List<double> summedCost = new();
            foreach (var hour in hoursData)
            {
                double sumCO2 = 0;
                double sumCost = 0;
                foreach (var boiler in hour.Boilers)
                {
                    sumCO2 += boiler.CO2Produced;
                    sumCost += boiler.Cost;
                }
                summedCO2.Add(sumCO2);
                summedCost.Add(sumCost);
            }

            summerCostCO2SeriesList.Add(
            new ColumnSeries<double>
            {
                Values = summedCO2,
                Fill = new LiveChartsCore.SkiaSharpView.Painting.SolidColorPaint
                {
                    Color = new SkiaSharp.SKColor(255, 0, 0)
                },
                Name = "CO2 Emission(Kg)"
            });
            summerCostCO2SeriesList.Add(
                new ColumnSeries<double>
                {
                    Values = summedCost,
                    Fill = new LiveChartsCore.SkiaSharpView.Painting.SolidColorPaint
                    {
                        Color = new SkiaSharp.SKColor(0, 0, 255)
                    },
                    Name = "Cost(DKK)"
                });

            SummerSeries = summerSeriesList.ToArray();
            SummerCostCO2Series = summerCostCO2SeriesList.ToArray();

            OnPropertyChanged(nameof(SummerSeries));
            OnPropertyChanged(nameof(SummerCostCO2Series));
        }
    }

    private void UpdateWinterGraph()
    {
        if (_winterHeatDemandData.ContainsKey(SelectedWinterDay))
        {
            var heatDemandData = _winterHeatDemandData[SelectedWinterDay];
            var hoursData = _winterOptimizedData[SelectedWinterDay];
            Dictionary<string, List<double>> boilersData = new();

            foreach (var boiler in _boilers)
            {
                boilersData[boiler.Name] = new List<double>();
            }

            foreach (var hour in hoursData)
            {
                foreach (var boiler in hour.Boilers)
                {
                    boilersData[boiler.Name].Add(boiler.HeatProduced);
                }
            }

            var filteredBoilersData = boilersData
                //.Where(b => b.Value.Any(v => v > 0))
                .OrderByDescending(b => b.Value.Sum())
                .ToList();

            var winterSeriesList = new List<ISeries>();
            var winterCostCO2SeriesList = new List<ISeries>();

            foreach (var boiler in filteredBoilersData)
            {
                winterSeriesList.Add(
                    new StackedAreaSeries<double>
                    {
                        Values = boiler.Value.ToArray(),
                        GeometrySize = 0,
                        LineSmoothness = 0,
                        Name = boiler.Key,
                        Fill = new LiveChartsCore.SkiaSharpView.Painting.SolidColorPaint
                        {
                            Color = _boilerColors.ContainsKey(boiler.Key)
                                ? _boilerColors[boiler.Key]
                                : new SkiaSharp.SKColor(128, 128, 128)
                        }
                    }
                );
            }

            List<double> heatDemandData2 = new();
            foreach(var hour in heatDemandData){
                heatDemandData2.Add(hour.Item1);
            }
            winterSeriesList.Add(
                new LineSeries<double>
                {
                    
                    Values = heatDemandData2.ToArray(),
                    GeometrySize = 0,
                    LineSmoothness = 0,
                    Stroke = new LiveChartsCore.SkiaSharpView.Painting.SolidColorPaint
                    {
                        Color = new SkiaSharp.SKColor(0, 0, 0),
                        StrokeThickness = 6
                    },
                    Fill = null,
                    Name = "Heat Demand"
                }
            );

            List<double> summedCO2 = new();
            List<double> summedCost = new();
            foreach (var hour in hoursData)
            {
                double sumCO2 = 0;
                double sumCost = 0;
                foreach (var boiler in hour.Boilers)
                {
                    sumCO2 += boiler.CO2Produced;
                    sumCost += boiler.Cost;
                }
                summedCO2.Add(sumCO2);
                summedCost.Add(Math.Abs(sumCost));
            }

            winterCostCO2SeriesList.Add(
            new ColumnSeries<double>
            {
                Values = summedCO2,
                Fill = new LiveChartsCore.SkiaSharpView.Painting.SolidColorPaint
                {
                    Color = new SkiaSharp.SKColor(255, 0, 0)
                },
                Name = "CO2 Emission(Kg)"
            });
            winterCostCO2SeriesList.Add(
                new ColumnSeries<double>
                {
                    Values = summedCost,
                    Fill = new LiveChartsCore.SkiaSharpView.Painting.SolidColorPaint
                    {
                        Color = new SkiaSharp.SKColor(0, 0, 255)
                    },
                    Name = "Cost(DKK)"
                });
            WinterSeries = winterSeriesList.ToArray();
            WinterCostCO2Series = winterCostCO2SeriesList.ToArray();

            OnPropertyChanged(nameof(WinterSeries));
            OnPropertyChanged(nameof(WinterCostCO2Series));
        }
    }


    private void ResultDataManager(Dictionary<int, List<Optimizer.hourData>> optimizedData)
{
    // Define output folder path
    string outputDirectory = Path.Combine(AppContext.BaseDirectory, "ResultData");
    // Ensure directory exists
    Directory.CreateDirectory(outputDirectory);

    // Create a dictionary to hold boiler-specific data
    var boilerData = new Dictionary<string, List<string>>();

    // Collect data per boiler
    foreach (var dayEntry in optimizedData)
    {
        int day = dayEntry.Key;
        var hours = dayEntry.Value;

        for (int hourIndex = 0; hourIndex < hours.Count; hourIndex++)
        {
            var hourData = hours[hourIndex];

            foreach (var boiler in hourData.Boilers)
            {
                if (!boilerData.ContainsKey(boiler.Name))
                {
                    boilerData[boiler.Name] = new List<string>();
                    // Add header line
                    boilerData[boiler.Name].Add("Day,Hour,HeatProduced,CO2Produced,Cost");
                }
                boilerData[boiler.Name].Add($"{day},{hourIndex},{boiler.HeatProduced},{boiler.CO2Produced},{boiler.Cost}");
            }
        }
    }

    // Write data to CSV files
    foreach (var boilerEntry in boilerData)
    {
        string sanitizedBoilerName = boilerEntry.Key.Replace(" ", "_").Replace("/", "_");
        string filePath = Path.Combine(outputDirectory, $"{sanitizedBoilerName}.csv");

        File.WriteAllLines(filePath, boilerEntry.Value);
    }
}


}