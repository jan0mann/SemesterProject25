namespace HPO.Models;

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