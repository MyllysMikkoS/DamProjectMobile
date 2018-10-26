using OxyPlot.Xamarin.Forms;
using System;
using System.Collections.Generic;
using System.Text;

namespace DamMobileApp
{
    public enum PlotType
    {
        WATERLEVEL,
        WATERFLOW
    }

    public sealed class SingletonPlot : PlotView
    {
        private static SingletonPlot instance = null;
        private static readonly object padlock = new object();
        public PlotType Type { get; set; }

        SingletonPlot()
        {
            Type = PlotType.WATERLEVEL;
        }

        public static SingletonPlot Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new SingletonPlot();
                    }
                    return instance;
                }
            }
        }
    }
}
