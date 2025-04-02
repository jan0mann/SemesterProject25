using System;
using System.Collections.ObjectModel;

namespace HPO.Models;

public abstract class Boiler
{
    public string Name { get; set; }
    public float MaxHeat { get; set; }
    public float ProdCost { get; set; }
    public float CO2Emission { get; set; }
    public float Consumption { get; set; }

    public Boiler(string name, float maxHeat, float prodCost, float co2Emission, float consumption)
    {
        Name = name;
        MaxHeat = maxHeat;
        ProdCost = prodCost;
        CO2Emission = co2Emission;
        Consumption = consumption;
    }
}
public class GasBoiler : Boiler
{
    public GasBoiler(string name, float heat, float prodCost, float co2Emission, float consumption) : base(name, heat, prodCost, co2Emission, consumption)
    {

    }
}

public class OilBoiler : Boiler
{
    public OilBoiler(string name, float heat, float prodCost, float co2Emission, float consumption) : base(name, heat, prodCost, co2Emission, consumption)
    {
    }
}


