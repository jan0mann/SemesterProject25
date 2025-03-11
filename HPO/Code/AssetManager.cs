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

    internal ProductionUnit(string Name, float Heat, float ProdCost, float CO2Emission, float FuelCons)
        {
            this.name = Name;
            this.heat = Heat;
            this.prod_cost = ProdCost;
            this.co2_emission = CO2Emission;
            this.cons_prim_energy = FuelCons;
        }
}

internal class GasBoiler : ProductionUnit

{
    internal GasBoiler(string Name, float Heat, float ProdCost, float CO2Emission, float GasCons)
        : base(Name, Heat, ProdCost, CO2Emission, GasCons) { }
}

internal class OilBoiler : ProductionUnit

{
    internal OilBoiler(string Name, float Heat, float ProdCost, float CO2Emission, float OilCons)
        : base(Name, Heat, ProdCost, CO2Emission, OilCons) { }
}

class Program
{
    void Main()
    {
        GasBoiler gasboiler01 = new GasBoiler("GB1", 4.0f, 520f, 175f, 0.9f);
        OilBoiler oilboiler01 = new OilBoiler("OB1", 4.0f, 670f, 330f, 1.5f);
        
    }
}
