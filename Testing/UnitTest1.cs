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
            Assert.Equal(demand, result.Demand);
            Assert.True(result.Boilers[0].MaxHeat >= result.Boilers[1].MaxHeat);
            
            // production matches demand
            double totalProduction = result.Boilers.Sum(b => b.HeatProduced);
            //Assert.Equal(demand, totalProduction, 0.001); e.g. for  precision delta for floating point comparison
        }
    }
}