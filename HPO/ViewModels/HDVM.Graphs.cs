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


            foreach (var boiler in hoursData[0].Boilers)
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
                        Color = new SkiaSharp.SKColor(255, 0, 255), 
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
            Color = new SkiaSharp.SKColor(255, 128, 0), 
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


    private void UpdateSummerGraphByScenario()
    {
        if (CurrentSummerScenario == 1)
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
        else
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

    private void UpdateWinterGraphByScenario()
    {
        if (CurrentWinterScenario == 1)
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
        else
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
}