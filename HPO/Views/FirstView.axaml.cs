using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using HPO.ViewModels;

namespace HPO.Views;

public partial class FirstView : UserControl
{
    public FirstView()
    {
        InitializeComponent();
        DataContext = new HeatDemandViewModel();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}