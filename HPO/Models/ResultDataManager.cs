using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using HPO.Optimizer;

namespace HPO.Optimizer
{
    public static class ResultDataManager
    {
        private static readonly string resultFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ResultData");

        public static void SaveBoilerResultsToCsv(List<hourData> hourlyDataList, string dayType, int day)
        {
            if (!Directory.Exists(resultFolder))
            {
                Directory.CreateDirectory(resultFolder);
            }

            string fileName = $"{dayType}_Day_{day}_Results.csv";
            string filePath = Path.Combine(resultFolder, fileName);

            using (var writer = new StreamWriter(filePath, false))
            {
                // Header
                writer.WriteLine("Hour,BoilerName,BoilerType,HeatProduced,Cost,CO2Produced");

                for (int hour = 0; hour < hourlyDataList.Count; hour++)
                {
                    var hourData = hourlyDataList[hour];

                    foreach (var boiler in hourData.Boilers)
                    {
                        writer.WriteLine($"{hour}," +
                                         $"{boiler.Name}," +
                                         $"{boiler.BoilerType}," +
                                         $"{boiler.HeatProduced.ToString(CultureInfo.InvariantCulture)}," +
                                         $"{boiler.Cost.ToString(CultureInfo.InvariantCulture)}," +
                                         $"{boiler.CO2Produced.ToString(CultureInfo.InvariantCulture)}");
                    }
                }
            }
        }
    }
}
