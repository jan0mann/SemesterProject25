using System.Collections.ObjectModel;

namespace HPO.Models
{
    public class AssetManager
    {
        public (ObservableCollection<GasBoiler> GasBoilers, ObservableCollection<OilBoiler> OilBoilers) ShowInfo()
        {
            var gasBoilers = new ObservableCollection<GasBoiler>
            {
                new GasBoiler("Gas Boiler 1", 100, 200, 300),
                new GasBoiler("Gas Boiler 2", 200, 300, 400),
                new GasBoiler("Gas Boiler 3", 300, 400, 500)
            };

            var oilBoilers = new ObservableCollection<OilBoiler>
            {
                new OilBoiler("Oil Boiler 1", 400, 500, 600),
                new OilBoiler("Oil Boiler 2", 500, 600, 700)
            };

            return (gasBoilers, oilBoilers);
        }
    }
}