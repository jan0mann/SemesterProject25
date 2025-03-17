using System;

namespace AssetManager;

internal abstract class HeatingGrid

{
    internal string name {get; }

    internal HeatingGrid(string Name)
    {
        this.name = Name;
    }
}

abstract class ProductionUnit

{
    internal string name {get; set;}

    internal float heat {get; set;}

    internal float prod_energy {get; set;}

    internal float cons_prim_energy {get; set;}

    internal float prod_cost {get; set;}

    internal float co2_emission {get; set;}

    internal float electricity {get; set;}

    internal ProductionUnit(string Name, float Heat, float ProdCost, float CO2Emission, float FuelCons, float Electricity)
        {
            this.name = Name;
            this.heat = Heat;
            this.prod_cost = ProdCost;
            this.co2_emission = CO2Emission;
            this.cons_prim_energy = FuelCons;
            this.electricity = Electricity;
        }
}

internal class GasBoiler : ProductionUnit

{
    internal GasBoiler(string Name, float Heat, float ProdCost, float CO2Emission, float GasCons, float Electricity)
        : base(Name, Heat, ProdCost, CO2Emission, GasCons, Electricity) { }
}

internal class OilBoiler : ProductionUnit

{
    internal OilBoiler(string Name, float Heat, float ProdCost, float CO2Emission, float OilCons, float Electricity)
        : base(Name, Heat, ProdCost, CO2Emission, OilCons, Electricity) { }
}

// case 2:

internal class GasMotor : ProductionUnit

{
    internal GasMotor(string Name, float Heat, float ProdCost, float CO2Emission, float GasCons, float Electricity)
        : base(Name, Heat, ProdCost, CO2Emission, GasCons, Electricity) { }
}

internal class HeatPump : ProductionUnit

{
        internal HeatPump(string Name, float Heat, float ProdCost, float CO2Emission, float OilCons, float Electricity)
        : base(Name, Heat, ProdCost, CO2Emission, OilCons, Electricity) { }
}

class Program
{
    void Main()
    {
        GasBoiler gasboiler01 = new GasBoiler   ("GB1", 4.0f, 520f, 175f, 0.9f, 0f);
        GasBoiler gasboiler02 = new GasBoiler   ("GB2", 3.0f, 560f, 130f, 0.7f, 0f);
        OilBoiler oilboiler01 = new OilBoiler   ("OB1", 4.0f, 670f, 330f, 1.5f, 0f);
        GasMotor  gasmotor01  = new GasMotor    ("GM1", 3.5f, 990f, 650f, 1.8f, 2.6f);
        HeatPump  heatpump    = new HeatPump    ("HP1", 6.0f, 60f, 0f, 0f, -6.0f);
    }
}
