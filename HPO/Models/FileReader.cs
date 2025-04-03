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

    public void ReadInfo()
    {
        List<Winter> winterList = WriteList<Winter>();
        List<Summer> summerList = WriteList<Summer>();
    }

    public List<T> WriteList<T>()
    {
        using (var reader = new StreamReader(@"C:\HPO\SemesterProject25\HPO\Assets\HPOInfo.csv"))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            return csv.GetRecords<T>().ToList();

        }
    }

    public Dictionary<int, List<double>> GetWinterHeatDemandData()
    {
        var winterList = WriteList<Winter>();

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

        return winterHeatDemandData;
    }

    public Dictionary<int, List<double>> GetSummerHeatDemandData()
    {
        var summerList = WriteList<Summer>();

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
}