using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System;

namespace HPO.Models;

public class FileReader
{
public void ReadInfo()
    {
        List<Winter> winterList = new();
        using (var reader = new StreamReader("HPOInfo.csv"))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            winterList = csv.GetRecords<Winter>().ToList();
        }



        foreach (var winter in winterList)
        {

            Console.WriteLine(winter.WTimeFrom);
            


        }
    }
}