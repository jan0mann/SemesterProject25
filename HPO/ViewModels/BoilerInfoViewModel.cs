using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using HPO.Models;

namespace HPO.ViewModels
{
    public partial class BoilerInfoViewModel : ViewModelBase
    {
        private ObservableCollection<Boiler> _Boilers;

        public ObservableCollection<Boiler> Boilers
        {
            get { return _Boilers; }
            set { SetProperty(ref _Boilers, value); }
        }

        public BoilerInfoViewModel()
        {
            Boilers = new ObservableCollection<Boiler>();
            InitializeBoilers();
        }

        private void InitializeBoilers()
        {
            var assetManager = new AssetManager();
            Boilers = assetManager.ShowInfo();
        }
    }
}