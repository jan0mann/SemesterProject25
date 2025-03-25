using System;
using System.Collections.ObjectModel;

namespace HPO.Models;

public class Boiler
{
    public string Name { get; set; }
    public float Heat { get; set; }
    public float ProdCost { get; set; }
    public float CO2Emission { get; set; }

    public Boiler(string name, float heat, float prodCost, float co2Emission)
    {
        Name = name;
        Heat = heat;
        ProdCost = prodCost;
        CO2Emission = co2Emission;
    }
}
public class GasBoiler : Boiler
{
    public GasBoiler(string name, float heat, float prodCost, float co2Emission) : base(name, heat, prodCost, co2Emission)
    {

    }
}

public class OilBoiler : Boiler
{
    public OilBoiler(string name, float heat, float prodCost, float co2Emission) : base(name, heat, prodCost, co2Emission)
    {
    }
}


