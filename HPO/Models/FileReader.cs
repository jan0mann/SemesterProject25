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

public class FileReader
{

    public List<Season> WriteList<T>()
    {
        string basePath = AppDomain.CurrentDomain.BaseDirectory;
        string filePath = Path.Combine(basePath, "Assets", "HPOInfo.csv");

        using (var reader = new StreamReader(filePath))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            return csv.GetRecords<T>().ToList().ConvertToSeason();
        }
    }

}

public static class SeasonExtensions
{
    public static List<Season> ConvertToSeason<T>(this List<T> data)
    {
        List<Season> converted = new List<Season>();
        if (typeof(T) == typeof(Winter))
        {
            foreach (var hour in data)
            {
                var winterHour = hour as Winter;
                converted.Add(new Season(winterHour.WTimeFrom, winterHour.WHourFrom, winterHour.WTimeTo, winterHour.WHourTo, winterHour.WHeatDemand, winterHour.WPrice));
            }
        }
        else
        { 
           foreach (var hour in data)
            {
                var winterHour = hour as Summer;
                converted.Add(new Season(winterHour.STimeFrom, winterHour.SHourFrom, winterHour.STimeTo, winterHour.SHourTo, winterHour.SHeatDemand, winterHour.SPrice));
            } 
        }
        return converted;
    }
}
