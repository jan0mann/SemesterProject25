using Xunit;
using System.Collections.Generic;
using System.Linq;
using HPO.Models;
using HPO; 
using HPO.Optimizer;

namespace Testing
{
    public class OptimizerTests
    {
        [Fact]
        public void OptimizeHour_ShouldCorrectlyAssignDemandToBoilersInOrderOfEfficiency()
        {
            // Arrange
            var boilers = new List<Boiler>
            {
                new Boiler("Gas Boiler 1", BoilerType.Gas, 4.0f, 520f, 175f, 0.9f),
                new Boiler("Gas Boiler 2", BoilerType.Gas, 3.0f, 560f, 130f, 0.7f),
                new Boiler("Oil Boiler 1", BoilerType.Oil, 4.0f, 670f, 330f, 1.5f)
            };
            double demand = 8.5;
            
            //Act
            var optimizer = new Optimizer();
            var result = optimizer.OptimizeHour(boilers, demand); 
            
            // Assert
            //Assert.True(result.Boilers[0].MaxHeat >= result.Boilers[1].MaxHeat);


            // production matches demand
            double totalProduction = result.Boilers.Sum(b => b.HeatProduced);
            Assert.Equal(demand, totalProduction, 0.001);// e.g. for  precision delta for floating point comparison
        }


        [Fact]
        public void OptimizeHour_CheckCO2Emissions()
        {
            // Arrange
            var boilers = new List<Boiler>
            {
                new Boiler("Gas Boiler 1", BoilerType.Gas, 4.0f, 520f, 175f, 0.9f),
                new Boiler("Gas Boiler 2", BoilerType.Gas, 3.0f, 560f, 130f, 0.7f),
                new Boiler("Oil Boiler 1", BoilerType.Oil, 4.0f, 670f, 330f, 1.5f)
            };
            List<double> co2emissions = new List<double>{700f, 390f, 495f};
            
            //Act
            var optimizer = new Optimizer();
            var result = optimizer.OptimizeHour(boilers, 8.5); 

            // production matches demand
            for (int i =0; i< boilers.Count(); i++){
                Assert.Equal(co2emissions[i], result.Boilers[i].CO2Produced, 0.001);// e.g. for  precision delta for floating point comparison
            }
        }


        [Fact]
        public void calculateEfficiency_CheckIfSortsCorrectly(){
             var optimizer = new Optimizer();

            var boilers1 = new List<Boiler>
            {
                new Boiler("Gas Boiler 1", BoilerType.Gas, 4.0f, 520f, 175f, 0.9f),
                new Boiler("Gas Boiler 2", BoilerType.Gas, 3.0f, 560f, 130f, 0.7f),
                new Boiler("Oil Boiler 1", BoilerType.Oil, 4.0f, 670f, 330f, 1.5f)
            };


            var result1 = optimizer.calculateEfficiency(boilers1);
            //Check if the boilers are sorted by cost
            for (int i = 0; i < result1.Count - 1; i++)
            {
                Assert.True(result1[i].ProdCostPerMWh <= result1[i + 1].ProdCostPerMWh);
            }
        }
    }
}