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

            // Create plot
            CreatePlot();

            // Add plot to the page
            PlotLayout.Children.Add(SingletonPlot.Instance, Constraint.Constant(0));
        }

        public void CreateUI()
        {
            // Create containers
            /*MainLayout = new StackLayout();
            PlotContainer = new StackLayout();
            DatePickerContainer = new StackLayout();
            ButtonContainer = new StackLayout();

            */ 
        }

        public void CreatePlot()
        {
            string PlotTitle;

            // Set title
            if (SingletonPlot.Instance.Type == PlotType.WATERLEVEL)
                PlotTitle = "Water level";
            else
                PlotTitle = "Water flow";

            // Create model and create the axes
            var model = new OxyPlot.PlotModel
            {
                Title = PlotTitle
            };
            var startDate = DateTime.Now.AddDays(-10);
            var endDate = DateTime.Now;
            var minValue = DateTimeAxis.ToDouble(startDate);
            var maxValue = DateTimeAxis.ToDouble(endDate);
            model.Axes.Add(new LinearAxis() { Position = AxisPosition.Left, Minimum = 0, Maximum = 100, IsPanEnabled = false, IsZoomEnabled = false });
            model.Axes.Add(new DateTimeAxis() { Position = AxisPosition.Bottom, Minimum = minValue, Maximum = maxValue, IsPanEnabled = false, IsZoomEnabled = false });

            // Create alert line
            var alertLine = new LineSeries
            {
                Color = OxyColor.Parse("#ff0000"),
                Title = "Alert line"
            };
            alertLine.Points.Add(new DataPoint(minValue, 70));
            alertLine.Points.Add(new DataPoint(maxValue, 70));

            // Create series
            var series = new LineSeries
            {
                Color = OxyColor.Parse("#0000ff")
            };
            var data1 = DateTime.Now.AddDays(-10);
            var data2 = DateTime.Now.AddDays(-8);
            var data3 = DateTime.Now.AddDays(-6);
            var data4 = DateTime.Now.AddDays(-4);
            var data5 = DateTime.Now.AddDays(-2);
            var d1 = DateTimeAxis.ToDouble(data1);
            var d2 = DateTimeAxis.ToDouble(data2);
            var d3 = DateTimeAxis.ToDouble(data3);
            var d4 = DateTimeAxis.ToDouble(data4);
            var d5 = DateTimeAxis.ToDouble(data5);
            series.Points.Add(new DataPoint(d1, 5));
            series.Points.Add(new DataPoint(d2, 90));
            series.Points.Add(new DataPoint(d3, 80));
            series.Points.Add(new DataPoint(d4, 20));
            series.Points.Add(new DataPoint(d5, 80));

            // Add series and alert line
            model.Series.Add(series);
            model.Series.Add(alertLine);

            // Add model to PlotView
            SingletonPlot.Instance.Model = model;
            SingletonPlot.Instance.VerticalOptions = LayoutOptions.Fill;
            SingletonPlot.Instance.HorizontalOptions = LayoutOptions.Fill;
        }
    }
}
