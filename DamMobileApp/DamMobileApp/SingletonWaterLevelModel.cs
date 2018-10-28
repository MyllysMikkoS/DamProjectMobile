using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace DamMobileApp
{
    class SingletonWaterLevelModel : OxyPlot.PlotModel
    {
        private static SingletonWaterLevelModel instance = null;
        private static readonly object padlock = new object();
        public double MinLevel = 86.40;
        public double MaxLevel = 87.60;
        public double AlertLineValue = 100;
        public LineSeries AlertLine;

        SingletonWaterLevelModel()
        {
            // Init title
            Title = "Water level";

            // Init alert line
            AlertLine = new LineSeries
            {
                Color = OxyColor.Parse("#ff0000"),
                Title = "Alert line"
            };

            // Set series
            SetSeries(SingletonWaterLevelDataList.Instance);
        }

        public static SingletonWaterLevelModel Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new SingletonWaterLevelModel();
                    }
                    return instance;
                }
            }
        }

        public double GetMinDateInDouble()
        {
            DateTime start = GlobalDateLimits.Instance.StartDate;
            DateTime minDate = new DateTime(start.Year, start.Month, start.Day, 0, 0, 0);
            return DateTimeAxis.ToDouble(minDate);
        }

        public double GetMaxDateInDouble()
        {
            DateTime end = GlobalDateLimits.Instance.EndDate;
            DateTime maxDate = new DateTime(end.Year, end.Month, end.Day, 0, 0, 0).Add(new TimeSpan(24, 0, 0));
            return DateTimeAxis.ToDouble(maxDate);
        }

        public void SetSeries(List<WaterLevelData> list)
        {
            try
            {
                // Init axes
                Axes.Clear();
                Axes.Add(new LinearAxis()
                {
                    Title = "meters",
                    Position = AxisPosition.Left,
                    Minimum = MinLevel,
                    Maximum = MaxLevel,
                    IsPanEnabled = false,
                    IsZoomEnabled = false,
                    MajorGridlineColor = OxyColor.Parse("#dddddd"),
                    MajorGridlineThickness = 2,
                    MajorGridlineStyle = LineStyle.Solid,
                    MinorGridlineColor = OxyColor.Parse("#dddddd"),
                    MinorGridlineThickness = 1,
                    MinorGridlineStyle = LineStyle.Solid
                });
                Axes.Add(new DateTimeAxis()
                {
                    Position = AxisPosition.Bottom,
                    Minimum = GetMinDateInDouble(),
                    Maximum = GetMaxDateInDouble(),
                    IsPanEnabled = false,
                    IsZoomEnabled = false,
                    MajorGridlineColor = OxyColor.Parse("#dddddd"),
                    MajorGridlineThickness = 2,
                    MajorGridlineStyle = LineStyle.Solid,
                    Angle = 90
                });

                // Set alert line
                AlertLine.Points.Clear();
                AlertLine.Points.Add(new DataPoint(GetMinDateInDouble(), AlertLineValue));
                AlertLine.Points.Add(new DataPoint(GetMaxDateInDouble(), AlertLineValue));

                // Create series with new values
                list.Sort((x, y) => x.Timestamp.CompareTo(y.Timestamp));
                LineSeries series = new LineSeries
                {
                    Color = OxyColor.Parse("#0000ff")
                };
                foreach (WaterLevelData data in list)
                {
                    series.Points.Add(new DataPoint(DateTimeAxis.ToDouble(data.Timestamp), data.WaterLevel));
                }

                // Update series
                Series.Clear();
                Series.Add(series);
                Series.Add(AlertLine);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error: " + e.ToString());
            }

            // Update graph UI
            try
            {
                InvalidatePlot(true);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message + "\nReinitializing model");
            }
        }
    }
}
