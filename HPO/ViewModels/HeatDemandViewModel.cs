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
using HPO.ViewModels;
using HPO.Optimizer;

namespace HPO.ViewModels;

public partial class HeatDemandViewModel : ViewModelBase
{
    private ObservableCollection<Boiler> _boilersScenario1;
    private ObservableCollection<Boiler> _boilersScenario2;
    public ObservableCollection<int> SummerDays { get; }
    public ObservableCollection<int> WinterDays { get; }
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

    [RelayCommand]
    private void SaveResults()
    {
        Console.WriteLine("SaveResults called, and saved to DownloadsFolder!");
        //SaveAllOptimizerResultsToCsv();
        SaveBoilerDistributionToCsv();
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

    // 1 or 2, can be set from UI or code
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
        _boilersScenario1 = assetManager.ShowInfo1();
        _boilersScenario2 = assetManager.ShowInfo2();

        LoadHeatDemandData();

        SelectedSummerDay = 11;
        SelectedWinterDay = 1;
    }
    private void LoadHeatDemandData()
    {
        _winterHeatDemandData = GetWinterHeatDemandData1();
        _summerHeatDemandData = GetSummerHeatDemandData1();

        _winterHeatDemandData2 = GetWinterHeatDemandData2();
        _summerHeatDemandData2 = GetSummerHeatDemandData2();


        UpdateWinterGraphScenario1();
        UpdateSummerGraphScenario1();


        //ResultDataManager(_winterOptimizedData);
        //ResultDataManager(_summerOptimizedData);
    }

    private Dictionary<int, List<(double, double)>> GetHeatDemandData<T, TData>(
    Func<TData, string> timeFromSelector,
    Func<TData, double?> heatDemandSelector,
    Func<TData, double?> priceSelector,
    IEnumerable<TData> dataList,
    Func<List<Boiler>, double, double, Optimizer.hourData> optimizeWithPrice,
    Func<List<Boiler>, double, Optimizer.hourData> optimizeWithoutPrice,
    bool usePrice,
    ObservableCollection<int> days,
    Dictionary<int, List<Optimizer.hourData>> optimizedDataDict,
    ObservableCollection<Boiler> scenarioBoilers)

    {
        var heatDemandData = new Dictionary<int, List<(double, double)>>();

        foreach (var item in dataList)
        {
            if (DateTime.TryParse(timeFromSelector(item), CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime timeFrom) &&
                heatDemandSelector(item).HasValue)
            {
                int day = timeFrom.Day;

                if (!heatDemandData.ContainsKey(day))
                {
                    heatDemandData[day] = new List<(double, double)>();
                }

                heatDemandData[day].Add((heatDemandSelector(item).Value, priceSelector(item) ?? 0));
            }
        }

        foreach (var day in days)
        {
            optimizedDataDict[day] = new List<Optimizer.hourData>();
        }

        foreach (var day in heatDemandData.Keys)
        {
            var data = heatDemandData[day];
            optimizedDataDict[day] = new List<Optimizer.hourData>();

            foreach (var hour in data)
            {
                var hourData = usePrice
                    ? optimizeWithPrice(scenarioBoilers.Select(x => x.Deepcopy()).ToList(), hour.Item1, hour.Item2)
                    : optimizeWithoutPrice(scenarioBoilers.Select(x => x.Deepcopy()).ToList(), hour.Item1);
                optimizedDataDict[day].Add(hourData);
            }
        }


        return heatDemandData;


    }

    public Dictionary<int, List<(double, double)>> GetSummerHeatDemandData1()
    {
        var optimizer = new Optimizer.Optimizer1();
        var summerList = new FileReader().WriteList<Summer>();
        return GetHeatDemandData<Summer, Summer>(
            s => s.STimeFrom,
            s => s.SHeatDemand,
            s => s.SPrice,
            summerList,
            (boilers, demand, price) => optimizer.OptimizeHour(boilers, demand),
            (boilers, demand) => optimizer.OptimizeHour(boilers, demand),
            false,
            SummerDays,
            _summerOptimizedData,
            _boilersScenario1
        );
    }

    public Dictionary<int, List<(double, double)>> GetSummerHeatDemandData2()
    {
        var optimizer = new Optimizer.Optimizer2();
        var summerList = new FileReader().WriteList<Summer>();
        return GetHeatDemandData<Summer, Summer>(
            s => s.STimeFrom,
            s => s.SHeatDemand,
            s => s.SPrice,
            summerList,
            (boilers, demand, price) => optimizer.OptimizeHour(boilers, demand, price),
            (boilers, demand) => optimizer.OptimizeHour(boilers, demand, 0),
            true,
            SummerDays,
            _summerOptimizedData2,
            _boilersScenario2
        );
    }

    public Dictionary<int, List<(double, double)>> GetWinterHeatDemandData1()
    {
        var optimizer = new Optimizer.Optimizer1();
        var winterList = new FileReader().WriteList<Winter>();
        return GetHeatDemandData<Winter, Winter>(
            w => w.WTimeFrom,
            w => w.WHeatDemand,
            w => w.WPrice,
            winterList,
            (boilers, demand, price) => optimizer.OptimizeHour(boilers, demand),
            (boilers, demand) => optimizer.OptimizeHour(boilers, demand),
            false,
            WinterDays,
            _winterOptimizedData,
            _boilersScenario1
        );
    }

    public Dictionary<int, List<(double, double)>> GetWinterHeatDemandData2()
    {
        var optimizer = new Optimizer.Optimizer2();
        var winterList = new FileReader().WriteList<Winter>();
        return GetHeatDemandData<Winter, Winter>(
            w => w.WTimeFrom,
            w => w.WHeatDemand,
            w => w.WPrice,
            winterList,
            (boilers, demand, price) => optimizer.OptimizeHour(boilers, demand, price),
            (boilers, demand) => optimizer.OptimizeHour(boilers, demand, 0),
            true,
            WinterDays,
            _winterOptimizedData2,
            _boilersScenario2
        );
    }

    private void UpdateGraph(
    Dictionary<int, List<(double, double)>> heatDemandDataDict,
    Dictionary<int, List<Optimizer.hourData>> optimizedDataDict,
    int selectedDay,
    Action<ISeries[]> setSeries,
    Action<ISeries[]> setCostCO2Series,
    Action<ISeries[]> setPriceSeries,
    string demandSeriesName,
    ObservableCollection<Boiler> scenarioBoilers)
    {
        if (heatDemandDataDict.ContainsKey(selectedDay))
        {
            var heatDemandData = heatDemandDataDict[selectedDay];
            var hoursData = optimizedDataDict[selectedDay];
            Dictionary<string, List<double>> boilersData = new();

            foreach (var boiler in scenarioBoilers)
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

            var seriesList = new List<ISeries>();
            var costCO2SeriesList = new List<ISeries>();
            var epriceseriesList = new List<ISeries>();

            foreach (var boiler in filteredBoilersData)
            {
                seriesList.Add(
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
            List<double> priceData = new();
            foreach (var hour in heatDemandData)
            {
                heatDemandData2.Add(hour.Item1);
                priceData.Add(hour.Item2);
            }
            seriesList.Add(
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
                    Name = demandSeriesName
                }
            );

            List<double> summedCO2 = new();
            List<double> summedCost = new();
            List<double> preOptCostList = new();

            foreach (var hour in heatDemandData)
            {
                double demand = hour.Item1;
                // Example: use the most expensive boiler for this scenario
                double avgProdCost = scenarioBoilers.Average(b => b.ProdCostPerMWh);
                double preOptCost = demand * avgProdCost;
                preOptCostList.Add(Math.Round(preOptCost));
            }

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

            costCO2SeriesList.Add(
                new ColumnSeries<double>
                {
                    Values = summedCO2,
                    Fill = new LiveChartsCore.SkiaSharpView.Painting.SolidColorPaint
                    {
                        Color = new SkiaSharp.SKColor(255, 0, 0)
                    },
                    Name = "CO2 Emission(Kg)"
                });
            costCO2SeriesList.Add(
                new ColumnSeries<double>
                {
                    Values = summedCost,
                    Fill = new LiveChartsCore.SkiaSharpView.Painting.SolidColorPaint
                    {
                        Color = new SkiaSharp.SKColor(0, 0, 255)
                    },
                    Name = "Cost(DKK)"
                });


            epriceseriesList.Add(
                new LineSeries<double>
                {
                    Values = priceData.ToArray(),
                    GeometrySize = 0,
                    LineSmoothness = 0,
                    Stroke = new LiveChartsCore.SkiaSharpView.Painting.SolidColorPaint
                    {
                        Color = new SkiaSharp.SKColor(255, 0, 255), // Magenta
                        StrokeThickness = 3
                    },
                    Fill = null,
                    Name = "Electricity Price (DKK)"
                }
            );
            costCO2SeriesList.Add(
    new LineSeries<double>
    {
        Values = preOptCostList,
        LineSmoothness = 0,
        Stroke = new LiveChartsCore.SkiaSharpView.Painting.SolidColorPaint
        {
            Color = new SkiaSharp.SKColor(255, 128, 0), // Orange
            StrokeThickness = 3
        },
        Fill = null,
        Name = "Cost Before Optimization (DKK)"
    });
            setSeries(seriesList.ToArray());
            setCostCO2Series(costCO2SeriesList.ToArray());
            setPriceSeries(epriceseriesList.ToArray());
        }
    }

    // Summer Scenario 1
    private void UpdateSummerGraphScenario1()
    {
        UpdateGraph(
            _summerHeatDemandData,
            _summerOptimizedData,
            SelectedSummerDay,
            series => SummerSeries = series,
            series => SummerCostCO2Series = series,
            series => SummerEPriceSeries = series,
            "Heat Demand",
            _boilersScenario1
        );
    }

    // Summer Scenario 2
    private void UpdateSummerGraphScenario2()
    {
        UpdateGraph(
            _summerHeatDemandData2,
            _summerOptimizedData2,
            SelectedSummerDay,
            series => SummerSeries = series,
            series => SummerCostCO2Series = series,
            series => SummerEPriceSeries = series,
            "Heat Demand",
            _boilersScenario2
        );
    }

    // Winter Scenario 1
    private void UpdateWinterGraphScenario1()
    {
        UpdateGraph(
            _winterHeatDemandData,
            _winterOptimizedData,
            SelectedWinterDay,
            series => WinterSeries = series,
            series => WinterCostCO2Series = series,
            series => WinterEPriceSeries = series,
            "Heat Demand",
            _boilersScenario1
        );
    }

    // Winter Scenario 2
    private void UpdateWinterGraphScenario2()
    {
        UpdateGraph(
            _winterHeatDemandData2,
            _winterOptimizedData2,
            SelectedWinterDay,
            series => WinterSeries = series,
            series => WinterCostCO2Series = series,
            series => WinterEPriceSeries = series,
            "Heat Demand",
            _boilersScenario2
        );

    }

    private void UpdateSummerGraphByScenario()
    {
        if (CurrentSummerScenario == 1)
            UpdateSummerGraphScenario1();
        else
            UpdateSummerGraphScenario2();
    }

    private void UpdateWinterGraphByScenario()
    {
        if (CurrentWinterScenario == 1)
            UpdateWinterGraphScenario1();
        else
            UpdateWinterGraphScenario2();
    }


    private void SaveOptimizerResultsToCsv(IEnumerable<Optimizer.hourData> results)
    {
        var fileWriter = new FileWriter();
        fileWriter.WriteResults(results);
    }

    public void SaveAllOptimizerResultsToCsv()
    {
        // Combine all hourData from all days for summer and winter (both scenarios if needed)
        var allResults = new List<Optimizer.hourData>();

        // Add all summer results (scenario 1)
        foreach (var dayResults in _summerOptimizedData.Values)
            allResults.AddRange(dayResults);

        // Add all summer results (scenario 2)
        foreach (var dayResults in _summerOptimizedData2.Values)
            allResults.AddRange(dayResults);

        // Add all winter results (scenario 1)
        foreach (var dayResults in _winterOptimizedData.Values)
            allResults.AddRange(dayResults);

        // Add all winter results (scenario 2)
        foreach (var dayResults in _winterOptimizedData2.Values)
            allResults.AddRange(dayResults);

        SaveOptimizerResultsToCsv(allResults);
    }

    public IEnumerable<BoilerHourResult> GetBoilerHourResults()
    {
        var results = new List<BoilerHourResult>();

        // Helper to add results from a scenario
        void AddScenarioResults(Dictionary<int, List<Optimizer.hourData>> optimizedData, string scenarioName)
        {
            foreach (var dayEntry in optimizedData)
            {
                int day = dayEntry.Key;
                var hours = dayEntry.Value;
                for (int hourIndex = 0; hourIndex < hours.Count; hourIndex++)
                {
                    var hourData = hours[hourIndex];
                    double demand = hourData.Demand; // Make sure this property exists in hourData
                    foreach (var boiler in hourData.Boilers)
                    {
                        results.Add(new BoilerHourResult
                        {
                            Day = day,
                            Hour = hourIndex,
                            BoilerName = boiler.Name,
                            HeatProduced = boiler.HeatProduced,
                            CO2Produced = boiler.CO2Produced,
                            Cost = boiler.Cost,
                            Scenario = scenarioName,
                            Demand = demand
                        });
                    }
                }
            }
        }

        AddScenarioResults(_summerOptimizedData, "SummerScenario1");
        AddScenarioResults(_summerOptimizedData2, "SummerScenario2");
        AddScenarioResults(_winterOptimizedData, "WinterScenario1");
        AddScenarioResults(_winterOptimizedData2, "WinterScenario2");

        return results;
    }
    private void SaveBoilerDistributionToCsv()
    {
        var fileWriter = new FileWriter();
        string downloadsPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            "Downloads",
            "HPOBoilerDistribution.csv"
        );
        var results = GetBoilerHourResults(); // This method should flatten your data as described earlier
        fileWriter.WriteBoilerHourResults(results, downloadsPath);
    }
    


    /*private void ResultDataManager(Dictionary<int, List<Optimizer.hourData>> optimizedData)
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
    }*/
}