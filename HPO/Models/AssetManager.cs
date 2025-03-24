using System;
using System.Collections.ObjectModel;

namespace HPO.Models;

public class Boiler
{
    public string Name { get; set; }
    public float Heat { get; set; }
    public float ProdCost { get; set; }
    public float CO2Emission { get; set; }
    public float GasConsumption { get; set; }
    public float OilConsumption { get; set; }

    public Boiler(string name, float maxHeat, float prodCost, float co2Emission, float gasConsumption)
    {
        Name = name;
        Heat = maxHeat;
        ProdCost = prodCost;
        CO2Emission = co2Emission;
        GasConsumption = gasConsumption;
    }
}
public class GasBoiler : Boiler
{
    public GasBoiler(string name, float heat, float prodCost, float co2Emission, float gasConsumption) : base(name, heat, prodCost, co2Emission, gasConsumption)
    {

    }
}

public class OilBoiler : Boiler
{
    public OilBoiler(string name, float heat, float prodCost, float co2Emission, float oilConsumption) : base(name, heat, prodCost, co2Emission, oilConsumption)
    {
    }
}


