using Xunit;
using System.Collections.Generic;
using System.Linq;
using HPO.Models;
using HPO;

namespace Testing
{
    public class AssetManagerTests
    {
        [Fact]
        public void RequestProduction_ShouldCalculateProperly()
        {

            // Given
            var boilers = new List<Boiler>
            {
                new Boiler("Gas Boiler 1", BoilerType.Gas, 4.0f, 520f, 175f, 0.9f, 0),
                new Boiler("Gas Boiler 2", BoilerType.Gas, 3.0f, 560f, 130f, 0.7f, 0),
                new Boiler("Oil Boiler 1", BoilerType.Oil, 4.0f, 670f, 330f, 1.5f, 0)
            };


            foreach (var boiler in boilers)
            {
                // Testing when necessaryHeat is 3
                double result = boiler.requestProduction(3);

                // Then
                Assert.Equal(3, result);
                Assert.Equal(3, boiler.HeatProduced);
                Assert.Equal(boiler.CO2EmissionPerMWh * 3, boiler.CO2Produced);
                Assert.Equal(boiler.ConsumptionPerMWh * 3, boiler.Consumed, 0.000001);
                Assert.Equal(boiler.ProdCostPerMWh * 3, boiler.Cost);

                // Testing when necessaryHeat is 0
                result = boiler.requestProduction(0);

                // Then
                Assert.Equal(0, result);
                Assert.Equal(0, boiler.HeatProduced);
                Assert.Equal(0, boiler.CO2Produced);
                Assert.Equal(0, boiler.Consumed);
                Assert.Equal(0, boiler.Cost);

                // Testing when necessaryHeat is above max for all boilers
                try
                {
                    result = boiler.requestProduction(8);

                    // Then
                    Assert.NotEqual(8, result);
                    Assert.NotEqual(8, boiler.HeatProduced);
                    Assert.NotEqual(boiler.CO2EmissionPerMWh * 8, boiler.CO2Produced);
                    Assert.NotEqual(boiler.ConsumptionPerMWh * 8, boiler.Consumed, 0.000001);
                    Assert.NotEqual(boiler.ProdCostPerMWh * 8, boiler.Cost);
                }
                catch (Exception lol)
                {
                    Console.WriteLine($"Test failed because:");

                    throw new InvalidOperationException(" the MaxHeat given was bigger than that of the boiler.", lol);


                }

            }
        }
    }
}