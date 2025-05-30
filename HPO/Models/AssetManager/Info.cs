using System.Collections.Generic;

namespace HPO.Models;

public class Winter
{
    public string? WTimeFrom { get; set; }
    public string? WHourFrom { get; set; }
    public string? WTimeTo { get; set; }
    public string? WHourTo { get; set; }

    public double? WHeatDemand { get; set; }

    public double? WPrice { get; set; }
    public Winter() { }

    public Winter(string? timeFrom, string? timeTo, double? heatDemand, double? price)
    {
        this.WTimeFrom = timeFrom;
        this.WTimeTo = timeTo;
        this.WHeatDemand = heatDemand;
        this.WPrice = price;
    }
}

public class Summer
{
    public string? STimeFrom { get; set; }
    public string? SHourFrom { get; set; }
    public string? STimeTo { get; set; }
    public string? SHourTo { get; set; }

    public double? SHeatDemand { get; set; }

    public double? SPrice { get; set; }

    public Summer() { }

    public Summer(string? timeFrom, string? timeTo, double? heatDemand, double? price)
    {
        this.STimeFrom = timeFrom;
        this.STimeTo = timeTo;
        this.SHeatDemand = heatDemand;
        this.SPrice = price;
    }
}


public class Season
{
    public string? TimeFrom { get; set; }
    public string? HourFrom { get; set; }
    public string? TimeTo { get; set; }
    public string? HourTo { get; set; }

    public double? HeatDemand { get; set; }

    public double? Price { get; set; }

    public Season() { }

    public Season(string? timeFrom, string? timeTo, double? heatDemand, double? price)
    {
        this.TimeFrom = timeFrom;
        this.TimeTo = timeTo;
        this.HeatDemand = heatDemand;
        this.Price = price;
    }

    public Season(string? timeFrom, string? hourFrom, string? timeTo, string? hourTo, double? heatDemand, double? price)
    {
        this.TimeFrom = timeFrom;
        this.TimeTo = timeTo;
        this.HourFrom = hourFrom;
        this.HourTo = hourTo;
        this.HeatDemand = heatDemand;
        this.Price = price;
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