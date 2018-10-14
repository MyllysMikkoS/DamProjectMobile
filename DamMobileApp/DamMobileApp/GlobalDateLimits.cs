using System;
using System.Collections.Generic;
using System.Text;

namespace DamMobileApp
{
    class GlobalDateLimits
    {
        private static GlobalDateLimits instance = null;
        private static readonly object padlock = new object();
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        GlobalDateLimits()
        {
            StartDate = new DateTime(2000, 1, 1);
            EndDate = new DateTime(2018, 12, 12);
        }

        public static GlobalDateLimits Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new GlobalDateLimits();
                    }
                    return instance;
                }
            }
        }
    }
}
