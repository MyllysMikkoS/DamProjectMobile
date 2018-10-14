using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.Xamarin.Forms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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

            // Update date limits
            InitDatePickers();

            // Create plot
            CreatePlot();

            // Add plot to the page
            AddPlotToPage();

            // Create onclick events for buttons and datepickers
            WaterFlowButton.Clicked += WaterFlowButtonPressed;
            WaterLevelButton.Clicked += WaterLevelButtonPressed;
            StartDatePicker.DateSelected += StartDatePicker_DateSelected;
            EndDatePicker.DateSelected += EndDatePicker_DateSelected;
        }

        private void EndDatePicker_DateSelected(object sender, DateChangedEventArgs e)
        {
            GlobalDateLimits.Instance.EndDate = EndDatePicker.Date;
            Debug.WriteLine("END DATE SELECTED: " + e.NewDate.ToString());
            StartDatePicker.MaximumDate = GlobalDateLimits.Instance.EndDate;

            // Update plot
            if (SingletonPlot.Instance.Type == PlotType.WATERLEVEL)
                SingletonWaterLevelModel.Instance.SetSeries(SingletonWaterLevelDataList.Instance);
            else
                SingletonWaterFlowModel.Instance.SetSeries(SingletonWaterFlowDataList.Instance);
        }

        private void StartDatePicker_DateSelected(object sender, DateChangedEventArgs e)
        {
            GlobalDateLimits.Instance.StartDate = StartDatePicker.Date;
            Debug.WriteLine("START DATE SELECTED: " + e.NewDate.ToString());
            EndDatePicker.MinimumDate = GlobalDateLimits.Instance.StartDate;

            // Update plot
            if (SingletonPlot.Instance.Type == PlotType.WATERLEVEL)
                SingletonWaterLevelModel.Instance.SetSeries(SingletonWaterLevelDataList.Instance);
            else
                SingletonWaterFlowModel.Instance.SetSeries(SingletonWaterFlowDataList.Instance);
        }

        public void CreatePlot()
        {
            // Get PlotModel according to plotviewtype
            if (SingletonPlot.Instance.Type == PlotType.WATERLEVEL)
                SingletonPlot.Instance.Model = SingletonWaterLevelModel.Instance;
            else
                SingletonPlot.Instance.Model = SingletonWaterFlowModel.Instance;
        }

        public void AddPlotToPage()
        {
            SingletonPlot.Instance.VerticalOptions = LayoutOptions.Fill;
            SingletonPlot.Instance.HorizontalOptions = LayoutOptions.Fill;
            PlotLayout.Children.Add(SingletonPlot.Instance, Constraint.Constant(0));
        }

        public void InitDatePickers()
        {
            StartDatePicker.Date = GlobalDateLimits.Instance.StartDate;
            EndDatePicker.Date = GlobalDateLimits.Instance.EndDate;

            StartDatePicker.MaximumDate = GlobalDateLimits.Instance.EndDate;
            EndDatePicker.MinimumDate = GlobalDateLimits.Instance.StartDate;
        }

        public async void WaterFlowButtonPressed(object sender, EventArgs e)
        {
            await GetWaterFlowData();
        }

        public async void WaterLevelButtonPressed(object sender, EventArgs e)
        {
            await GetWaterLevelData();
        }

        public async Task GetWaterFlowData()
        {
            // Get Data
            await Task.Run(() =>
            {
                SingletonWaterFlowDataList.Instance.Clear();
                SingletonWaterFlowDataList.Instance.Add(new WaterFlowData(new DateTime(2000, 2, 2), 10));
                SingletonWaterFlowDataList.Instance.Add(new WaterFlowData(new DateTime(2007, 3, 3), 20));
                SingletonWaterFlowDataList.Instance.Add(new WaterFlowData(new DateTime(2013, 4, 4), 90));
                SingletonWaterFlowDataList.Instance.Add(new WaterFlowData(new DateTime(2018, 5, 5), 50));
                SingletonWaterFlowDataList.Instance.Add(new WaterFlowData(new DateTime(2004, 5, 5), 40));
                SingletonWaterFlowDataList.Instance.Add(new WaterFlowData(new DateTime(2015, 7, 7), 60));
            });

            // Update UI
            Device.BeginInvokeOnMainThread(() =>
            {
                SingletonWaterFlowModel.Instance.SetSeries(SingletonWaterFlowDataList.Instance);

                SingletonPlot.Instance.Type = PlotType.WATERFLOW;
                CreatePlot();
            });
        }

        public async Task GetWaterLevelData()
        {
            // Get data
            await Task.Run(() =>
            {
                SingletonWaterLevelDataList.Instance.Clear();
                SingletonWaterLevelDataList.Instance.Add(new WaterLevelData(new DateTime(2000, 2, 2), 40));
                SingletonWaterLevelDataList.Instance.Add(new WaterLevelData(new DateTime(2003, 3, 3), 10));
                SingletonWaterLevelDataList.Instance.Add(new WaterLevelData(new DateTime(2011, 4, 4), 80));
                SingletonWaterLevelDataList.Instance.Add(new WaterLevelData(new DateTime(2018, 5, 5), 10));
            });

            // Update UI
            Device.BeginInvokeOnMainThread(() =>
            {
                SingletonWaterLevelModel.Instance.SetSeries(SingletonWaterLevelDataList.Instance);

                SingletonPlot.Instance.Type = PlotType.WATERLEVEL;
                CreatePlot();
            });
        }
    }
}
