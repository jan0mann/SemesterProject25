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

    private Dictionary<int, List<double>> _winterHeatDemandData;
    private Dictionary<int, List<double>> _summerHeatDemandData;


    private Dictionary<int, List<Optimizer.Optimizer.hourData>> _winterOptimizedData;
    private ObservableCollection<Boiler> _boilers;


    public HeatDemandViewModel()
    {
        SummerDays = new ObservableCollection<int>(Enumerable.Range(11, 14));
        WinterDays = new ObservableCollection<int>(Enumerable.Range(1, 14));
        _winterHeatDemandData = new Dictionary<int, List<double>>();
        _summerHeatDemandData = new Dictionary<int, List<double>>();


        _winterOptimizedData = new Dictionary<int, List<Optimizer.Optimizer.hourData>>();
        var assetManager = new AssetManager();
        _boilers = assetManager.ShowInfo();

        LoadWinterHeatDemandData();
        LoadSummerHeatDemandData();
        SelectedSummerDay = 11;
        SelectedWinterDay = 1; 
    }
 
    private void LoadWinterHeatDemandData()
    {
        var optimizer = new Optimizer.Optimizer();
        List<Boiler> boilersList = [.. _boilers];

        using (var reader = new StreamReader(CsvFilePath))
        {
            var header = reader.ReadLine(); 
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(',');

                
                if (values.Length >= 5 &&
                    DateTime.TryParse(values[0], CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime wTimeFrom) &&
                    double.TryParse(values[4], NumberStyles.Any, CultureInfo.InvariantCulture, out double wHeatDemand))
                {
                    int day = wTimeFrom.Day;

                    if (!_winterHeatDemandData.ContainsKey(day))
                    {
                        _winterHeatDemandData[day] = new List<double>();
                        _winterOptimizedData[day] = new List<Optimizer.Optimizer.hourData>();
                    }

                    _winterHeatDemandData[day].Add(wHeatDemand);
                    var hourdata = optimizer.OptimizeHour(boilersList, wHeatDemand);
                    _winterOptimizedData[day].Add(hourdata.deepcopy());

                    //Console.WriteLine(hourdata.Demand+"  B1:"+hourdata.Boilers[0].HeatProduced+"  B2:"+hourdata.Boilers[1].HeatProduced+"  B3:"+hourdata.Boilers[2].HeatProduced);
                }
            }
        }



        //_winterOptimizedData = optimizer.Optimize(boilersList, _winterHeatDemandData.Values.SelectMany(x => x).ToList());

        UpdateWinterGraph();
    }

    private Boiler Boiler(string v1, int v2, int v3, int v4, int v5)
    {
        throw new NotImplementedException();
    }

    private void LoadSummerHeatDemandData()
    {
        using (var reader = new StreamReader(CsvFilePath))
        {
            var header = reader.ReadLine(); 

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(',');

                
                if (values.Length >= 11 &&
                    DateTime.TryParse(values[6], CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime sTimeFrom) &&
                    double.TryParse(values[10], NumberStyles.Any, CultureInfo.InvariantCulture, out double sHeatDemand))
                {
                    int day = sTimeFrom.Day;

                    if (!_summerHeatDemandData.ContainsKey(day))
                    {
                        _summerHeatDemandData[day] = new List<double>();
                    }

                    _summerHeatDemandData[day].Add(sHeatDemand);
                }
            }
        }

        UpdateSummerGraph();
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
           
            foreach (var boiler in _boilers){
                boilersData[boiler.Name] = new List<double>();
            }


            //convert
            foreach (var hour in hoursData){
                foreach (var boiler in hour.Boilers){
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
            foreach (var boiler in boilersData){
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


            WinterSeries = winterSeriesList.ToArray();

            OnPropertyChanged(nameof(WinterSeries));
        }
    }
}