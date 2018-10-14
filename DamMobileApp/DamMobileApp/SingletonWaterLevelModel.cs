using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Text;

namespace DamMobileApp
{
    class SingletonWaterLevelModel : OxyPlot.PlotModel
    {
        private static SingletonWaterLevelModel instance = null;
        private static readonly object padlock = new object();
        public double MinLevel = 0;
        public double MaxLevel = 100;
        public double AlertLineValue = 70;
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
            return DateTimeAxis.ToDouble(GlobalDateLimits.Instance.StartDate);
        }

        public double GetMaxDateInDouble()
        {
            return DateTimeAxis.ToDouble(GlobalDateLimits.Instance.EndDate);
        }

        public void SetSeries(List<WaterLevelData> list)
        {
            // Init axes
            Axes.Clear();
            Axes.Add(new LinearAxis()
            {
                Position = AxisPosition.Left,
                Minimum = MinLevel,
                Maximum = MaxLevel,
                IsPanEnabled = false,
                IsZoomEnabled = false
            });
            Axes.Add(new DateTimeAxis()
            {
                Position = AxisPosition.Bottom,
                Minimum = GetMinDateInDouble(),
                Maximum = GetMaxDateInDouble(),
                IsPanEnabled = false,
                IsZoomEnabled = false
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

            // Update graph UI
            InvalidatePlot(true);
        }
    }
}
