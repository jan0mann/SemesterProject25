using System;
using System.Collections.ObjectModel;
using LiveChartsCore.SkiaSharpView.Drawing.Layouts;

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


public enum BoilerType
{
    Gas,
    Oil,
    HeatPump,
    GasMotor
}

public class Boiler
{
    public string ConsumptionPerMWhDisplay => ConsumptionPerMWh.ToString("F2");
    public string Name { get; set; }
    public double MaxHeat { get; set; }
    public double ProdCostPerMWh { get; set; }
    public double CO2EmissionPerMWh { get; set; }
    public double ConsumptionPerMWh { get; set; }
    public double HeatProduced { get; set; } // used only for calculations in optimizer
    public double CO2Produced { get; set; }
    public double Consumed { get; set; }
    public double Cost { get; set; }
    public double MaxElectricity { get; set; }
    public double ElecProduced { get; set; }
    public BoilerType BoilerType { get; private set; }

    public double requestProduction(double necessaryHeat)
    {
        if (necessaryHeat > MaxHeat)
        {
            HeatProduced = MaxHeat;
        }
        else
        {
            HeatProduced = necessaryHeat;
        }

        CO2Produced = CO2EmissionPerMWh * HeatProduced;
        Consumed = ConsumptionPerMWh * HeatProduced;
        Cost = ProdCostPerMWh * HeatProduced;
        return MaxHeat;
    }
    public void sellElectricity(double elprice)
    {
        ElecProduced = MaxElectricity;
        CO2Produced = CO2EmissionPerMWh * ElecProduced;
        Consumed = ConsumptionPerMWh * ElecProduced;
        Cost = elprice * ElecProduced - ProdCostPerMWh * ElecProduced;
    }

    public Boiler(string name, BoilerType boilerType, double maxHeat, double prodCost, double co2Emission, double consumption, double maxElectricity)
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
        MaxElectricity = maxElectricity;
        ElecProduced = 0.0;
    }

    public Boiler Deepcopy()
    {
        return new Boiler(Name, BoilerType, MaxHeat, ProdCostPerMWh, CO2EmissionPerMWh, ConsumptionPerMWh, MaxElectricity);
    }

}