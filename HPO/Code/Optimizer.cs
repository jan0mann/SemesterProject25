using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using HPO.Models;
using HPO.Views;

namespace HPO.Optimizer
{
    public class hourData{
        public string Date { get; set; }
        public string Time { get; set; }
        public double Demand { get; set; }
        //public double ElectricityPrice {get; set;}
        public List<Boiler> Boilers { get; set; }
        public hourData(string date, string time, double demand,/* double electricityPrice,*/ List<Boiler> boilers)
        {
            Date = date;
            Time = time;
            Demand = demand;
            //ElectricityPrice =electricityPrice;
            Boilers = boilers;
        }

        public hourData()
        {
        }

        public hourData deepcopy()
        {
            var boilers = Boilers.Select(x => x.Deepcopy()).ToList();
            return new hourData(Date, Time, Demand,/* ElectricityPrice,*/ boilers);
        }
    }

    public class Optimizer
    {
        public List<Boiler> calculateEfficiency(List<Boiler> boilers, double elprice)
        {
            boilers.Find(x => x.BoilerType == BoilerType.HeatPump).ProdCostPerMWh = (float)elprice+60f;
            var boi = boilers.OrderBy(x => x.ProdCostPerMWh).ToList();
            return boi;
        }

        public hourData OptimizeHour(List<Boiler> boilers, double demand, double elprice){

            //double elprice = 96.9;
            var efficiencyBoilers = calculateEfficiency(boilers, elprice);
            double demandForCalc = demand;
            int i = 0;

            while(demandForCalc > 0 && i < efficiencyBoilers.Count){
                demandForCalc -= efficiencyBoilers[i].requestProduction(demandForCalc);
                i++;
            }
            Boiler GasMotor = efficiencyBoilers.Find(x => x.BoilerType == BoilerType.GasMotor);
            if((GasMotor.HeatProduced == 0)&&(GasMotor.ProdCostPerMWh < elprice)){
                GasMotor.sellElectricity(elprice);
            }

            return new hourData("", "", demand, efficiencyBoilers);
        }
        
    }
}