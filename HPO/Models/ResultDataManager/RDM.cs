using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HPO.Optimizer;

namespace HPO.Models
{
    public static class RDM
    {
        public static void SaveScenarioBoilerDistributionToCsvMerged(
            int scenario,
            Dictionary<int, List<Optimizer.hourData>> summerOptimizedData,
            Dictionary<int, List<Optimizer.hourData>> winterOptimizedData,
            Dictionary<int, List<Optimizer.hourData>> summerOptimizedData2,
            Dictionary<int, List<Optimizer.hourData>> winterOptimizedData2)
        {
            var fileWriter = new FileWriter();
            string downloadsPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                "Downloads",
                scenario == 1 ? "HPOBoilerDistribution_Scenario1.csv" : "HPOBoilerDistribution_Scenario2.csv"
            );
            var results = scenario == 1
                ? GetBoilerHourResultsForScenario(summerOptimizedData, winterOptimizedData, "Scenario1")
                : GetBoilerHourResultsForScenario(summerOptimizedData2, winterOptimizedData2, "Scenario2");
            fileWriter.WriteBoilerHourResults(results, downloadsPath);
        }

        public static string GetPrimaryEnergyType(BoilerType type)
        {
            return type switch
            {
                BoilerType.Oil => "Oil",
                BoilerType.Gas => "Gas",
                BoilerType.GasMotor => "Gas",
                BoilerType.HeatPump => "Electricity",
                _ => "Unknown"
            };
        }

        public static IEnumerable<BoilerHourResult> GetBoilerHourResultsForScenario(
            Dictionary<int, List<Optimizer.hourData>> summerData,
            Dictionary<int, List<Optimizer.hourData>> winterData,
            string scenarioName)
        {
            var results = new List<BoilerHourResult>();
            void AddData(Dictionary<int, List<Optimizer.hourData>> data, string period)
            {
                foreach (var dayEntry in data)
                {
                    int day = dayEntry.Key;
                    var hours = dayEntry.Value;
                    for (int hourIndex = 0; hourIndex < hours.Count; hourIndex++)
                    {
                        var hourData = hours[hourIndex];
                        double demand = hourData.Demand;

                        // Calculate total heat produced per energy type for this hour
                        var energySums = new Dictionary<string, double>
                        {
                            { "Oil", 0 },
                            { "Gas", 0 },
                            { "Electricity", 0 }
                        };

                        foreach (var boiler in hourData.Boilers)
                        {
                            var energyType = GetPrimaryEnergyType(boiler.BoilerType);
                            if (energySums.ContainsKey(energyType))
                                energySums[energyType] += boiler.HeatProduced;
                        }

                        // Find the dominant energy type
                        var dominantEnergy = energySums.OrderByDescending(kv => kv.Value).First().Key;

                        // Add results for each boiler, but set PrimaryEnergy to dominant
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
                                Scenario = scenarioName + "_" + period,
                                Demand = demand,
                                PrimaryEnergy = dominantEnergy
                            });
                        }
                    }
                }
            }
            AddData(summerData, "Summer");
            AddData(winterData, "Winter");
            return results;
        }
    }
}