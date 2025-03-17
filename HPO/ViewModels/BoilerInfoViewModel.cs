using System.Collections.ObjectModel;
using System;
using CommunityToolkit.Mvvm.ComponentModel;


namespace HPO.ViewModels;

public partial class BoilerInfoViewModel : ViewModelBase
{
    [ObservableProperty]
    private string secondViewText = "On this page will be boiler information.";
}