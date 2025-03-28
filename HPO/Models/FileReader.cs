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
    public List<Winter>? winterList;
    public List<Summer>? summerList;

public void ReadInfo()
    {
        winterList = WriteList<Winter>();
        summerList = WriteList<Summer>();
    }


    public List<T> WriteList<T>()
    {
        using (var reader = new StreamReader("HPOInfo.csv"))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            return csv.GetRecords<T>().ToList();
            
        }
    }


}