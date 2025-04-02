using System.Collections.ObjectModel;

namespace HPO.Models
{
    public class AssetManager
    {

        public ObservableCollection<Boiler> ShowInfoCombined()//change the name to just ShowInfo after removing the old method
        {
            var Boilers = new ObservableCollection<Boiler>
            {
                new GasBoiler("Gas Boiler 1", 4.0f, 520f, 175f, 0.9f),
                new GasBoiler("Gas Boiler 2", 3.0f, 560f, 130f, 0.7f),
                new OilBoiler("Oil Boiler 1", 4.0f, 670f, 330f, 1.5f)
            };

            return Boilers;
        }

    }
}