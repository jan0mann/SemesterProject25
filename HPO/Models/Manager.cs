using System.Collections.ObjectModel;

namespace HPO.Models
{
    public class AssetManager
    {

        public ObservableCollection<Boiler> ShowInfo2()
        {
            var Boilers = new ObservableCollection<Boiler>
            {
                new Boiler("Gas Boiler 1", BoilerType.Gas, 4.0f, 520f, 175f, 0.9f, 0),
                new Boiler("Oil Boiler 1", BoilerType.Oil, 4.0f, 670f, 330f, 1.5f, 0),
                new Boiler("Gas Motor 1", BoilerType.GasMotor, 3.5f, 990f, 650f, 1.8f, 2.6),
                new Boiler("Heat Pump 1", BoilerType.HeatPump, 6.0f, 60f, 0f, 0f, -6.0)
            };

            return Boilers;
        }
        public ObservableCollection<Boiler> ShowInfo1()
        {
            var Boilers = new ObservableCollection<Boiler>
            {
                new Boiler("Gas Boiler 1", BoilerType.Gas, 4.0f, 520f, 175f, 0.9f, 0),
                new Boiler("Gas Boiler 2", BoilerType.Gas, 3.0f, 560f, 130f, 0.7f, 0),
                new Boiler("Oil Boiler 1", BoilerType.Oil, 4.0f, 670f, 330f, 1.5f, 0)
            };

            return Boilers;
        }

    }
}