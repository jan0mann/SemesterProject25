using System.Collections.ObjectModel;

namespace HPO.Models
{
    public class AssetManager
    {
        public (ObservableCollection<GasBoiler> GasBoilers, ObservableCollection<OilBoiler> OilBoilers) ShowInfo()
        {
            var gasBoilers = new ObservableCollection<GasBoiler>
            {
                new GasBoiler("Gas Boiler 1", 4.0f, 520f, 175f, 0.9f),
                new GasBoiler("Gas Boiler 2", 3.0f, 560f, 130f, 0.7f),

            };

            var oilBoilers = new ObservableCollection<OilBoiler>
            {
                new OilBoiler("Oil Boiler 1", 4.0f, 670f, 330f, 1.5f)

            };

            return (gasBoilers, oilBoilers);
        }


    }
}