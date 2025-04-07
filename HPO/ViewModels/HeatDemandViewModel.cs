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

    public Axis[] YAxes { get; set; }
        = new Axis[]
        {
            new Axis
            {
                Name = "Heat Demand in MW/h",
            }
        };

    private Dictionary<int, List<double>> _winterHeatDemandData;
    private Dictionary<int, List<double>> _summerHeatDemandData;


    private Dictionary<int, List<Optimizer.hourData>> _winterOptimizedData;
    private ObservableCollection<Boiler> _boilers;


    public HeatDemandViewModel()
    {
        SummerDays = new ObservableCollection<int>(Enumerable.Range(11, 14));
        WinterDays = new ObservableCollection<int>(Enumerable.Range(1, 14));


        _winterOptimizedData = new Dictionary<int, List<Optimizer.hourData>>();
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

    }

    public Dictionary<int, List<double>> GetWinterHeatDemandData()
    {
        var optimizer = new Optimizer.Optimizer();
        var winterList = new FileReader().WriteList<Winter>();
        var winterHeatDemandData = new Dictionary<int, List<double>>();

        foreach (var winter in winterList)
        {
            if (DateTime.TryParse(winter.WTimeFrom, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime wTimeFrom) &&
                winter.WHeatDemand.HasValue)
            {
                int day = wTimeFrom.Day;

                if (!winterHeatDemandData.ContainsKey(day))
                {
                    winterHeatDemandData[day] = new List<double>();
                }

                winterHeatDemandData[day].Add(winter.WHeatDemand.Value);
            }
        }

        foreach (var day in winterHeatDemandData.Keys)
        {
            var data = winterHeatDemandData[day];
            _winterOptimizedData[day] = new List<Optimizer.hourData>();

            foreach (var hour in data)
            {
                var hourdata = optimizer.OptimizeHour(_boilers.Select(x => x.Deepcopy()).ToList(), hour);
                _winterOptimizedData[day].Add(hourdata);
            }
        }

        return winterHeatDemandData;
    }

    public Dictionary<int, List<double>> GetSummerHeatDemandData()
    {
        var summerList = new FileReader().WriteList<Summer>();

        var summerHeatDemandData = new Dictionary<int, List<double>>();

        foreach (var summer in summerList)
        {
            if (DateTime.TryParse(summer.STimeFrom, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime sTimeFrom) &&
                summer.SHeatDemand.HasValue)
            {
                int day = sTimeFrom.Day;

                if (!summerHeatDemandData.ContainsKey(day))
                {
                    summerHeatDemandData[day] = new List<double>();
                }

                summerHeatDemandData[day].Add(summer.SHeatDemand.Value);
            }
        }

        return summerHeatDemandData;
    }

    private void UpdateSummerGraph()
    {
        if (_summerHeatDemandData.ContainsKey(SelectedSummerDay))
        {
            var heatDemandData = _summerHeatDemandData[SelectedSummerDay];

            SummerSeries = new ISeries[]
            {
            new LineSeries<double>
            {
                Values = heatDemandData.ToArray(),
                Fill = null,
                GeometrySize = 0,
                LineSmoothness = 0
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
            var hoursData = _winterOptimizedData[SelectedWinterDay];
            Dictionary<string, List<double>> boilersData = new();

            foreach (var boiler in _boilers)
            {
                boilersData[boiler.Name] = new List<double>();
            }


            //convert
            foreach (var hour in hoursData)
            {
                foreach (var boiler in hour.Boilers)
                {
                    //Console.WriteLine(boiler.HeatProduced);
                    boilersData[boiler.Name].Add(boiler.HeatProduced);
                }
            }



            var winterSeriesList = new List<ISeries>
            {
                new LineSeries<double>
                {
                    Values = heatDemandData.ToArray(),
                    Fill = null,
                    GeometrySize = 0,
                    LineSmoothness = 0,
                    Name = "Heat Demand"
                }
            };
            foreach (var boiler in boilersData)
            {
                winterSeriesList.Add(
                    new LineSeries<double>
                    {
                        Values = boiler.Value.ToArray(),
                        Fill = null,
                        GeometrySize = 0,
                        LineSmoothness = 0,
                        Name = boiler.Key
                    }
                );
                //foreach(var val in boiler.Value){Console.WriteLine(val);}
            }


            //just for easier tesing, to check if sum of boilers produced is equal to requested
            List<double> summed = new();
            foreach (var hour in hoursData)
            {
                double sum = 0;
                foreach (var boiler in hour.Boilers)
                {
                    sum += boiler.HeatProduced;
                }
                summed.Add(sum);
            }
            winterSeriesList.Add(new LineSeries<double>
            {
                Values = summed,
                Fill = null,
                GeometrySize = 0,
                LineSmoothness = 0,
                Name = "Sum"
            });

            WinterSeries = winterSeriesList.ToArray();

            OnPropertyChanged(nameof(WinterSeries));
        }
    }
}