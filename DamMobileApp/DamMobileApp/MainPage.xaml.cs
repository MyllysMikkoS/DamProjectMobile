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
            model.Axes.Add(new LinearAxis() { Position = AxisPosition.Left, Minimum = 0, Maximum = 100, IsPanEnabled = false, IsZoomEnabled = false });
            model.Axes.Add(new DateTimeAxis() { Position = AxisPosition.Bottom, Minimum = 0, Maximum = 100, IsPanEnabled = false, IsZoomEnabled = false });

            // Create alert line
            var alertLine = new LineSeries
            {
                Color = OxyColor.Parse("#ff0000"),
                Title = "Alert line"
            };
            alertLine.Points.Add(new DataPoint(0, 70));
            alertLine.Points.Add(new DataPoint(100, 70));

            // Create series
            var series = new LineSeries
            {
                Color = OxyColor.Parse("#0000ff")
            };
            series.Points.Add(new DataPoint(0, 5));
            series.Points.Add(new DataPoint(20, 90));
            series.Points.Add(new DataPoint(40, 80));
            series.Points.Add(new DataPoint(60, 20));
            series.Points.Add(new DataPoint(80, 80));

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
