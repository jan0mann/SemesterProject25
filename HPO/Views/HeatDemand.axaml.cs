using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace HPO.Views;

public partial class HeatDemand : UserControl
{
    public HeatDemand()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}