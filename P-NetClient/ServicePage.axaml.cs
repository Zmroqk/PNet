using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;
using System.Collections.Generic;
using OxyPlot;
using OxyPlot.Avalonia;
using Avalonia.Media;
using Avalonia.Threading;
using System.Linq;
using OxAxis = OxyPlot.Axes;
using Avalonia.Input;

namespace PNetClient
{
    public partial class ServicePage : UserControl
    {
        ServicePageViewModel ViewModel;

        public ServicePage()
        {
            InitializeComponent();
            ViewModel = new ServicePageViewModel(this);
            DataContext = this;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            ViewModel.ReadDataAsync().ContinueWith((data) =>
            {
                foreach (KeyValuePair<string, List<(DateTime Time, int Ping)>> record in data.Result)
                {
                    List<DataPoint> Pings = record.Value.Select((elem) => new DataPoint(OxAxis.DateTimeAxis.ToDouble(elem.Time), elem.Ping)).ToList();
                    Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        Plot plot = new Plot();
                        plot.Height = 500;
                        plot.Width = 1300;
                        plot.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left;
                        plot.Margin = new Thickness(10);
                        plot.Background = new SolidColorBrush(Color.Parse("DarkGray"));
                        PlotView plotView = (PlotView)this.Resources["PlotView"];
                        plotView.Model = plot.ActualModel;
                        plot.DefaultTrackerTemplate = plotView.DefaultTrackerTemplate;
                        Axis xAxis = new DateTimeAxis();
                        xAxis.Key = "DateTimeAxis";
                        xAxis.StringFormat = "|MM.dd HH.mm|";
                        xAxis.FontSize = 12;
                        xAxis.Minimum = OxAxis.DateTimeAxis.ToDouble(DateTime.Now.AddHours(-1));
                        xAxis.MaximumRange = 0.2d;
                        xAxis.AbsoluteMaximum = OxAxis.DateTimeAxis.ToDouble(DateTime.Now.AddHours(1));
                        Axis yAxis = new LinearAxis();
                        yAxis.Key = "PingAxis";
                        yAxis.AbsoluteMinimum = 0;
                        yAxis.AbsoluteMaximum = 5000;
                        yAxis.Maximum = 100;
                        yAxis.FontSize = 12;
                        yAxis.MaximumRange = 250;
                        plot.Axes.Add(xAxis);
                        plot.Axes.Add(yAxis);
                        LineSeries ls = new LineSeries();                      
                        ls.DataFieldX = "Time";
                        ls.DataFieldY = "Ping";
                        ls.XAxisKey = "DateTimeAxis";
                        ls.YAxisKey = "PingAxis";
                        ls.Color = Color.Parse("DarkRed");
                        ls.StrokeThickness = 0.2d;
                        ls.MarkerSize = 3;
                        ls.MarkerType = MarkerType.Circle;
                        ls.Items = Pings;
                        plot.Series.Add(ls);
                        plot.InvalidatePlot();
                        StackPanel sp = this.Find<StackPanel>("PlotStack");
                        sp.Children.Add(new Label() { 
                            Content=$"Data for: {record.Key}", 
                            HorizontalContentAlignment=Avalonia.Layout.HorizontalAlignment.Center,
                            FontSize=20
                        });
                        sp.Children.Add(plot);
                    });
                }
            });
        }
    }
}
