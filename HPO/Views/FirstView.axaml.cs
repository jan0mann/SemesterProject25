using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using HPO.ViewModels;

namespace HPO.Views;

public partial class FirstView : UserControl
{
    public FirstView()
    {
        InitializeComponent();
        DataContext = new HeatDemandViewModel();
    }

    // private void InitializeComponent()
    // {
    //     AvaloniaXamlLoader.Load(this);
    // }

    private async void SaveButton_Click(object? sender, RoutedEventArgs e)
    {
        // var heatDemandViewModel = new HeatDemandViewModel();

        // heatDemandViewModel.SaveScenario1BoilerDistributionCommand.Execute(null);
        if (sender is Button clickedButton)
        {
            var originalBrush = clickedButton.Background;
            var originalText = clickedButton.Content;
            //var originalTextColor = clickedButton.Foreground;
            clickedButton.Background = Brushes.LightGreen;
            clickedButton.Content = "        ✔ Saved        ";
            //clickedButton.Foreground = Brushes.Blue;
            await Task.Delay(1500); // 1 second delay

            clickedButton.Background = originalBrush;
            clickedButton.Content = originalText;
            //clickedButton.Foreground = originalTextColor;

        }
    }
}