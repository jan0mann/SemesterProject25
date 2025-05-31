using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HPO.Models;
using LiveChartsCore.VisualElements;

namespace HPO.ViewModels
{
    public partial class BoilerInfoViewModel : ViewModelBase
    {
        private ObservableCollection<Boiler> _Boilers;
        private readonly AssetManager assetManager = new AssetManager();

        public ObservableCollection<Boiler> Boilers
        {
            get { return _Boilers; }
            set { SetProperty(ref _Boilers, value); }
        }

        public IRelayCommand ShowScenario1Command { get; }
        public IRelayCommand ShowScenario2Command { get; }

        public BoilerInfoViewModel()
        {
            Boilers = new ObservableCollection<Boiler>();
            ShowScenario1Command = new RelayCommand(ShowScenario1);
            ShowScenario2Command = new RelayCommand(ShowScenario2);
            ShowScenario1();
        }

        private void ShowScenario1()
        {
            Boilers = assetManager.ShowBoilerInfo1();
        }

        private void ShowScenario2()
        {
            Boilers = assetManager.ShowBoilerInfo2();
        }
    }
}