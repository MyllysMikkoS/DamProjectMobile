using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.Xamarin.Forms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

            // TEST PLOT DATA
            SingletonWaterFlowDataList.Instance.Add(new WaterFlowData(new DateTime(1111, 2, 2), 10));
            SingletonWaterFlowDataList.Instance.Add(new WaterFlowData(new DateTime(3333, 3, 3), 20));
            SingletonWaterFlowDataList.Instance.Add(new WaterFlowData(new DateTime(4444, 4, 4), 90));
            SingletonWaterFlowDataList.Instance.Add(new WaterFlowData(new DateTime(9999, 5, 5), 50));

            // Create plot
            CreatePlot();

            // Add plot to the page
            PlotLayout.Children.Add(SingletonPlot.Instance, Constraint.Constant(0));

            // -- TESTING
            WaterFlowButton.Clicked += (object sender, EventArgs e) =>
            {
                SingletonWaterFlowDataList.Instance.Add(new WaterFlowData(new DateTime(5555, 5, 5), 40));
                SingletonWaterFlowDataList.Instance.Add(new WaterFlowData(new DateTime(7777, 7, 7), 60));
                SingletonWaterFlowModel.Instance.SetSeries(SingletonWaterFlowDataList.Instance);
            };
            WaterLevelButton.Clicked += (object sender, EventArgs e) =>
            {
                SingletonWaterFlowDataList.Instance.Clear();
                SingletonWaterFlowDataList.Instance.Add(new WaterFlowData(new DateTime(1111, 5, 5), 10));
                SingletonWaterFlowDataList.Instance.Add(new WaterFlowData(new DateTime(9999, 7, 7), 90));
                SingletonWaterFlowModel.Instance.SetSeries(SingletonWaterFlowDataList.Instance);
            };
        }

        public void CreatePlot()
        {
            // Get PlotModel according to plotviewtype
            if (SingletonPlot.Instance.Type == PlotType.WATERLEVEL)
                SingletonPlot.Instance.Model = SingletonWaterLevelModel.Instance;
            else
                SingletonPlot.Instance.Model = SingletonWaterFlowModel.Instance;

            SingletonPlot.Instance.VerticalOptions = LayoutOptions.Fill;
            SingletonPlot.Instance.HorizontalOptions = LayoutOptions.Fill;
        }
    }
}
