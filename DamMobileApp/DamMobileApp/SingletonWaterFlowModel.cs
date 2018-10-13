using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Text;

namespace DamMobileApp
{
    class SingletonWaterFlowModel : OxyPlot.PlotModel
    {
        private static SingletonWaterFlowModel instance = null;
        private static readonly object padlock = new object();
        public DateTime MinDate { get; set; } = new DateTime(1111, 1, 1);
        public DateTime MaxDate { get; set; } = new DateTime(9999, 9, 9);
        public double MinFlow = 0;
        public double MaxFlow = 100;
        public double AlertLineValue = 70;
        public LineSeries AlertLine;

        SingletonWaterFlowModel()
        {
            // Init title
            Title = "Water flow";

            // Init alert line
            AlertLine = new LineSeries
            {
                Color = OxyColor.Parse("#ff0000"),
                Title = "Alert line"
            };

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
            return DateTimeAxis.ToDouble(MinDate);
        }

        public double GetMaxDateInDouble()
        {
            return DateTimeAxis.ToDouble(MaxDate);
        }

        public void SetSeries(List<WaterFlowData> list)
        {
            // Init axes
            Axes.Clear();
            Axes.Add(new LinearAxis()
            {
                Position = AxisPosition.Left,
                Minimum = MinFlow,
                Maximum = MaxFlow,
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
            foreach (WaterFlowData data in list)
            {
                series.Points.Add(new DataPoint(DateTimeAxis.ToDouble(data.Timestamp), data.WaterFlow));
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
