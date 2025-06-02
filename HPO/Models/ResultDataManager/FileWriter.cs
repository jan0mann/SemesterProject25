using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System;
using Tmds.DBus.Protocol;

namespace HPO.Models;

public class FileWriter
{

    public void WriteResults<T>(IEnumerable<T> results)
    {
        string downloadsPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            "Downloads"
        );
        string filePath = Path.Combine(downloadsPath, "HPOResults.csv");

        Directory.CreateDirectory(Path.GetDirectoryName(filePath));

        using (var writer = new StreamWriter(filePath))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(results);
        }
    }

        public void WriteBoilerHourResults(IEnumerable<BoilerHourResult> results, string filePath)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            using (var writer = new StreamWriter(filePath))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(results);
            }
        }
}



public class BoilerHourResult
{
    public int Day { get; set; }
    public int Hour { get; set; }
    public string? BoilerName { get; set; }
    public double HeatProduced { get; set; }
    public double CO2Produced { get; set; }
    public double Cost { get; set; }
    public string? Scenario { get; set; }
    public double Demand { get; set; }
    public string? PrimaryEnergy { get; set; }
    }

