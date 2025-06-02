using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using HPO.Models;
using HPO.Views;
using LiveChartsCore.VisualElements;

namespace HPO.Optimizer
{
    public class hourData
    {
        public string Date { get; set; }
        public string Time { get; set; }
        public double Demand { get; set; }
        public List<Boiler> Boilers { get; set; }
        public hourData(string date, string time, double demand, List<Boiler> boilers)
        {
            Date = date;
            Time = time;
            Demand = demand;
            Boilers = boilers;
        }

        public hourData()
        {
        }

        public hourData deepcopy()
        {
            var boilers = Boilers.Select(x => x.Deepcopy()).ToList();
            return new hourData(Date, Time, Demand,boilers);
        }
    }


    public class Optimizer
    {
        public List<Boiler> calculateEfficiency(List<Boiler> boilers, double elprice)
        {
        
            foreach (var boiler in boilers)
            {
                if (boiler.BoilerType == BoilerType.HeatPump)
                {
                    boiler.ProdCostPerMWh = elprice + 60; 
                }
                else if (boiler.BoilerType == BoilerType.GasMotor)
                {
                    boiler.ProdCostPerMWh -= elprice; 
                }
            }

            return boilers.OrderBy(x => x.ProdCostPerMWh).ToList();
        }

        public hourData OptimizeHour(List<Boiler> boilers, double demand, double elprice)
        {
            var efficiencyBoilers = calculateEfficiency(boilers, elprice);
            double remainingDemand = demand;
            int i = 0;

            
            while (remainingDemand > 0 && i < efficiencyBoilers.Count)
            {
                var boiler = efficiencyBoilers[i];

                if (boiler.BoilerType == BoilerType.HeatPump && boiler.ProdCostPerMWh <= elprice + 60)
                {
                    remainingDemand -= boiler.requestProduction(remainingDemand);
                }
                else if (boiler.BoilerType != BoilerType.HeatPump)
                {
                    remainingDemand -= boiler.requestProduction(remainingDemand);
                }

                i++;
            }

            
            var gasMotor = efficiencyBoilers.Find(x => x.BoilerType == BoilerType.GasMotor);
            if (gasMotor != null && gasMotor.HeatProduced == 0 && gasMotor.ProdCostPerMWh < elprice)
            {
                gasMotor.sellElectricity(elprice);
            }

            return new hourData("", "", demand, efficiencyBoilers);
        }
    }
}