using System;
using System.Collections.Generic;
using System.Text;

namespace DamMobileApp
{
    class SingletonWaterLevelDataList
    {
        private static SingletonWaterLevelDataList instance = null;
        private static readonly object padlock = new object();

        public static SingletonWaterLevelDataList Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new SingletonWaterLevelDataList();
                    }
                    return instance;
                }
            }
        }
    }
}
