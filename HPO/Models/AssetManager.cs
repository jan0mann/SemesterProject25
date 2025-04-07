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
    public double HeatProduced {get; set;}// = 0.0; // used only for calculations in optimizer

    public double requestProduction(double necessaryHeat){
        if(necessaryHeat > MaxHeat){
            HeatProduced = MaxHeat;
            return MaxHeat;
        }else{
            HeatProduced = necessaryHeat;
            return necessaryHeat;
        }

    }

    public Boiler(string name, float maxHeat, float prodCost, float co2Emission, float consumption, double heatProduced)
    {
        Name = name;
        MaxHeat = maxHeat;
        ProdCost = prodCost;
        CO2Emission = co2Emission;
        Consumption = consumption;
        HeatProduced = heatProduced;
    }

    public abstract Boiler deepcopy();
}
public class GasBoiler : Boiler
{
    public GasBoiler(string name, float heat, float prodCost, float co2Emission, float consumption, double heatProduced = 0.0) : base(name, heat, prodCost, co2Emission, consumption, heatProduced)
    {
    }

    public override GasBoiler deepcopy(){
        return new GasBoiler(Name, MaxHeat, ProdCost, CO2Emission, Consumption, HeatProduced);
    }
}

public class OilBoiler : Boiler
{
    public OilBoiler(string name, float heat, float prodCost, float co2Emission, float consumption, double heatProduced = 0.0) : base(name, heat, prodCost, co2Emission, consumption, heatProduced)
    {
    }
    public override OilBoiler deepcopy(){
        return new OilBoiler(Name, MaxHeat, ProdCost, CO2Emission, Consumption, HeatProduced);
    }
}


