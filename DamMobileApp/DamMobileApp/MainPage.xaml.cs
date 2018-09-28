using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.Xamarin.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DamMobileApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            double MaxX = 200;
            double MinX = 0;
            double MaxY = 200;
            double MinY = 0;
            double alertLevel = 100;

            var model = new OxyPlot.PlotModel
            {
                Title = "Water level"
            };

            var alertLine = new LineSeries
            {
                Color = OxyColor.Parse("#ff0000"),
                Title = "Alert line"
            };
            alertLine.Points.Add(new DataPoint(MinX, alertLevel));
            alertLine.Points.Add(new DataPoint(MaxX, alertLevel));

            var series = new LineSeries
            {
                Color = OxyColor.Parse("#0000ff")
            };
            series.Points.Add(new DataPoint(0, 5));
            series.Points.Add(new DataPoint(10, 140));
            series.Points.Add(new DataPoint(20, 60));
            series.Points.Add(new DataPoint(30, 120));
            series.Points.Add(new DataPoint(40, 80));

            model.Axes.Add(new LinearAxis() { Position = AxisPosition.Bottom, Minimum = 0, Maximum = 40 });
            model.Axes.Add(new LinearAxis() { Position = AxisPosition.Left, Minimum = 0, Maximum = 150 });
            model.Series.Add(series);
            model.Series.Add(alertLine);

            Content = new PlotView
            {
                Model = model,
                VerticalOptions = LayoutOptions.Fill,
                HorizontalOptions = LayoutOptions.Fill
            };
        }
    }
}
