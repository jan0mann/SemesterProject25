using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using HPO.Models;

namespace HPO.ViewModels
{
    public partial class BoilerInfoViewModel : ViewModelBase
    {
        private ObservableCollection<GasBoiler> _gasBoilers;
        private ObservableCollection<OilBoiler> _oilBoilers;

        public ObservableCollection<GasBoiler> GasBoilers
        {
            get { return _gasBoilers; }
            set { SetProperty(ref _gasBoilers, value); }
        }

        public ObservableCollection<OilBoiler> OilBoilers
        {
            get { return _oilBoilers; }
            set { SetProperty(ref _oilBoilers, value); }
        }

        public BoilerInfoViewModel()
        {
            GasBoilers = new ObservableCollection<GasBoiler>();
            OilBoilers = new ObservableCollection<OilBoiler>();
            InitializeBoilers();
        }

        private void InitializeBoilers()
        {
            var assetManager = new AssetManager();
            var (gasBoilers, oilBoilers) = assetManager.ShowInfo();
            GasBoilers = gasBoilers;
            OilBoilers = oilBoilers;
        }
    }
}