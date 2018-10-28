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
        bool DatePickerUpdateFlag = false;

        public MainPage()
        {
            InitializeComponent();
            Debug.WriteLine("Initializing MAIN PAGE");
            try
            {
                // Enable loading screen
                MainLayout.RaiseChild(LoadingView);
                LoadingViewEnable();

                // Get water data on app start
                Task.Run(async () => await GetAllWaterDataOnAppStart());
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error: " + e.ToString());
            }
        }

        public void LoadingViewEnable()
        {
            Device.BeginInvokeOnMainThread(() => {
                LoadingView.IsEnabled = true;
                LoadingView.IsVisible = true;
                LoadingSpinner.IsRunning = true;
            });
        }

        public void LoadingViewDisable()
        {
            Device.BeginInvokeOnMainThread(() => {
                LoadingView.IsEnabled = false;
                LoadingView.IsVisible = false;
                LoadingSpinner.IsRunning = false;
            });
        }

        private void EndDatePicker_DateSelected(object sender, DateChangedEventArgs e)
        {
            if (!DatePickerUpdateFlag)
                Task.Run(async () => { await GetWaterData(StartDatePicker.Date, e.NewDate); });
        }

        private void StartDatePicker_DateSelected(object sender, DateChangedEventArgs e)
        {
            if (!DatePickerUpdateFlag)
                Task.Run(async () => { await GetWaterData(e.NewDate, EndDatePicker.Date); });
        }

        public void CheckIfAlertIsOn()
        {
            try {
                // Do the check
                if (SingletonWaterLevelDataList.Instance.OrderByDescending(x => x.Timestamp).First().WaterLevel
                    >= SingletonWaterLevelModel.Instance.AlertLineValue)
                {
                    // Display alert
                    Debug.WriteLine("DISPLAYING ALERT");
                    DisplayAlert("Alert!", "The water level has passed alert level.", "OK");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error: " + e.ToString());
            }
        }

        public void CreatePlot()
        {
            try
            {
                Debug.WriteLine("CREATING PLOT");
                // Get PlotModel according to plotviewtype
                if (SingletonPlot.Instance.Type == PlotType.WATERLEVEL)
                    SingletonPlot.Instance.Model = SingletonWaterLevelModel.Instance;
                else
                    SingletonPlot.Instance.Model = SingletonWaterFlowModel.Instance;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error: " + e.ToString());
            }
        }

        public void AddPlotToPage()
        {
            try
            {
                SingletonPlot.Instance.VerticalOptions = LayoutOptions.Fill;
                SingletonPlot.Instance.HorizontalOptions = LayoutOptions.Fill;
                PlotLayout.Children.Add(SingletonPlot.Instance, Constraint.Constant(0));
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error: " + e.ToString());
            }
        }

        public void UpdateDatePickers()
        {
            try
            {
                // Updating dates
                DatePickerUpdateFlag = true;

                Debug.WriteLine("SETTING DATES AND TIMES");
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
                DateTime FinlandTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, App.FinlandTimeZone);
                TimeSpan span = FinlandTime - DateTime.UtcNow;
                Debug.WriteLine("-------------------------------------------------- " + span.ToString() + " ---- " + GlobalDateLimits.Instance.EndDate.ToString());
                EndDatePicker.MaximumDate = DateTime.UtcNow.Add(span);

                // Set current values
                StartDatePicker.Date = GlobalDateLimits.Instance.StartDate;
                EndDatePicker.Date = GlobalDateLimits.Instance.EndDate;

                DateTime StartDateMax = new DateTime(end.Year, end.Month, end.Day);
                DateTime EndDateMin = new DateTime(start.Year, start.Month, start.Day);
                StartDatePicker.MaximumDate = StartDateMax;
                EndDatePicker.MinimumDate = EndDateMin;

                // End updating datepickers
                DatePickerUpdateFlag = false;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error: " + e.ToString());
            }
        }

        public void UpdateDatePickersOnAppStart()
        {
            try
            {
                // Updating dates
                DatePickerUpdateFlag = true;

                Debug.WriteLine("APP START DATES");
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
                DateTime FinlandTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, App.FinlandTimeZone);
                TimeSpan span = FinlandTime - DateTime.UtcNow;
                EndDatePicker.MaximumDate = DateTime.UtcNow.Add(span);

                // Set current values
                StartDatePicker.Date = GlobalDateLimits.Instance.StartDate;
                EndDatePicker.Date = GlobalDateLimits.Instance.EndDate;

                DateTime StartDateMax = new DateTime(end.Year, end.Month, end.Day);
                DateTime EndDateMin = new DateTime(start.Year, start.Month, start.Day);
                StartDatePicker.MaximumDate = StartDateMax;
                EndDatePicker.MinimumDate = EndDateMin;

                // End updating datepickers
                DatePickerUpdateFlag = false;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error: " + e.ToString());
            }
        }

        public void WaterFlowButtonPressed(object sender, EventArgs e)
        {
            ShowWaterFlowData();
            Debug.WriteLine("WATERFLOW BUTTON");
        }

        public void WaterLevelButtonPressed(object sender, EventArgs e)
        {
            ShowWaterLevelData();
            Debug.WriteLine("WATERLEVEL BUTTON");
        }

        public async Task GetAllWaterDataOnAppStart()
        {
            try
            {
                Debug.WriteLine("GET DATA ON APP START");
                Device.BeginInvokeOnMainThread(() => { LoadingLabel.Text = "Getting water data..."; });

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

                    Device.BeginInvokeOnMainThread(() => { LoadingLabel.Text = "Updating graphs..."; });

                    // Get timeDifference
                    DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, App.FinlandTimeZone);
                    TimeSpan timeDifference = localTime - DateTime.UtcNow;

                    // Add fetched data into datalists
                    foreach (JObject data in dynamicData)
                    {
                        DateTime time = new DateTime(1970, 1, 1).AddMilliseconds(double.Parse(data["timestamp"].ToString())).Add(timeDifference);
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
                    Debug.WriteLine("GOT WATER DATA, FLOW OBJECTS: " + dataListFlow.Count + " - LEVEL OBJECTS: " + dataListLevel.Count);
                    Debug.WriteLine("GETTING ALERT LEVEL");

                    var AlertLevelRequest = new GraphQLRequest
                    {
                        Query = "{ water_level_alarm{ alarm_level } }"
                    };
                    graphQLResponse = await graphQLClient.PostAsync(AlertLevelRequest);
                    var alertData = graphQLResponse.Data["water_level_alarm"];
                    JObject alertLvl = alertData;

                    Debug.WriteLine("-------- ALERT LEVEL IS: " + alertLvl["alarm_level"].ToString());

                    // The alarm level is in centimeters in database so dividing by 100 to get value in meters
                    SingletonWaterLevelModel.Instance.AlertLineValue = double.Parse(alertLvl["alarm_level"].ToString()) / 100;
                }).ContinueWith((task) =>
                {
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

                        // Disable loading screen
                        LoadingViewDisable();

                        // Check alert
                        CheckIfAlertIsOn();

                        Debug.WriteLine("---------- WATER DATA GOT AND UI UPDATED");
                    });
                });
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error: " + e.ToString());
            }
        }

        public async Task GetWaterData(DateTime start, DateTime end)
        {
            try
            {
                // Enable loading screen
                LoadingViewEnable();
                Debug.WriteLine("GET WATER DATA: " + start.ToString() + " - " + end.ToString());
                Device.BeginInvokeOnMainThread(() => { LoadingLabel.Text = "Getting water data..."; });

                DateTime startTime = start;
                DateTime endTime = end;

                // Set timespan - minusSpan (UTC - FINNISH TIME) is to get UTC data relative to finnish time datePickers
                // e.g. 10/10/2018 00:00:00 - 11/10/2018 00:00:00 FI TIME (+2) is 9/10/2018 22:00:00 - 10/10/2018 22:00:00 UTC TIME
                DateTime FinlandTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, App.FinlandTimeZone);
                TimeSpan minusSpan = DateTime.UtcNow - FinlandTime;
                startTime = new DateTime(start.Year, start.Month, start.Day, 0, 0, 0).Add(minusSpan);
                endTime = new DateTime(end.Year, end.Month, end.Day).Add(new TimeSpan(23, 59, 59).Add(minusSpan));

                // Clear old datalists
                SingletonWaterFlowDataList dataListFlow = SingletonWaterFlowDataList.Instance;
                SingletonWaterLevelDataList dataListLevel = SingletonWaterLevelDataList.Instance;
                dataListFlow.Clear();
                dataListLevel.Clear();

                // Get Data and parse it
                await Task.Run(async () =>
                {
                    String startTm = String.Format("{0:s}", startTime);
                    String endTm = String.Format("{0:s}", endTime);
                    var WaterFlowDataRequest = new GraphQLRequest
                    {
                        Query = "{ water_data_query( start_time: \""
                                + startTm + "\" end_time: \""
                                + endTm + "\" ) { flow level timestamp } }"
                    };
                    Debug.WriteLine("{ water_data_query( start_time: \""
                                + startTm + "\" end_time: \""
                                + endTm + "\" ) { flow level timestamp } }");
                    var graphQLClient = new GraphQLClient("http://api.otpt18-c.course.tamk.cloud/");
                    var graphQLResponse = await graphQLClient.PostAsync(WaterFlowDataRequest);
                    var dynamicData = graphQLResponse.Data["water_data_query"];

                    Device.BeginInvokeOnMainThread(() => { LoadingLabel.Text = "Updating graphs..."; });

                    // Get timeDifference
                    DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, App.FinlandTimeZone);
                    TimeSpan timeDifference = localTime - DateTime.UtcNow;

                    // Add fetched data into datalists
                    foreach (JObject data in dynamicData)
                    {
                        DateTime time = new DateTime(1970, 1, 1).AddMilliseconds(double.Parse(data["timestamp"].ToString())).Add(timeDifference);
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
                    Debug.WriteLine("GOT WATER DATA, FLOW OBJECTS: " + dataListFlow.Count + " - LEVEL OBJECTS: " + dataListLevel.Count);
                    DateTime newestLevelDate = SingletonWaterLevelDataList.Instance.OrderByDescending(x => x.Timestamp).First().Timestamp;
                    Debug.WriteLine("-------------------------------------- NEWEST DATE ON LIST: " + newestLevelDate.ToString());
                }).ContinueWith((task) =>
                {
                // Update plots
                Device.BeginInvokeOnMainThread(() =>
                    {
                        // Update date limits
                        UpdateDatePickers();

                        SingletonWaterLevelModel.Instance.SetSeries(SingletonWaterLevelDataList.Instance);
                        SingletonWaterFlowModel.Instance.SetSeries(SingletonWaterFlowDataList.Instance);
                        CreatePlot();

                        // Disable loading screen
                        LoadingViewDisable();
                        Debug.WriteLine("----- WATER DATA GOT AND UI UPDATED");
                    });
                });
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error: " + e.ToString());
            }
        }

        public void ShowWaterFlowData()
        {
            try
            {
                // Update UI
                Device.BeginInvokeOnMainThread(() =>
                {
                    SingletonWaterFlowModel.Instance.SetSeries(SingletonWaterFlowDataList.Instance);

                    SingletonPlot.Instance.Type = PlotType.WATERFLOW;
                    CreatePlot();
                });
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error: " + e.ToString());
            }
        }

        public void ShowWaterLevelData()
        {
            try
            {
                // Update UI
                Device.BeginInvokeOnMainThread(() =>
                {
                    SingletonWaterLevelModel.Instance.SetSeries(SingletonWaterLevelDataList.Instance);

                    SingletonPlot.Instance.Type = PlotType.WATERLEVEL;
                    CreatePlot();
                });
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error: " + e.ToString());
            }
        }
    }
}
