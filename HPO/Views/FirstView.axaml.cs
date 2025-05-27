using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using HPO.ViewModels;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using LiveChartsCore.SkiaSharpView.Avalonia;
using LiveChartsCore.SkiaSharpView.SKCharts;
using System;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;


namespace HPO.Views;

public partial class FirstView : UserControl
{
    public FirstView()
    {
        InitializeComponent();
        DataContext = new HeatDemandViewModel();
        //CreateImageFromCartesianControl();
    }


    public void CreateImageFromCartesianControl(int scenario)
    {
        var HeatDemandViewModel = new HeatDemandViewModel();//as for both winter and summer the number of current scenatio is set at the same time, thus it doesnt matter which value we are using
        string name="";
        // if (scenario == 'w')
        //     name = "winterChartScenario";
        // else
        //     name = "summerChartScenario";
        //string fullName = name + HeatDemandViewModel.CurrentSummerScenario.ToString() + ".png";
        string fullName = name + scenario + ".png";

        string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", fullName);
        //var chartControl = this.FindControl<CartesianChart>(scenario=='w'?"winterMainChart":"summerMainChart");
        var chartControl = this.FindControl<CartesianChart>("winterMainChart");
        var skChart = new SKCartesianChart(chartControl) { Width = 1920, Height = 1080, };
        skChart.SaveImage(path);
        Console.WriteLine($"Image saved succesfully @ {path}");
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}