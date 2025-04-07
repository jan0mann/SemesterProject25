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

    public List<T> WriteList<T>()
    {
        string basePath = AppDomain.CurrentDomain.BaseDirectory;
        string filePath = Path.Combine(basePath, "Assets", "HPOInfo.csv");

        using (var reader = new StreamReader(filePath))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            return csv.GetRecords<T>().ToList();

        }
    }


}
