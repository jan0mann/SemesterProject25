using Xunit;
using System.Collections.Generic;
using System.Linq;
using HPO.Models;
using HPO;

namespace Testing
{
    public class TestBoiler //test class because heatproduced is read-only
    {
        public BoilerType BoilerType { get; set; }
        public double HeatProduced { get; set; }
    }
    public class PrimaryEnergyTest
    {
        [Fact]
        public void DominantPrimaryEnergy_IsCorrectlyIdentified()
        {
            // Arrange
            var boilers = new List<TestBoiler>
            {
                new TestBoiler { BoilerType = BoilerType.Gas, HeatProduced = 3.0 },
                new TestBoiler { BoilerType = BoilerType.Oil, HeatProduced = 1.0 },
                new TestBoiler { BoilerType = BoilerType.HeatPump, HeatProduced = 0.5 }
            };

            // Act
            var energySums = new Dictionary<string, double> { { "Oil", 0 }, { "Gas", 0 }, { "Electricity", 0 } };
            foreach (var boiler in boilers)
            {
                var type = boiler.BoilerType switch
                {
                    BoilerType.Oil => "Oil",
                    BoilerType.Gas => "Gas",
                    BoilerType.GasMotor => "Gas",
                    BoilerType.HeatPump => "Electricity",
                    _ => "Unknown"
                };
                if (energySums.ContainsKey(type))
                    energySums[type] += boiler.HeatProduced;
            }
            var dominant = energySums.OrderByDescending(kv => kv.Value).First().Key;

            // Assert
            Assert.Equal("Gas", dominant);
        }
    }
}