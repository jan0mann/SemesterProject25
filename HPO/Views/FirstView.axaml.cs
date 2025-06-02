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

 
    private async void SaveButton_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is Button clickedButton)
        {
            var originalBrush = clickedButton.Background;
            var originalText = clickedButton.Content;
            clickedButton.Background = Brushes.LightGreen;
            clickedButton.Content = "        ✔ Saved        ";
            await Task.Delay(1500); 

            clickedButton.Background = originalBrush;
            clickedButton.Content = originalText;


        }
    }
}