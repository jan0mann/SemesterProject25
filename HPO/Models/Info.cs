namespace HPO.Models;

public class Winter
{
    public string? WTimeFrom { get; set;}

    public string? WTimeTo { get; set;}

    public double? WHeatDemand { get; set;}

    public double? WPrice { get; set;}

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
   public string? STimeFrom { get; set;}

    public string? STimeTo { get; set;}

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

public class Starter
{
public void MakinFunction()
{
    Winter winter = new Winter("23/07/2024","27/08/2024",23.2,524.56);
    Summer summer = new Summer("25/08/2024","29/09/2024",35.2,213.56);
}

}


/*
public class Info
{
    public string? WinterTimeFrom { get; set; }

    public string? WinterTimeTo { get; set; }

    public float Winter_Heat_Demand { get; set; }

    public float Winter_Electricity_Price { get; set; }

    public string? SummerTimeFrom { get; set; }

    public string? SummerTimeTo { get; set; }

    public float Summer_Heat_Demand { get; set; }

    public float Summer_Electricity_Price { get; set; }

    public Info(string wintifro, string wintito, float winheatde, float winelpr, string summtifro, string summtito, float summheatde, float summelpr)
    {

    }


}

*/