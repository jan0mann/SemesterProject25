using System;
using CommunityToolkit.Mvvm.ComponentModel;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

namespace HPO.ViewModels;

public partial class HeatDemandViewModel : ViewModelBase
{
    private const string CsvFilePath = "Assets/HeatDemand.csv";
    public ISeries[] Series { get; set; }

    public Axis[] YAxes { get; set; }
            = new Axis[]
            {
                new Axis
                {
                    Name = "Heat Demand in MW/h",
                }
            };


    public HeatDemandViewModel()
    {
        LoadHeatDemandData();
    }

    private void LoadHeatDemandData()
    {
        var heatDemandData = new List<double>();
        using (var reader = new StreamReader(CsvFilePath))
        {
            // Skip the header line
            reader.ReadLine();
            int lineCount = 0;
            while (!reader.EndOfStream && lineCount < 24)
            {
                var line = reader.ReadLine();
                var values = line.Split(',');
                if (values.Length >= 3 && double.TryParse(values[2], NumberStyles.Any, CultureInfo.InvariantCulture, out double demand))
                {
                    heatDemandData.Add(demand);
                    lineCount++;
                }
            }
        }

        Series = new ISeries[]
        {
            new LineSeries<double>
            {
                Values = heatDemandData.ToArray(),
                Fill = null,
                GeometrySize = 0,
                LineSmoothness = 0
            }
        };
    }
}


