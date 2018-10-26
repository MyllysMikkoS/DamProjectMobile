using GraphQL.Client;
using GraphQL.Common.Request;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;


namespace DamMobileApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            // Get water data on app start
            Task.Run(async () => await GetAllWaterDataOnAppStart());
        }

        private void EndDatePicker_DateSelected(object sender, DateChangedEventArgs e)
        {
            Task.Run(async () => { await GetWaterData(StartDatePicker.Date, e.NewDate); });
        }

        private void StartDatePicker_DateSelected(object sender, DateChangedEventArgs e)
        {
            Task.Run(async () => { await GetWaterData(e.NewDate, EndDatePicker.Date); });
        }

        public void CheckIfAlertIsOn()
        {
            // Do the check
            if (SingletonWaterLevelDataList.Instance.OrderByDescending(x => x.Timestamp).First().WaterLevel 
                >= SingletonWaterLevelModel.Instance.AlertLineValue)
            {
                // Display alert
                Debug.WriteLine("DISPLAYING ALERT");
                DisplayAlert("Alert!", "The water level rised over the alert level.", "OK");
            }
        }

        public void CreatePlot()
        {
            // Get PlotModel according to plotviewtype
            if (SingletonPlot.Instance.Type == PlotType.WATERLEVEL)
                SingletonPlot.Instance.Model = SingletonWaterLevelModel.Instance;
            else
                SingletonPlot.Instance.Model = SingletonWaterFlowModel.Instance;
        }

        public void UpdatePlot()
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

        public void UpdateDatePickers()
        {
            DateTime newestFlowDate = SingletonWaterFlowDataList.Instance.OrderByDescending(x => x.Timestamp).First().Timestamp;
            DateTime oldestFlowDate = SingletonWaterFlowDataList.Instance.OrderByDescending(x => x.Timestamp).Last().Timestamp;
            DateTime newestLevelDate = SingletonWaterLevelDataList.Instance.OrderByDescending(x => x.Timestamp).First().Timestamp;
            DateTime oldestLevelDate = SingletonWaterLevelDataList.Instance.OrderByDescending(x => x.Timestamp).Last().Timestamp;
            DateTime start;
            DateTime end;

            // Check newest date of flows and levels
            if (newestFlowDate > newestLevelDate) end = newestFlowDate;
            else end = newestLevelDate;

            // Check oldest date of flows and levels
            if (oldestFlowDate < oldestLevelDate) start = oldestFlowDate;
            else start = oldestLevelDate;

            GlobalDateLimits.Instance.StartDate = start;
            GlobalDateLimits.Instance.EndDate = end;

            // Set maximum date according to current time
            EndDatePicker.MaximumDate = DateTime.UtcNow;

            // Set current values
            StartDatePicker.Date = GlobalDateLimits.Instance.StartDate;
            EndDatePicker.Date = GlobalDateLimits.Instance.EndDate;

            DateTime StartDateMax = new DateTime(end.Year, end.Month, end.Day);
            DateTime EndDateMin = new DateTime(start.Year, start.Month, start.Day);
            StartDatePicker.MaximumDate = StartDateMax;
            EndDatePicker.MinimumDate = EndDateMin;
        }

        public void UpdateDatePickersOnAppStart()
        {
            DateTime newestFlowDate = SingletonWaterFlowDataList.Instance.OrderByDescending(x => x.Timestamp).First().Timestamp;
            DateTime oldestFlowDate = SingletonWaterFlowDataList.Instance.OrderByDescending(x => x.Timestamp).Last().Timestamp;
            DateTime newestLevelDate = SingletonWaterLevelDataList.Instance.OrderByDescending(x => x.Timestamp).First().Timestamp;
            DateTime oldestLevelDate = SingletonWaterLevelDataList.Instance.OrderByDescending(x => x.Timestamp).Last().Timestamp;
            DateTime start;
            DateTime end;

            // Check newest date of flows and levels
            if (newestFlowDate > newestLevelDate) end = newestFlowDate;
            else end = newestLevelDate;

            // Check oldest date of flows and levels
            if (oldestFlowDate < oldestLevelDate) start = oldestFlowDate;
            else start = oldestLevelDate;

            GlobalDateLimits.Instance.FirstDateInDataBase = start;
            GlobalDateLimits.Instance.StartDate = start;
            GlobalDateLimits.Instance.EndDate = end;

            // Set minimum date according to all database data
            StartDatePicker.MinimumDate = GlobalDateLimits.Instance.FirstDateInDataBase;

            // Set maximum date according to current time
            EndDatePicker.MaximumDate = DateTime.UtcNow;

            // Set current values
            StartDatePicker.Date = GlobalDateLimits.Instance.StartDate;
            EndDatePicker.Date = GlobalDateLimits.Instance.EndDate;

            DateTime StartDateMax = new DateTime(end.Year, end.Month, end.Day);
            DateTime EndDateMin = new DateTime(start.Year, start.Month, start.Day);
            StartDatePicker.MaximumDate = StartDateMax;
            EndDatePicker.MinimumDate = EndDateMin;
        }

        public void WaterFlowButtonPressed(object sender, EventArgs e)
        {
            ShowWaterFlowData();
        }

        public void WaterLevelButtonPressed(object sender, EventArgs e)
        {
            ShowWaterLevelData();
        }

        public async Task GetAllWaterDataOnAppStart()
        {
            Debug.WriteLine("GET DATA ON APP START");

            // Clear old datalists
            SingletonWaterFlowDataList dataListFlow = SingletonWaterFlowDataList.Instance;
            SingletonWaterLevelDataList dataListLevel = SingletonWaterLevelDataList.Instance;
            dataListFlow.Clear();
            dataListLevel.Clear();

            // Get Data and parse it
            await Task.Run(async () =>
            {
                var WaterFlowDataRequest = new GraphQLRequest
                {
                    Query = "{ water_data_query( start_time: \"\" end_time: \"\" ) { flow level timestamp } }"
                };
                var graphQLClient = new GraphQLClient("http://api.otpt18-c.course.tamk.cloud/");
                var graphQLResponse = await graphQLClient.PostAsync(WaterFlowDataRequest);
                var dynamicData = graphQLResponse.Data["water_data_query"];

                // Add fetched data into datalists
                foreach (JObject data in dynamicData)
                {
                    DateTime time = (new DateTime(1970, 1, 1)).AddMilliseconds(double.Parse(data["timestamp"].ToString()));
                    dataListFlow.Add(new WaterFlowData(
                        // Date
                        time,
                        // Flow in cubic meter per second
                        float.Parse(data["flow"].ToString())
                        ));
                    dataListLevel.Add(new WaterLevelData(
                        // Date
                        time,
                        // Level in meters
                        float.Parse(data["level"].ToString()) / 100 // value is in centimeters so dividing by 100
                        ));
                }
            }).ContinueWith((task) => {
                // Update plots
                Device.BeginInvokeOnMainThread(() =>
                {
                    // Update date limits
                    UpdateDatePickersOnAppStart();

                    SingletonWaterLevelModel.Instance.SetSeries(SingletonWaterLevelDataList.Instance);
                    SingletonWaterFlowModel.Instance.SetSeries(SingletonWaterFlowDataList.Instance);
                    CreatePlot();

                    // Add plot to the page
                    AddPlotToPage();

                    // Create onclick events for buttons and datepickers
                    WaterFlowButton.Clicked += WaterFlowButtonPressed;
                    WaterLevelButton.Clicked += WaterLevelButtonPressed;
                    StartDatePicker.DateSelected += StartDatePicker_DateSelected;
                    EndDatePicker.DateSelected += EndDatePicker_DateSelected;

                    // Check alert
                    CheckIfAlertIsOn();
                });
            });
        }

        public async Task GetWaterData(DateTime start, DateTime end)
        {
            Debug.WriteLine("GET WATER DATA: " + start.ToString() + " - " + end.ToString());

            DateTime startTime = start;
            DateTime endTime = end;

            startTime = new DateTime(start.Year, start.Month, start.Day, 0, 0, 0);
            endTime = new DateTime(end.Year, end.Month, end.Day, 23, 59, 59);

            // Clear old datalists
            SingletonWaterFlowDataList dataListFlow = SingletonWaterFlowDataList.Instance;
            SingletonWaterLevelDataList dataListLevel = SingletonWaterLevelDataList.Instance;
            dataListFlow.Clear();
            dataListLevel.Clear();

            // Get Data and parse it
            await Task.Run(async () =>
            {
                var WaterFlowDataRequest = new GraphQLRequest
                {
                    Query = "{ water_data_query( start_time: \"" 
                            + startTime.ToString("yyyy-MM-ddTHH:mm:ss") + "\" end_time: \"" 
                            + endTime.ToString("yyyy-MM-ddTHH:mm:ss") + "\" ) { flow level timestamp } }"
                };
                var graphQLClient = new GraphQLClient("http://api.otpt18-c.course.tamk.cloud/");
                var graphQLResponse = await graphQLClient.PostAsync(WaterFlowDataRequest);
                var dynamicData = graphQLResponse.Data["water_data_query"];

                // Add fetched data into datalists
                foreach (JObject data in dynamicData)
                {
                    DateTime time = (new DateTime(1970, 1, 1)).AddMilliseconds(double.Parse(data["timestamp"].ToString()));
                    dataListFlow.Add(new WaterFlowData(
                        // Date
                        time,
                        // Flow in cubic meter per second
                        float.Parse(data["flow"].ToString())
                        ));
                    dataListLevel.Add(new WaterLevelData(
                        // Date
                        time,
                        // Level in meters
                        float.Parse(data["level"].ToString())/100 // value is in centimeters so dividing by 100
                        ));
                }
            }).ContinueWith((task) => {
                // Update plots
                Device.BeginInvokeOnMainThread(() =>
                {
                    // Update date limits
                    UpdateDatePickers();

                    SingletonWaterLevelModel.Instance.SetSeries(SingletonWaterLevelDataList.Instance);
                    SingletonWaterFlowModel.Instance.SetSeries(SingletonWaterFlowDataList.Instance);
                    CreatePlot();
                });
            });

            // Update date limits
            UpdateDatePickers();
        }

        public void ShowWaterFlowData()
        {
            // Update UI
            Device.BeginInvokeOnMainThread(() =>
            {
                SingletonWaterFlowModel.Instance.SetSeries(SingletonWaterFlowDataList.Instance);

                SingletonPlot.Instance.Type = PlotType.WATERFLOW;
                CreatePlot();
            });
        }

        public void ShowWaterLevelData()
        {
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
