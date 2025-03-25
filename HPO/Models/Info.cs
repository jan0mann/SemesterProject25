namespace HPO.Models;

public abstract class Information
{
    public string? TimeFrom { get; set;}

    public string? TimeTo { get; set;}

    public double? HeatDemand { get; set;}

    public double? HeatPrice { get; set;}

    public Information(string? timeFrom, string? timeTo, double? heatDemand, double? heatPrice)
    {
        TimeFrom = timeFrom;
        TimeTo = timeTo;
        HeatDemand = heatDemand;
        HeatPrice = heatPrice;
    }
} 

public class Winter : Information
{
    public Winter(string? timeFrom, string? timeTo, double? heatDemand, double? heatPrice)
            : base(timeFrom, timeTo, heatDemand, heatPrice) { }
}

public class Summer : Information
{
    public Summer(string? timeFrom, string? timeTo, double? heatDemand, double? heatPrice)
            : base(timeFrom, timeTo, heatDemand, heatPrice) { }
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