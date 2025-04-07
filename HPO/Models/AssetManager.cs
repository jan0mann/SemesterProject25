using System;
using System.Collections.ObjectModel;

namespace HPO.Models;

// public abstract class Boiler
// {
//     public string Name { get; set; }
//     public float MaxHeat { get; set; }
//     public float ProdCost { get; set; }
//     public float CO2Emission { get; set; }
//     public float Consumption { get; set; }
//     public double HeatProduced {get; set;} // used only for calculations in optimizer

//     public double requestProduction(double necessaryHeat){
//         if(necessaryHeat > MaxHeat){
//             HeatProduced = MaxHeat;
//             return MaxHeat;
//         }else{
//             HeatProduced = necessaryHeat;
//             return necessaryHeat;
//         }

//     }

//     public Boiler(string name, float maxHeat, float prodCost, float co2Emission, float consumption, double heatProduced)
//     {
//         Name = name;
//         MaxHeat = maxHeat;
//         ProdCost = prodCost;
//         CO2Emission = co2Emission;
//         Consumption = consumption;
//         HeatProduced = heatProduced;
//     }

//     public abstract Boiler Deepcopy();
// }
// public class GasBoiler : Boiler
// {
//     public GasBoiler(string name, float heat, float prodCost, float co2Emission, float consumption, double heatProduced = 0.0) : base(name, heat, prodCost, co2Emission, consumption, heatProduced)
//     {
//     }

//     public override GasBoiler Deepcopy(){
//         return new GasBoiler(Name, MaxHeat, ProdCost, CO2Emission, Consumption, HeatProduced);
//     }
// }

// public class OilBoiler : Boiler
// {
//     public OilBoiler(string name, float heat, float prodCost, float co2Emission, float consumption, double heatProduced = 0.0) : base(name, heat, prodCost, co2Emission, consumption, heatProduced)
//     {
//     }
//     public override OilBoiler Deepcopy(){
//         return new OilBoiler(Name, MaxHeat, ProdCost, CO2Emission, Consumption, HeatProduced);
//     }
// }


public enum BoilerType{
    Gas,
    Oil,
    HeatPump,
    GasMotor
}

public class Boiler
{
    public string Name { get; set; }
    public float MaxHeat { get; set; }
    public float ProdCostPerMWh { get; set; }
    public float CO2EmissionPerMWh { get; set; }
    public float ConsumptionPerMWh { get; set; }
    public double HeatProduced {get; set;} // used only for calculations in optimizer
    public double CO2Produced {get;set;}
    public double Consumed {get; set;}
    public double Cost {get;set;}

    public BoilerType BoilerType {get; private set;}

    public double requestProduction(double necessaryHeat){
        if(necessaryHeat > MaxHeat){
            HeatProduced = MaxHeat;
            CO2Produced = CO2EmissionPerMWh*HeatProduced;
            Consumed = ConsumptionPerMWh*HeatProduced;
            Cost = ProdCostPerMWh*HeatProduced;
            return MaxHeat; 
        }else{
            HeatProduced = necessaryHeat;
            CO2Produced = CO2EmissionPerMWh*HeatProduced;
            Consumed = ConsumptionPerMWh*HeatProduced;
            Cost = ProdCostPerMWh*HeatProduced;
            return necessaryHeat;
        }
    }

    public Boiler(string name, BoilerType boilerType, float maxHeat, float prodCost, float co2Emission, float consumption)
    {
        Name = name;
        BoilerType = boilerType;
        MaxHeat = maxHeat;
        ProdCostPerMWh = prodCost;
        CO2EmissionPerMWh = co2Emission;
        ConsumptionPerMWh = consumption;
        HeatProduced = 0.0;
        CO2Produced = 0.0;
        Consumed = 0.0;
        Cost = 0.0;
    }

    //only for testing purposes
    public Boiler(string name, BoilerType boilerType, float maxHeat, float prodCost, float co2Emission, float consumption, double heatProduced)
    {
        Name = name;
        BoilerType = boilerType;
        MaxHeat = maxHeat;
        ProdCostPerMWh = prodCost;
        CO2EmissionPerMWh = co2Emission;
        ConsumptionPerMWh = consumption;
        HeatProduced = heatProduced;
        CO2Produced = 0.0;
        Consumed = 0.0;
        Cost = 0.0;
    }

    public Boiler Deepcopy(){
        return new Boiler(Name, BoilerType, MaxHeat, ProdCostPerMWh, CO2EmissionPerMWh, ConsumptionPerMWh);
    }
}