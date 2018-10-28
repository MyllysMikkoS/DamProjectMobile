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
    class SingletonWaterFlowModel : OxyPlot.PlotModel
    {
        private static SingletonWaterFlowModel instance = null;
        private static readonly object padlock = new object();
        public double MinFlow = 0;
        public double MaxFlow = 8;

        SingletonWaterFlowModel()
        {
            // Init title
            Title = "Water flow";

            // Set series
            SetSeries(SingletonWaterFlowDataList.Instance);
        }

        public static SingletonWaterFlowModel Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new SingletonWaterFlowModel();
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
            DateTime maxDate = new DateTime(end.Year, end.Month, end.Day, 0, 0, 0).Add(new TimeSpan(24,0,0));
            return DateTimeAxis.ToDouble(maxDate);
        }

        public void SetSeries(List<WaterFlowData> list)
        {
            try
            {
                // Init axes
                Axes.Clear();
                Axes.Add(new LinearAxis()
                {
                    Title = "square meters per second",
                    Position = AxisPosition.Left,
                    Minimum = MinFlow,
                    Maximum = MaxFlow,
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

                // Create series with new values
                list.Sort((x, y) => x.Timestamp.CompareTo(y.Timestamp));
                LineSeries series = new LineSeries
                {
                    Color = OxyColor.Parse("#0000ff")
                };
                foreach (WaterFlowData data in list)
                {
                    series.Points.Add(new DataPoint(DateTimeAxis.ToDouble(data.Timestamp), data.WaterFlow));
                }

                // Update series
                Series.Clear();
                Series.Add(series);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error: " + e.ToString());
            }

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
