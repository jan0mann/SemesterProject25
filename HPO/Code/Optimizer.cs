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
            return new hourData(Date, Time, Demand, boilers);
        }
    }

    public class Optimizer
    {
        List<Boiler> calculateEfficiency(List<Boiler> boilers)
        {
            // foreach(var boiler in boilers){
            //     //part where we calculate the efficiency of the boiler
            //     // when we will have electricity
            // }
            var boi = boilers.OrderBy(x => x.ProdCost).ToList();
            return boi;
        }


        // public Dictionary<int, List<hourData>> Optimize(List<Boiler> boilers, List<HPO.Models.Winter> data)
        // {
        //     Dictionary<int, List<hourData>> optimizedData = new();

        //     int dayI = 1;
        //     int hourI = 0;
        //     foreach(var hour in data){
        //         var efficiencyBoilers = calculateEfficiency(boilers);

        //         double demand = (double)hour.WHeatDemand;
        //         int i = 0;

        //         while(demand > 0){
        //             // efficiencyBoilers[i].HeatProduced = efficiencyBoilers[i].MaxHeat;
        //             // demand -= efficiencyBoilers[i].MaxHeat;
        //             demand -= efficiencyBoilers[i].requestProduction(demand);
        //             i++;
        //         }
        //         // while(demand > 0){
        //         //     if(demand > efficiencyBoilers[i].MaxHeat){
        //         //         efficiencyBoilers[i].HeatProduced = efficiencyBoilers[i].MaxHeat;
        //         //         demand -= efficiencyBoilers[i].MaxHeat;
        //         //         i++;
        //         //     }else{
        //         //         efficiencyBoilers[i].HeatProduced = demand;
        //         //         demand = 0;
        //         //     }
        //         // }

        //         hourI++;

        //         // if(hour.WDateFrom == "2024-03-01"){
        //         //     Console.WriteLine();
        //         //     Console.WriteLine(hour.WDateFrom+"+ "+hour.WHourFrom+". "+(double)hour.WHeatDemand);
                    
        //         //     foreach (var boiler in efficiencyBoilers){
        //         //         Console.WriteLine(boiler.Name);
        //         //         Console.WriteLine(boiler.HeatProduced);
        //         //     }
        //         // }
                
        //         if(hourI > 23){
        //             hourI = 0;
        //             dayI++;
        //         }
        //         if(!optimizedData.ContainsKey(dayI)){
        //             optimizedData[dayI] = new List<hourData>();
        //         }
        //         optimizedData[dayI].Add(new hourData(hour.WDateFrom, hour.WHourFrom, hour.WHeatDemand ?? 0, efficiencyBoilers));
        //     }
        //     return optimizedData;
        // }

        public hourData OptimizeHour(List<Boiler> boilers, double demand){

            //reset the boiler data

            var efficiencyBoilers = calculateEfficiency(boilers);
            double demandForCalc = demand;
            int i = 0;

            while(demandForCalc > 0){
                demandForCalc -= efficiencyBoilers[i].requestProduction(demandForCalc);
                i++;
            }
            return new hourData("", "", demand, efficiencyBoilers);
        }
        
    }
}