using System;
using System.Collections.Generic;
using System.Text;

namespace DamMobileApp
{
    class SingletonWaterFlowDataList : List<WaterFlowData>
    {
        private static SingletonWaterFlowDataList instance = null;
        private static readonly object padlock = new object();

        public static SingletonWaterFlowDataList Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new SingletonWaterFlowDataList();
                    }
                    return instance;
                }
            }
        }
    }
}
