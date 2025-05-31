using System;

using System.IO;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using HPO.Models;
using HPO.Optimizer;

namespace HPO.ViewModels;

public partial class HeatDemandViewModel : ViewModelBase
{


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

    public Dictionary<int, List<(double, double)>> GetHeatDemandData(int scenario, char season)
    {
        var optimizer = new Optimizer.Optimizer();
        List<Season> seasonList;

        Dictionary<int, List<hourData>> OptimizerData;
        ObservableCollection<Boiler> boilers;
        ObservableCollection<int> days;
        bool arg;

        if (season == 'w')
        {
            seasonList = new FileReader().WriteList<Winter>();
            days = WinterDays;
            OptimizerData = scenario == 1 ? _winterOptimizedData : _winterOptimizedData2;
        }
        else
        {
            seasonList = new FileReader().WriteList<Summer>();
            days = SummerDays;
            OptimizerData = scenario == 1 ? _summerOptimizedData : _summerOptimizedData2;
        }
        if (scenario == 1)
        {
            boilers = _boilersScenario1;
            arg = false;
        }
        else
        {
            boilers = _boilersScenario2;
            arg = true;
        }

        return GetHeatDemandData<Season, Season>(
            s => s.TimeFrom,
            s => s.HeatDemand,
            s => s.Price,
            seasonList,
            (boilers, demand, price) => optimizer.OptimizeHour(boilers, demand, price),
            (boilers, demand) => optimizer.OptimizeHour(boilers, demand, 0),
            arg,
            days,
            OptimizerData,
            boilers
        );
    }
}