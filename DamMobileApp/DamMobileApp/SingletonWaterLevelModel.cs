using System;
using System.Collections.Generic;
using System.Text;

namespace DamMobileApp
{
    class SingletonWaterLevelModel : OxyPlot.PlotModel
    {
        private static SingletonWaterLevelModel instance = null;
        private static readonly object padlock = new object();

        SingletonWaterLevelModel()
        {
            Title = "Water level";
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
    }
}
