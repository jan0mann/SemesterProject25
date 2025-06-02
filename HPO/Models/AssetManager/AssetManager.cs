using System;
using System.Collections.ObjectModel;
using LiveChartsCore.SkiaSharpView.Drawing.Layouts;

namespace HPO.Models;

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
    public string Name { get;}
    public double MaxHeat { get;}
    public double ProdCostPerMWh { get; set; }
    public double CO2EmissionPerMWh { get;}
    public double ConsumptionPerMWh { get;}
    public double HeatProduced { get; private set;} 
    public double CO2Produced { get; private set;}
    public double Consumed { get; private set;}
    public double Cost { get; private set; }
    public double MaxElectricity { get; }
    public double ElecProduced { get; private set; }
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
        return HeatProduced;
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