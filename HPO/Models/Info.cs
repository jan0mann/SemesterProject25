namespace HPO.Models;

public class Winter
{
    public string? WDateFrom { get; set;}
    public string? WHourFrom { get; set;}
    public string? WDateTo { get; set;}
    public string? WHourTo { get; set;}

    public double? WHeatDemand { get; set;}

    public double? WPrice { get; set;}

    public Winter(string? timeFrom, string? timeTo, double? heatDemand, double? price)
    {
        this.WDateFrom = timeFrom;
        this.WDateTo = timeTo;
        this.WHeatDemand = heatDemand;
        this.WPrice = price;
    }
}

public class Summer
{
   public string? STimeFrom { get; set;}
   public string? SHourFrom { get; set;}
    public string? STimeTo { get; set;}
    public string? SHourTo { get; set;}

    public double? SHeatDemand { get; set;}

    public double? SPrice { get; set;}

    public Summer(string? timeFrom, string? timeTo, double? heatDemand, double? price)
    {
        this.STimeFrom = timeFrom;
        this.STimeTo = timeTo;
        this.SHeatDemand = heatDemand;
        this.SPrice = price;
    }
}
