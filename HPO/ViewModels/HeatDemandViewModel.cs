using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using HPO.Models;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Globalization;

namespace HPO.ViewModels;

public partial class HeatDemandViewModel : ObservableObject
{
    private DateTime _selectedDate = DateTime.Now;
    private TimeSpan _selectedTime = DateTime.Now.TimeOfDay;
    private DateTime _selectedDateTime;

    public HeatDemandViewModel()
    {
  
        LoadPredefinedDateTimes();

        SelectedDateTime = PredefinedDateTimes.Count > 0 ? PredefinedDateTimes[0] : DateTime.Now;

        Series = new ISeries[]
        {
            new LineSeries<double>
            {
                Values = new double[] { 3, 7, 9, 14, 2, 6, 8 },
                Stroke = new SolidColorPaint(SKColors.Red) { StrokeThickness = 3 }
            }
        };

        YAxes = new Axis[]
        {
            new Axis { Name = "Heat Demand", LabelsRotation = 15 }
        };
    }

    public ObservableCollection<DateTime> PredefinedDateTimes { get; } = new();

    public DateTime SelectedDateTime
    {
        get => _selectedDateTime;
        set => SetProperty(ref _selectedDateTime, value);
    }

    public DateTime SelectedDate
    {
        get => _selectedDate;
        set
        {
            SetProperty(ref _selectedDate, value);
            UpdateDateTime();
        }
    }

    public TimeSpan SelectedTime
    {
        get => _selectedTime;
        set
        {
            SetProperty(ref _selectedTime, value);
            UpdateDateTime();
        }
    }

    public ISeries[] Series { get; }
    public Axis[] YAxes { get; }

    private void UpdateDateTime() => SelectedDateTime = SelectedDate.Date + SelectedTime;
    private void LoadPredefinedDateTimes()
    {
        FileReader fileReader = new FileReader();
        fileReader.ReadInfo();
        var predefinedOffsets = fileReader.winterList;
        

        
        foreach (var item in predefinedOffsets)
            {
                
                if (DateTime.TryParseExact(item.WTimeTo, "MM-dd-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
                {
                    
                    PredefinedDateTimes.Add(parsedDate.AddDays(item.WHeatDemand));
                }
                else
                {
                   
                    Console.WriteLine($"Invalid date format: {item.WTimeFrom}");
                }
            }
    }
}
